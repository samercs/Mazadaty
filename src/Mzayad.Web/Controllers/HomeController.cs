using System.Web.Mvc;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.Controllers
{
    public class HomeController : ApplicationController
    {
        public HomeController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}