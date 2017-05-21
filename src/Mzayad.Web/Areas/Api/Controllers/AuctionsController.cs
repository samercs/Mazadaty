using System;
using Mzayad.Services;
using Mzayad.Web.Areas.Api.Models.Auctions;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/auctions")]
    public class AuctionsController : ApplicationApiController
    {
        private const int AuctionsCount = 12;
        private readonly AuctionService _auctionService;

        public AuctionsController(IAppServices appServices) : base(appServices)
        {
            _auctionService = new AuctionService(DataContextFactory, appServices.QueueService);
        }

        [Route("")]
        public async Task<IHttpActionResult> GetLatestAuctions()
        {
            var liveAuctions = _auctionService.GetLiveAuctions(Language);
            var closedAuctions = _auctionService.GetClosedAuctions(Language, AuctionsCount);
            var upcomingAuctions = _auctionService.GetUpcomingAuctions(Language, AuctionsCount);

            await Task.WhenAll(liveAuctions, closedAuctions, upcomingAuctions);

            return Ok(new
            {
                live = liveAuctions.Result.Select(AuctionModel.Create),
                closed = closedAuctions.Result.Select(AuctionModel.Create),
                upcoming = upcomingAuctions.Result.Select(AuctionModel.Create)
            });
        }

        [Route("live")]
        public async Task<IHttpActionResult> GetLiveAuctions()
        {
            var auctions = await _auctionService.GetLiveAuctions(Language);
            var viewModel = auctions.Select(LiveAuctionExtendedModel.Create);

            return Ok(viewModel);
        }

        [Route("closed")]
        public async Task<IHttpActionResult> GetClosedAuctions()
        {
            var closedAuctions = await _auctionService.GetClosedAuctions(Language, 100);
            return Ok(closedAuctions.Select(AuctionModel.Create));
        }

        [Route("{auctionId:int}")]
        public async Task<IHttpActionResult> GetAuction(int auctionId)
        {
            var auction = await _auctionService.GetAuction(auctionId, Language);
            if (auction == null)
            {
                return NotFound();
            }
            if (DateTime.UtcNow.Subtract(auction.StartUtc).TotalDays > 7)
            {
                return NotFound();
            }
            var viewModel = AuctionModel.Create(auction);
            return Ok(viewModel);
        }


    }
}
