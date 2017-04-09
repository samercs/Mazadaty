using Mzayad.Services;
using Mzayad.Services.Identity;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Home;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Models;
using OrangeJetpack.Localization;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}")]
    public class HomeController : ApplicationController
    {
        private readonly UserService _userService;
        private readonly AuctionService _auctionService;
        private readonly AddressService _addressService;
        private readonly BannerService _bannerService;

        public HomeController(IAppServices appServices) : base(appServices)
        {
            _userService = new UserService(appServices.DataContextFactory);
            _auctionService = new AuctionService(appServices.DataContextFactory, appServices.QueueService);
            _addressService = new AddressService(appServices.DataContextFactory);
            _bannerService = new BannerService(DataContextFactory);
        }

        public async Task<ActionResult> Index(string language)
        {
            if (language == null)
            {
                return RedirectToAction("Index", new { Language });
            }

            var liveAuctions = await _auctionService.GetLiveAuctions(Language);
            var closedAuctions = await _auctionService.GetClosedAuctions(Language, 12);
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
        public async Task<ActionResult> BuyNow()
        {
            var auctions = await _auctionService.GetBuyNowAuctions(Language);
            return View(auctions);
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