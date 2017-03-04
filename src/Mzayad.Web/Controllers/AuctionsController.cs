using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Services;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Auctions;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/auctions")]
    public class AuctionsController : ApplicationController
    {
        private readonly AuctionService _auctionService;

        public AuctionsController(IAppServices appServices) : base(appServices)
        {
            _auctionService = new AuctionService(appServices.DataContextFactory, appServices.QueueService);
        }

        [Route("details")]
        public ActionResult Details(int id)
        {
            return HttpNotFound();
        }

        [Route("closed")]
        public async Task<ActionResult> Closed()
        {
            var closedAuctions = await _auctionService.GetClosedAuctions(Language, 100);
            var viewModel = closedAuctions.Select(AuctionViewModel.Create).ToList();

            return View(viewModel);
        } 
    }
}