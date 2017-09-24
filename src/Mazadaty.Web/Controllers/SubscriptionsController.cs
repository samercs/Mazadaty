using Mazadaty.Core.Exceptions;
using Mazadaty.Services;
using Mazadaty.Services.Payment;
using Mazadaty.Web.Core.Configuration;
using Mazadaty.Web.Core.Services;
using Mazadaty.Web.Models.Shared;
using Mazadaty.Web.Models.Subscriptions;
using Mazadaty.Web.Resources;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Base.Web;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mazadaty.Web.Controllers
{
    [RoutePrefix("{language:regex(^en|ar$)}/subscriptions"), Authorize]
    public class SubscriptionsController : ApplicationController
    {
        private readonly SubscriptionService _subscriptionService;
        private readonly AddressService _addressService;
        private readonly KnetService _knetService;
        private readonly PrizeService _prizeService;

        public SubscriptionsController(IAppServices appServices) : base(appServices)
        {
            _subscriptionService = new SubscriptionService(DataContextFactory);
            _addressService = new AddressService(DataContextFactory);
            _knetService = new KnetService(DataContextFactory);
            _prizeService = new PrizeService(DataContextFactory);
        }

        [Route("")]
        public async Task<ActionResult> Index()
        {
            var subscriptions = await _subscriptionService.GetActiveSubscriptions(Language);

            return View(subscriptions);
        }

        [Route("buy/{subscriptionId:int}")]
        public async Task<ActionResult> Buy(int subscriptionId)
        {
            var subscription = await _subscriptionService.GetValidSubscription(subscriptionId, Language);
            if (subscription == null)
            {
                return HttpNotFound();
            }

            var user = await AuthService.CurrentUser();

            var viewModel = new BuyNowViewModel
            {
                Subscription = subscription,
                AvailableTokens = user.Tokens
            };

            return View(viewModel);
        }

        [Route("buy/{subscriptionId:int}/tokens")]
        public async Task<ActionResult> BuyWithTokens(int subscriptionId)
        {
            var subscription = await _subscriptionService.GetValidSubscription(subscriptionId, Language);
            if (subscription == null)
            {
                return HttpNotFound();
            }

            var user = await AuthService.CurrentUser();

            var viewModel = new BuyNowViewModel
            {
                Subscription = subscription,
                AvailableTokens = user.Tokens
            };

            return View(viewModel);
        }

        [Route("buy/{subscriptionId:int}/tokens")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> BuyWithTokens(int subscriptionId, FormCollection formCollection)
        {
            var subscription = await _subscriptionService.GetValidSubscription(subscriptionId, Language);
            if (subscription == null)
            {
                return HttpNotFound();
            }

            var user = await AuthService.CurrentUser();
            user.Address = await _addressService.GetAddress(user.AddressId);

            try
            {
                await _subscriptionService.BuySubscriptionWithTokens(subscription, user, AuthService.UserHostAddress());
                await UpdateCachedSubscription();
                SetStatusMessage(StringFormatter.ObjectFormat(Global.SubscriptionPurchaseAcknowledgement, new { subscription }));
                var prizeUrl = await InsertUserPrize();
                return Redirect(prizeUrl);
            }
            catch (SubscriptionInvalidForPurchaseException)
            {
                SetStatusMessage(Global.SubscriptionNotValidForPurchaseErrorMessage, StatusMessageType.Error);
            }
            catch (SubscriptionCannotBePurchasesWithTokensException)
            {
                SetStatusMessage(Global.SubscriptionNotValidForPurchaseWithTokensErrorMessage, StatusMessageType.Error);
            }
            catch (InsufficientTokensException)
            {
                SetStatusMessage(Global.NoEnoughTokensErrorMessage, StatusMessageType.Error);
            }

            return RedirectToAction("Dashboard", "User");
        }

        [Route("buy/{subscriptionId:int}/knet")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> BuyWithKnet(int subscriptionId, FormCollection formCollection)
        {
            var subscription = await _subscriptionService.GetValidSubscription(subscriptionId, Language);
            if (subscription == null)
            {
                return HttpNotFound();
            }

            var user = await AuthService.CurrentUser();
            user.Address = await _addressService.GetAddress(user.AddressId);

            var order = await _subscriptionService.BuySubscriptionWithKnet(subscription, user, AuthService.UserHostAddress());
            var result = await _knetService.InitTransaction(order, AuthService.CurrentUserId(), AuthService.UserHostAddress());

            await UpdateCachedSubscription();

            return Redirect(result.RedirectUrl);
        }

        private async Task UpdateCachedSubscription()
        {
            var user = await AuthService.CurrentUser();
            var cacheKey = string.Format(CacheKeys.UserSubscriptionUtc, user.Id);
            CacheService.Set(cacheKey, new SubscriptionExpiration(user.SubscriptionUtc));
        }

        private async Task<string> InsertUserPrize()
        {
            var user = await AuthService.CurrentUser();
            var prize = await _prizeService.InsertUserPrize(user);
            var url = $"{AppSettings.CanonicalUrl}{Url.Action("Index", "Prize", new { id = prize.UserPrizeLogId, Language })}";
            return url;
        }
    }
}
