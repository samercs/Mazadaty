using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Services;
using Mzayad.Web.Core.Services;

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
    }
}