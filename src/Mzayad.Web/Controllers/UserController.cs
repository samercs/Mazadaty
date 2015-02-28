using System.Threading.Tasks;
using Mzayad.Web.Core.Services;
using System.Web.Mvc;
using Mzayad.Models.Enum;
using Mzayad.Web.Extensions;
using Mzayad.Web.Models.User;
using Mzayad.Web.Resources;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Localization;
using OrangeJetpack.Services.Models;

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

        [Route("change-password")]
        public ActionResult ChangePassword()
        {
            var viewModel = new ChangePasswordViewModel();

            return View(viewModel);
        }

        [Route("change-password")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await AuthService.ChangePassword(User.Identity, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                SetStatusMessage(Global.PasswordChangeFailureMessage, StatusMessageType.Error);

                return View(model);
            }

            await SendPasswordChangedEmail();

            SetStatusMessage(Global.PasswordSuccessfullyChanged);

            return RedirectToAction("MyAccount");
        }

        private async Task SendPasswordChangedEmail()
        {
            var user = await AuthService.CurrentUser();
            var emailTeamplet = await _EmailTemplateService.GetByTemplateType(EmailTemplateType.PasswordChanged);
            var email = new Email
            {
                ToAddress = user.Email,
                Subject = emailTeamplet.Localize(Language, i => i.Subject).Subject,
                Message = string.Format(emailTeamplet.Localize(Language, i => i.Message).Message, user.FirstName)
            };

            await MessageService.SendMessage(email.WithTemplate(this));
        }
    }
}