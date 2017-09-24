using Mazadaty.Web.Controllers;
using Mazadaty.Web.Core.Attributes;
using Mazadaty.Web.Core.Identity;
using Mazadaty.Web.Core.Services;
using System.Web.Mvc;

namespace Mazadaty.Web.Areas.admin.Controllers
{
    [RoleAuthorize(Role.Administrator)]
    public class ReportsController : ApplicationController
    {
        public ReportsController(IAppServices appServices) : base(appServices)
        {
        }

        public ActionResult Index()
        {
            return View();
        }
	}
}
