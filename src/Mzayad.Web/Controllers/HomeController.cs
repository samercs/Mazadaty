using System;
using Mzayad.Web.Core.Services;
using System.Web.Mvc;
using Mzayad.Web.Core.Configuration;

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

        public ActionResult About()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("change-language")]
        public ActionResult ChangeLanguage(string languageCode, Uri returnUrl)
        {
            CookieService.Add(CookieKeys.LanguageCode, languageCode, DateTime.Today.AddYears(10));

            var routeInfo = new RouteInfo(returnUrl, "/");
            routeInfo.RouteData.Values["language"] = languageCode;

            return RedirectToRoute(routeInfo.RouteData.Values);
        }
    }
}