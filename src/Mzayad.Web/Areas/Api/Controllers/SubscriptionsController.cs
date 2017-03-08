using Mzayad.Services;
using Mzayad.Web.Areas.Api.Models.Subscriptions;
using Mzayad.Web.Core.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/subscriptions")]
    public class SubscriptionsController : ApplicationApiController
    {
        private readonly SubscriptionService _subscriptionService;
        public SubscriptionsController(IAppServices appServices) : base(appServices)
        {
            _subscriptionService = new SubscriptionService(DataContextFactory);
        }


        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var subscriptions = await _subscriptionService.GetActiveSubscriptions(Language);
            return Ok(subscriptions.Select(SubscriptionViewModel.Create));
        }

    }
}
