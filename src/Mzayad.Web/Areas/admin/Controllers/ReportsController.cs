using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.admin.Controllers
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