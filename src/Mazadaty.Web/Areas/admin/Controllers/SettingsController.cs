using Mazadaty.Models.Enum;
using Mazadaty.Web.Controllers;
using Mazadaty.Web.Core.Attributes;
using Mazadaty.Web.Core.Identity;
using Mazadaty.Web.Core.Services;
using OrangeJetpack.Localization;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mazadaty.Models.Enums;
using OrangeJetpack.Base.Core.Formatting;

namespace Mazadaty.Web.Areas.admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("settings"), RoleAuthorize(Role.Administrator)]
    public class SettingsController : ApplicationController
    {
        public SettingsController(IAppServices appServices) : base(appServices)
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("EmailTemplates");
        }

        [Route("email-templates")]
        public async Task<ActionResult> EmailTemplates()
        {
            var model = await EmailTemplateService.GetAll();
            return View(model);
        }

        [Route("edit-email-template")]
        public async Task<ActionResult> EditEmailTemplate(EmailTemplateType id)
        {
            var template = await EmailTemplateService.GetByTemplateType(id);
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
            var template = await EmailTemplateService.GetByTemplateType(id);
            if (template != null)
            {
                template.Subject = Subject.Serialize();
                foreach (var localizedContent in Message)
                {
                    localizedContent.Value = StringFormatter.StripWordHtml(localizedContent.Value);
                }

                template.Message = Message.Serialize();

                await EmailTemplateService.Save(template);
                SetStatusMessage("Email template has been edited successfully.");
                return RedirectToAction("EmailTemplates");

            }
            return RedirectToAction("EmailTemplates");
        }

        
	}
}
