using System.Web.Mvc;
using Mzayad.Models.Enums;
using Mzayad.Services.Activity;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.Controllers
{
    [Authorize]
    [RoutePrefix("queue")]
    public class QueueTestController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IActivityService _activityService;

        public QueueTestController(IAuthService authService, IActivityService activityService)
        {
            _authService = authService;
            _activityService = activityService;
        }

        [Route("test")]
        public ActionResult Test()
        {
            return View();
        }

        [Route("test")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Test(FormCollection formCollection)
        {
            _activityService.AddActivity(ActivityType.TestActivity, _authService.CurrentUserId());

            return View();
        }
    }
}