using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mazadaty.Services;
using Mazadaty.Web.Core.Services;
using Mazadaty.Web.Models.Auctions;

namespace Mazadaty.Web.Controllers
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
