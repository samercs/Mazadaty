using System.Linq;
using Mzayad.Services;
using Mzayad.Web.Core.Services;
using System.Threading.Tasks;
using System.Web.Http;
using Mzayad.Web.Areas.Api.Models.Auctions;

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
            var auctions = await _auctionService.GetCurrentAuctions("en"); // TODO use correct language
            var viewModel = auctions.Select(AuctionModel.Create);

            return Ok(viewModel);
        }

        [HttpGet]
        [Route("{auctionId:int}")]
        public async Task<IHttpActionResult> GetAuction(int auctionId)
        {
            var auction = await _auctionService.GetAuction(auctionId, "en"); // TODO use correct language
            if (auction == null)
            {
                return NotFound();
            }
            
            var viewModel = AuctionModel.Create(auction);

            return Ok(viewModel);
        }
    }
}
