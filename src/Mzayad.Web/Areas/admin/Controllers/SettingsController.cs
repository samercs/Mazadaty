using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Models.Enum;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Formatting;
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

        public async Task<ActionResult> EditEmailTemplate(EmailTemplateType id)
        {
            var template = await _EmailTemplateService.GetByTemplateType(id);
            if (template != null)
            {
                return View(template);
            }
            return RedirectToAction("EmailTemplates");

        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public async Task<ActionResult> EditEmailTemplate(EmailTemplateType id, LocalizedContent[] Subject, LocalizedContent[] Message)
        {
            var template = await _EmailTemplateService.GetByTemplateType(id);
            if (template != null)
            {
                template.Subject = Subject.Serialize();
                foreach (var localizedContent in Message)
                {
                    localizedContent.Value = StringFormatter.StripWordHtml(localizedContent.Value);
                }

                template.Message = Message.Serialize();

                await _EmailTemplateService.Save(template);
                SetStatusMessage("Email template has been edited successfully.");
                return RedirectToAction("EmailTemplates");

            }
            return RedirectToAction("EmailTemplates");
        }

        
	}
}