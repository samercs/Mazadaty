using Mzayad.Core.Exceptions;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Services.Payment;
using Mzayad.Web.Areas.Api.ErrorHandling;
using Mzayad.Web.Areas.Api.Models.Orders;
using Mzayad.Web.Areas.Api.Models.Subscriptions;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Shared;
using Mzayad.Web.Resources;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Localization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/subscriptions")]
    public class SubscriptionsController : ApplicationApiController
    {
        private readonly SubscriptionService _subscriptionService;
        private readonly AddressService _addressService;
        private readonly PrizeService _prizeService;
        private readonly KnetService _knetService;
        public SubscriptionsController(IAppServices appServices) : base(appServices)
        {
            _subscriptionService = new SubscriptionService(DataContextFactory);
            _addressService = new AddressService(DataContextFactory);
            _prizeService = new PrizeService(DataContextFactory);
            _knetService = new KnetService(DataContextFactory);
        }


        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var subscriptions = await _subscriptionService.GetActiveSubscriptions(Language);
            return Ok(subscriptions.Select(SubscriptionViewModel.Create));
        }

        [Route("{subscriptionId:int}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(int subscriptionId)
        {
            var subscription = await _subscriptionService.GetById(subscriptionId);
            return Ok(SubscriptionViewModel.Create(subscription.Localize(Language)));
        }
        [Route("buy/{subscriptionId:int}/tokens")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> BuyWithToken(int subscriptionId)
        {
            var resultModel = new BuySbscriptionResultViewModel
            {
                IsSuccess = false
            };

            var subscription = await _subscriptionService.GetValidSubscription(subscriptionId, Language);
            if (subscription == null)
            {
                return NotFound();
            }

            var user = await AuthService.CurrentUser();
            user.Address = await _addressService.GetAddress(user.AddressId);

            try
            {
                await _subscriptionService.BuySubscriptionWithTokens(subscription, user, AuthService.UserHostAddress());
                await UpdateCachedSubscription();
                resultModel.Message = StringFormatter.ObjectFormat(Global.SubscriptionPurchaseAcknowledgementApi, new { subscription });
                var prize = await InsertUserPrize();
                resultModel.PrizeId = prize.UserPrizeLogId;
                resultModel.IsSuccess = true;
                return Ok(resultModel);
            }
            catch (SubscriptionInvalidForPurchaseException)
            {
                return ApiErroResult(Global.SubscriptionNotValidForPurchaseErrorMessage,
                    ApiErrorType.SubscriptionNotValidForPurchase);
            }
            catch (SubscriptionCannotBePurchasesWithTokensException)
            {
                return ApiErroResult(Global.SubscriptionNotValidForPurchaseWithTokensErrorMessage,
                    ApiErrorType.SubscriptionNotValidForPurchaseWithTokens);
            }
            catch (InsufficientTokensException)
            {
                return ApiErroResult(Global.NoEnoughTokensErrorMessage,
                    ApiErrorType.InsufficientTokensError);
            }
        }

        [Route("buy/{subscriptionId:int}/knet")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> BuyWithKnet(int subscriptionId)
        {
            var subscription = await _subscriptionService.GetValidSubscription(subscriptionId, Language);
            if (subscription == null)
            {
                return NotFound();
            }

            var user = await AuthService.CurrentUser();
            user.Address = await _addressService.GetAddress(user.AddressId);
            var order = await _subscriptionService.BuySubscriptionWithKnet(subscription, user, AuthService.UserHostAddress());
            var result = await _knetService.InitTransaction(order, AuthService.CurrentUserId(), AuthService.UserHostAddress());
            await UpdateCachedSubscription();
            var transaction = await _knetService.GetTransactionByOrderId(order.OrderId);
            return Ok(OrderCreateResult.Create(transaction, result));
        }


        private async Task UpdateCachedSubscription()
        {
            var user = await AuthService.CurrentUser();
            var cacheKey = string.Format(CacheKeys.UserSubscriptionUtc, user.Id);
            CacheService.Set(cacheKey, new SubscriptionExpiration(user.SubscriptionUtc));
        }

        private async Task<UserPrizeLog> InsertUserPrize()
        {
            var user = await AuthService.CurrentUser();
            var prize = await _prizeService.InsertUserPrize(user);
            //var url = $"{AppSettings.CanonicalUrl}{Url.Action("Index", "Prize", new { id = prize.UserPrizeLogId, Language })}";
            return prize;
        }

    }
}
