using Mzayad.Web.Core.Services;
using System.Web.Mvc;

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
            return Content(LanguageCode);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("change-language")]
        public ActionResult ChangeLanguage()
        {
            return Content("...");
        }
    }
}