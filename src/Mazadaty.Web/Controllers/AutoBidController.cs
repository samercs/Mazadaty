using Mazadaty.Services;
using Mazadaty.Web.Core.Attributes;
using Mazadaty.Web.Core.Services;
using Mazadaty.Web.Models;
using Mazadaty.Web.Resources;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mazadaty.Web.Controllers
{
    [LanguageRoutePrefix("auto-bid"), Authorize]
    public class AutoBidController : ApplicationController
    {
        private readonly AuctionService _auctionService;
        private readonly AutoBidService _autoBidService;

        public AutoBidController(IAppServices appServices) : base(appServices)
        {
            _auctionService = new AuctionService(DataContextFactory, appServices.QueueService);
            _autoBidService = new AutoBidService(DataContextFactory);
        }

        [Route("edit/{auctionId:int}")]
        public async Task<ActionResult> Edit(int auctionId)
        {
            var userId = AuthService.CurrentUserId();
            var auction = await _auctionService.GetAuction(auctionId, Language);
            var autoBid = await _autoBidService.Get(userId, auctionId);
            
            var viewModel = new EditAutoBidViewModel
            {
                Auction = auction,
                AutoBid = autoBid
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("edit/{auctionId:int}")]
        public async Task<ActionResult> Edit(int auctionId, EditAutoBidViewModel model)
        {
            var userId = AuthService.CurrentUserId();
            await _autoBidService.Save(userId, auctionId, model.AutoBid.MaxBid);

            SetStatusMessage(Global.AutoBidSaveSuccess);

            return RedirectToAction("Edit", new { auctionId });
        }

        [Route("delete/{auctionId:int}")]
        public ActionResult Delete(int auctionId)
        {
            return DeleteConfirmation(Global.AutoBidDeleteTitle, Global.AutoBidDeleteConfirmation);
        }

        [HttpPost]
        [Route("delete/{auctionId:int}")]
        public async Task<ActionResult> Delete(int auctionId, FormCollection formCollection)
        {
            var userId = AuthService.CurrentUserId();
            await _autoBidService.Delete(userId, auctionId);

            SetStatusMessage(Global.AutoBidDeleteSuccess);

            return RedirectToAction("Edit", new { auctionId });
        }
    }
}
