using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.WishList;
using Mzayad.Web.Resources;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/wishlist"), Authorize]
    public class WishListController : ApplicationController
    {
        private readonly WishListService _wishListService;
        private readonly IAuthService _authService;
        private readonly ProductService _productService;
        // GET: WishList
        public WishListController(IAppServices appServices) : base(appServices)
        {
            _wishListService=new WishListService(DataContextFactory);
            _authService = appServices.AuthService;
            _productService=new ProductService(DataContextFactory);
        }

        public async Task<ActionResult> Index()
        {
            var userId = _authService.CurrentUserId();
            var userWishList =await _wishListService.GetByUser(userId);
            var model = new IndexViewModel()
            {
                WishList = userWishList
            };
            return View(model);
        }

        public async Task<ActionResult> Add()
        {
            var model = await (new AddViewModel()).Hydrate(_authService, _productService);

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


        public ActionResult Delete()
        {
            return DeleteConfirmation(Global.DeleteWishList, Global.WishListItemDeleteConfirmation);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var wishlist = await _wishListService.GetById(id);
            if (wishlist == null)
            {
                return HttpNotFound();
            }
            if (wishlist.UserId != _authService.CurrentUserId())
            {
                return HttpNotFound();
            }
            await _wishListService.Delete(wishlist);
            SetStatusMessage(Global.WishListItemRemoveMessage);
            return RedirectToAction("Index");
        }
    }
}