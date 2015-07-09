using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Services;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Subscriptions;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/subscriptions"), Authorize]
    public class SubscriptionsController : ApplicationController
    {
        private readonly SubscriptionService _subscriptionService;
        
        public SubscriptionsController(IAppServices appServices) : base(appServices)
        {
            _subscriptionService = new SubscriptionService(DataContextFactory);
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
    }
}