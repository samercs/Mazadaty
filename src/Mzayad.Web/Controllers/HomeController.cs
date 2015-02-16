using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using System;
using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}")]
    public class HomeController : ApplicationController
    {
        public HomeController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        public ActionResult Index(string language)
        {
            if (language == null)
            {
                return RedirectToAction("Index", new { Language });
            }

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
            //routeInfo.RouteData.Values.Add("language", language);
            routeInfo.RouteData.Values.Remove("MS_DirectRouteMatches");
            
            return RedirectToRoute(routeInfo.RouteData.Values);
        }
    }
}