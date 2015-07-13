using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Models.Enum;
using Mzayad.Services;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Subscriptions;
using Mzayad.Web.Resources;
using OrangeJetpack.Base.Core.Formatting;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language:regex(^en|ar$)}/subscriptions"), Authorize]
    public class SubscriptionsController : ApplicationController
    {
        private readonly SubscriptionService _subscriptionService;
        private readonly OrderService _orderService;
        private readonly AddressService _addressService;
        
        public SubscriptionsController(IAppServices appServices) : base(appServices)
        {
            _subscriptionService = new SubscriptionService(DataContextFactory);
            _orderService = new OrderService(DataContextFactory);
            _addressService = new AddressService(DataContextFactory);
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

            SetStatusMessage(StringFormatter.ObjectFormat(Global.SubscriptionPurchaseAcknowledgement, new { subscription }));

            return RedirectToAction("Dashboard", "User");
        }

    }
}