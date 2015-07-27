using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    [Authorize]
    [RoutePrefix("queue")]
    public class QueueTestController : Controller
    {
        [Route("test")]
        public ActionResult Test()
        {
            return View();
        }

        [Route("test")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Test(FormCollection formCollection)
        {
            

            return View();
        }
    }
}