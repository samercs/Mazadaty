using System.Linq;
using Mzayad.Services;
using Mzayad.Web.Core.Services;
using System.Threading.Tasks;
using System.Web.Http;
using Mzayad.Web.Areas.Api.Models.Auctions;
using Mzayad.Web.Models;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/auctions")]
    public class AuctionsController : ApplicationApiController
    {
        private readonly AuctionService _auctionService;
        
        public AuctionsController(IAppServices appServices) : base(appServices)
        {
            _auctionService = new AuctionService(appServices.DataContextFactory);
        }

        [HttpGet]
        [Route("live")]
        public async Task<IHttpActionResult> GetLiveAuctions()
        {
            var auctions = await _auctionService.GetLiveAuctions(Language);
            var viewModel = auctions.Select(LiveAuctionExtendedModel.Create);

            return Ok(viewModel);
        }

        [HttpGet]
        [Route("{auctionId:int}")]
        public async Task<IHttpActionResult> GetAuction(int auctionId)
        {
            var auction = await _auctionService.GetAuction(auctionId, Language);
            if (auction == null)
            {
                return NotFound();
            }
            
            var viewModel = AuctionModel.Create(auction);

            return Ok(viewModel);
        }
    }
}
