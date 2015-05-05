using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Services;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.WishList;

namespace Mzayad.Web.Controllers
{
    public class WishListController : ApplicationController
    {
        private readonly WishListService _wishListService;
        private readonly IAuthService _authService;
        // GET: WishList
        public WishListController(IAppServices appServices) : base(appServices)
        {
            _wishListService=new WishListService(DataContextFactory);
            _authService = appServices.AuthService;
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
    }
}