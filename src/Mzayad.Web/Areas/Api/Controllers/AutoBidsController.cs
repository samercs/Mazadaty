using Mzayad.Services;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Auctions;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/auctions/{auctionId:int}/auto-bid")]
    [Authorize]
    public class AutoBidsController : ApplicationApiController
    {
        private readonly AutoBidService _autoBidService;

        public AutoBidsController(IAppServices appServices) : base(appServices)
        {
            _autoBidService = new AutoBidService(DataContextFactory);
        }

        [Route(""), HttpPost]
        public async Task<IHttpActionResult> SetAutoBid(AuctionViewModel model)
        {
            var userId = AuthService.CurrentUserId();
            await _autoBidService.Save(userId, model.AuctionId, model.MaximumBid ?? 0);
            return Ok();
        }

        [Route(""), HttpDelete]
        public async Task<IHttpActionResult> DeleteAutoBid(int auctionId)
        {
            var userId = AuthService.CurrentUserId();
            await _autoBidService.Delete(userId, auctionId);
            return Ok();
        }

        [Route(""), HttpGet]
        public async Task<IHttpActionResult> GeAutoBid(int auctionId)
        {
            var userId = AuthService.CurrentUserId();
            var autoBid = await _autoBidService.Get(userId, auctionId);
            var maxBid = autoBid?.MaxBid ?? 0;

            return Ok(maxBid);
        }

    }
}
