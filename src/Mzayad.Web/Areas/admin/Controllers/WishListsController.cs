using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.WishList;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using WebGrease.Css.Extensions;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("wishlist"), RoleAuthorize(Role.Administrator)]
    public class WishListsController : ApplicationController
    {

        private readonly WishListService _wishListService;


        // GET: admin/WishList
        public WishListsController(IAppServices appServices) : base(appServices)
        {
            _wishListService=new WishListService(DataContextFactory);
        }

        public async Task<ActionResult> Index(DateTime? StartDate ,DateTime? EndDate)
        {
            
            var model = await (new IndexViewModel()).Hydrate(_wishListService,StartDate,EndDate);
            return View(model);
        }

        public async Task<JsonResult> GetWishList([DataSourceRequest] DataSourceRequest request,DateTime? startDate=null,DateTime? endDate=null)
        {
            var wishList = (await _wishListService.GetGroupBy(startDate,endDate));
            return Json(wishList.ToDataSourceResult(request));
        }


        public async Task<ActionResult> Edit(string name)
        {
            var model = await (new EditViewModel()).Hydrate(_wishListService,name);
            if (!model.WishLists.Any())
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string name, EditViewModel model)
        {
            var wishlist =await _wishListService.GetByNameNormalized(name);
            if (!wishlist.Any())
            {
                return HttpNotFound();
            }
            wishlist.ForEach(i=>i.NameNormalized=model.NameNormalized);
            await _wishListService.EditRange(wishlist);
            SetStatusMessage("Wishlist items has been edited successfully.");
            return RedirectToAction("Index");

        }
    }
}