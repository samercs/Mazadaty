using Mzayad.Services;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Subscriptions;
using Mzayad.Web.Resources;
using OrangeJetpack.Base.Core.Formatting;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Services.Payment;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Models.Shared;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language:regex(^en|ar$)}/subscriptions"), Authorize]
    public class SubscriptionsController : ApplicationController
    {
        private readonly SubscriptionService _subscriptionService;
        private readonly AddressService _addressService;
        private readonly KnetService _knetService;
        
        public SubscriptionsController(IAppServices appServices) : base(appServices)
        {
            _subscriptionService = new SubscriptionService(DataContextFactory);
            _addressService = new AddressService(DataContextFactory);
            _knetService = new KnetService(DataContextFactory);
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

            await _subscriptionService.BuySubscriptionWithTokens(subscription, user, AuthService.UserHostAddress());

            await UpdateCachedSubscription();

            SetStatusMessage(StringFormatter.ObjectFormat(Global.SubscriptionPurchaseAcknowledgement, new { subscription }));

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
    }
}