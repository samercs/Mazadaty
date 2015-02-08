using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    public class HomeController : ApplicationController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}