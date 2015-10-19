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
        private readonly IActivityQueueService _activityQueueService;

        public QueueTestController(IAuthService authService, IActivityQueueService activityQueueService)
        {
            _authService = authService;
            _activityQueueService = activityQueueService;
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
            _activityQueueService.QueueActivity(ActivityType.TestActivity, _authService.CurrentUserId());

            return View();
        }

        [Route("test")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TestEarnXp(FormCollection formCollection)
        {
            _activityQueueService.QueueActivity(ActivityType.TestActivity, _authService.CurrentUserId(), 5);

            return View();
        }
    }
}