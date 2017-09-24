using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mazadaty.Services;
using Mazadaty.Web.Areas.admin.Models.WishList;
using Mazadaty.Web.Controllers;
using Mazadaty.Web.Core.Attributes;
using Mazadaty.Web.Core.Identity;
using Mazadaty.Web.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace Mazadaty.Web.Areas.admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("wishlist"), RoleAuthorize(Role.Administrator)]
    public class WishListsController : ApplicationController
    {
        private readonly WishListService _wishListService;

        public WishListsController(IAppServices appServices)
            : base(appServices)
        {
            _wishListService = new WishListService(DataContextFactory);
        }

        public async Task<ActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var m = ModelState;
            
            var model = await (new IndexViewModel()).Hydrate(_wishListService, startDate, endDate);
            return View(model);
        }

        public async Task<JsonResult> GetWishList([DataSourceRequest] DataSourceRequest request, DateTime? startDate = null, DateTime? endDate = null)
        {
            var wishList = (await _wishListService.GetGroupBy(startDate, endDate));
            return Json(wishList.ToDataSourceResult(request));
        }

        public async Task<ActionResult> Edit(string name)
        {
            var model = await (new EditViewModel()).Hydrate(_wishListService, name);
            if (!model.WishLists.Any())
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string name, EditViewModel model)
        {
            var wishlist = await _wishListService.GetByNameNormalized(name);
            if (!wishlist.Any())
            {
                return HttpNotFound();
            }
            wishlist.ForEach(i => i.NameNormalized = model.NameNormalized);
            await _wishListService.EditRange(wishlist);
            SetStatusMessage("Wishlist items has been edited successfully.");
            return RedirectToAction("Index");
        }
    }
}
