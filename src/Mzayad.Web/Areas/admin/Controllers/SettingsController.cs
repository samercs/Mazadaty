using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Models.Enum;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Localization;

namespace Mzayad.Web.Areas.admin.Controllers
{
    public class SettingsController : ApplicationController
    {
        //
        // GET: /admin/Settings/
        public SettingsController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("EmailTemplates");
        }

        public async Task<ActionResult> EmailTemplates()
        {
            var model = await _EmailTemplateService.GetAll();
            return View(model);
        }

        
	}
}