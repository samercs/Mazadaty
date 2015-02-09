using Mzayad.Web.Core.Services;
using System.Web.Mvc;

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