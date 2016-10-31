using Mzayad.Services;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.WishList;
using Mzayad.Web.Resources;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/wishlist"), Authorize]
    public class WishListController : ApplicationController
    {
        private readonly WishListService _wishListService;
        private readonly ProductService _productService;

        public WishListController(IAppServices appServices) : base(appServices)
        {
            _wishListService = new WishListService(DataContextFactory);
            _productService = new ProductService(DataContextFactory);
        }

        public async Task<ActionResult> Index()
        {
            var userId = AuthService.CurrentUserId();
            var userWishList = await _wishListService.GetByUser(userId);
            var model = new IndexViewModel()
            {
                WishList = userWishList
            };
            return View(model);
        }

        public async Task<ActionResult> Add(string item = "")
        {
            if (!string.IsNullOrEmpty(item))
            {
                item = HttpUtility.UrlDecode(item);
            }

            var model = await new AddViewModel().Hydrate(AuthService, _productService);
            model.WishList.NameEntered = item;

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddViewModel model)
        {
            model.WishList.NameNormalized = model.WishList.NameEntered;
            await _wishListService.Add(model.WishList);

            SetStatusMessage(Global.WishListItemAddedMessage);

            return RedirectToAction("Index");
        }

        public ActionResult Remove(int id)
        {
            return DeleteConfirmation(Global.RemoveItem, Global.WishListItemDeleteConfirmation);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Remove(int id, FormCollection formCollection)
        {
            var wishlist = await _wishListService.GetById(id);
            if (wishlist == null)
            {
                return HttpNotFound();
            }
            if (wishlist.UserId != AuthService.CurrentUserId())
            {
                return HttpNotFound();
            }
            await _wishListService.Delete(wishlist);
            SetStatusMessage(Global.WishListItemRemoveMessage);
            return RedirectToAction("Index");
        }
    }
}