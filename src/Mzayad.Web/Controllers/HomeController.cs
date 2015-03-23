using System.Threading.Tasks;
using Mzayad.Services;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using System;
using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}")]
    public class HomeController : ApplicationController
    {
        private readonly AuctionService _auctionService;
        
        public HomeController(IControllerServices controllerServices) : base(controllerServices)
        {
            _auctionService = new AuctionService(controllerServices.DataContextFactory);
        }

        public async Task<ActionResult> Index(string language)
        {
            if (language == null)
            {
                return RedirectToAction("Index", new { Language });
            }

            var auctions = await _auctionService.GetCurrentAuctions();

            return View(auctions);
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

        public ActionResult Ouch()
        {
            throw new Exception("Ouch");
        }
    }
}