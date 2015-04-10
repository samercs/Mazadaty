using System.Diagnostics;
using System.Linq;
using Mzayad.Services;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Web.Models.Home;

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

            Trace.TraceInformation("Loading Index()...");

            //var auctions = await CacheService.TryGet(CacheKeys.CurrentAuctions, () => _auctionService.GetCurrentAuctions(Language), TimeSpan.FromDays(1));

            var viewModel = new IndexViewModel
            {
                Auctions = (await _auctionService.GetCurrentAuctions(Language)).Select(AuctionViewModel.Create)
            };

            return View(viewModel);
        }

        public ActionResult SignalR()
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

            var routeInfo = new RouteInfo(returnUrl, "/");
            routeInfo.RouteData.Values["language"] = language;
            routeInfo.RouteData.Values.Remove("MS_DirectRouteMatches");
            
            return RedirectToRoute(routeInfo.RouteData.Values);
        }
    }
}