using Mzayad.Web.Core.Services;
using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/user")]
    public class UserController : ApplicationController
    {
        public UserController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        [Route("my-account")]
        public ActionResult MyAccount()
        {
            return View();
        }
    }
}