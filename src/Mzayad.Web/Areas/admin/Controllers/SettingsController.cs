using Mzayad.Models.Enum;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Localization;
using System.Threading.Tasks;
using System.Web.Mvc;
using OrangeJetpack.Base.Core.Formatting;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("settings")]
    public class SettingsController : ApplicationController
    {
        public SettingsController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("EmailTemplates");
        }

        [Route("email-templates")]
        public async Task<ActionResult> EmailTemplates()
        {
            var model = await _EmailTemplateService.GetAll();
            return View(model);
        }

        [Route("edit-email-template")]
        public async Task<ActionResult> EditEmailTemplate(EmailTemplateType id)
        {
            var template = await _EmailTemplateService.GetByTemplateType(id);
            if (template != null)
            {
                return View(template);
            }
            return RedirectToAction("EmailTemplates");

        }

        [Route("edit-email-template")]
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