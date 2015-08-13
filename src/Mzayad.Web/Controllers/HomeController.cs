using Mzayad.Services;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Home;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}")]
    public class HomeController : ApplicationController
    {
        private readonly AuctionService _auctionService;
        
        public HomeController(IAppServices appServices) : base(appServices)
        {
            _auctionService = new AuctionService(appServices.DataContextFactory);
        }

        public async Task<ActionResult> Index(string language)
        {
            if (language == null)
            {
                return RedirectToAction("Index", new { Language });
            }

            //var auctions = await CacheService.TryGet(CacheKeys.CurrentAuctions, () => _auctionService.GetCurrentAuctions(Language), TimeSpan.FromDays(1));
            var auctions = await _auctionService.GetCurrentAuctions(Language);

            var viewModel = new IndexViewModel(auctions);

            return View(viewModel);
        }

        public ActionResult SignalR()
        {
            return View();
        }

        [Route("about")]
        public ActionResult About()
        {
            return View();
        }

        [Route("buy-now")]
        public ActionResult BuyNow()
        {
            return View();
        }

        [Route("contact-us")]
        public ActionResult ContactUs()
        {
            return View();
        }

        [Route("terms-and-conditions")]
        public ActionResult TermsAndConditions()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("change-language")]
        public ActionResult ChangeLanguage(string language, Uri returnUrl)
        {
            CookieService.Add(CookieKeys.LanguageCode, language, DateTime.Today.AddYears(10));

            var redirectUrl = Regex.Replace(returnUrl.ToString(), @"/(en|ar)/", "/" + language + "/");
            return Redirect(redirectUrl);
        }

        [Route("~/ok")]
        public ActionResult Ok()
        {
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}