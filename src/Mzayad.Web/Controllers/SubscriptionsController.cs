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

        [Route("buy")]
        public async Task<ActionResult> Buy()
        {
            var subscriptions = await _subscriptionService.GetActiveSubscriptions(Language);

            return View(subscriptions);
        }

        [Route("buy-now/{subscriptionId:int}")]
        public async Task<ActionResult> BuyNow(int subscriptionId)
        {
            var subscription = await _subscriptionService.GetActiveSubscription(subscriptionId, Language);
            if (subscription == null)
            {
                return HttpNotFound();
            }

            var viewModel = new BuyNowViewModel
            {
                Subscription = subscription
            };

            return View(viewModel);
        }
    }
}