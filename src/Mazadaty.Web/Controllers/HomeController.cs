using Mazadaty.Services;
using Mazadaty.Services.Identity;
using Mazadaty.Web.Core.Configuration;
using Mazadaty.Web.Core.Services;
using Mazadaty.Web.Models.Home;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mazadaty.Models;
using OrangeJetpack.Localization;

namespace Mazadaty.Web.Controllers
{
    [RoutePrefix("{language}")]
    public class HomeController : ApplicationController
    {
        private readonly UserService _userService;
        private readonly AuctionService _auctionService;
        private readonly AddressService _addressService;
        private readonly BannerService _bannerService;
        private readonly CategoryService _categoryService;

        public HomeController(IAppServices appServices) : base(appServices)
        {
            _userService = new UserService(DataContextFactory);
            _auctionService = new AuctionService(DataContextFactory, appServices.QueueService);
            _addressService = new AddressService(DataContextFactory);
            _bannerService = new BannerService(DataContextFactory);
            _categoryService = new CategoryService(DataContextFactory);
        }

        public async Task<ActionResult> Index(string language)
        {
            if (language == null)
            {
                return RedirectToAction("Index", new { Language });
            }

            var liveAuctions = await _auctionService.GetLiveAuctions(Language);
            var closedAuctions = await _auctionService.GetClosedAuctions(Language, 8);
            var upcomingAuctions = await _auctionService.GetUpcomingAuctions(Language, 12);
            var banners = (await _bannerService.GetAll()).ToList();
            banners = banners.Localize<Banner>(Language, LocalizationDepth.OneLevel).ToList();
            var viewModel = new IndexViewModel(liveAuctions, closedAuctions, upcomingAuctions);
            viewModel.Banners = banners;

            var user = await AuthService.CurrentUser();
            viewModel.UserCountry = "-- No Address --";
            viewModel.User = user;
            if (user != null)
            {
                var userAddress = await _addressService.GetAddress(user.AddressId ?? 0);
                viewModel.UserCountry = userAddress?.CountryCode ?? "-- No Address --";
            }

            return View(viewModel);
        }

        [ChildActionOnly, OutputCache(Duration = 60, VaryByParam = "*", VaryByCustom = "User")]
        public PartialViewResult SubscriptionStatus()
        {
            DateTime? subscriptionUtc = null;

            if (AuthService.IsAuthenticated())
            {
                subscriptionUtc = _userService.GetUserSubscriptionUtc(AuthService.CurrentUserId());
            }

            return PartialView("_Layout_SubscriptionStatus", subscriptionUtc);
        }


        public ActionResult SignalR()
        {
            return View();
        }

        [Route("~/{language}/about")]
        public ActionResult About()
        {
            return View();
        }

        [Route("~/{language}/buy-now")]
        public async Task<ActionResult> BuyNow(string q = null, int? categoryId = null)
        {
            var model = new BuyNowModel
            {
                Search = q,
                CategoryId = categoryId,
                Auctions = await _auctionService.GetBuyNowAuctions(Language, q, categoryId),
                Categories = await _categoryService.GetCategoriesAsHierarchy(Language)
            };

            return View(model);
        }

        [Route("~/{language}/contact-us")]
        public ActionResult ContactUs()
        {
            return View();
        }

        [Route("~/{language}/terms-and-conditions")]
        public ActionResult TermsAndConditions()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("~/change-language")]
        public ActionResult ChangeLanguage(string language, Uri returnUrl)
        {
            CookieService.Add(CookieKeys.LanguageCode, language, DateTime.Today.AddYears(10));
            var redirectUrl = Regex.Replace(returnUrl.ToString(), @"/(en|ar)", "/" + language);
            return Redirect(redirectUrl);
        }

        [Route("~/ok")]
        public ActionResult Ok()
        {
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

    }
}
