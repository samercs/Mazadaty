using System;
using Mzayad.Web.Core.Services;
using System.Web;
using System.Web.Mvc;
using Mzayad.Web.Core.Configuration;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("")]
    public class HomeController : ApplicationController
    {
        public HomeController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        public ActionResult Index(string languageCode)
        {
            if (languageCode == null)
            {
                return RedirectToAction("Index", new { LanguageCode });
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
            
            //var uri = new Uri(returnUrl);
            var routeInfo = new RouteInfo(returnUrl, "/");
            routeInfo.RouteData.Values["LanguageCode"] = languageCode;

            return RedirectToRoute(routeInfo.RouteData.Values);
        }
    }
}