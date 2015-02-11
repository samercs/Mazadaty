using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Account;
using Mzayad.Web.Resources;
using OrangeJetpack.Base.Web;

namespace Mzayad.Web.Controllers
{
    [Authorize, RoutePrefix("{language}/account")]
    public class AccountController : ApplicationController
    {
        public AccountController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        [AllowAnonymous, Route("sign-in")]
        public ActionResult SignIn(string returnUrl, int? shipmentId)
        {
            var viewModel = new SignInViewModel
            {
                ReturnUrl = returnUrl,
                Email = CookieService.Get(CookieKeys.LastSignInEmail)
            };

            return View(viewModel);
        }

        [AllowAnonymous, Route("sign-in")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(SignInViewModel model, string returnUrl, int? shipmentId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await AuthService.SignIn(model.Email, model.Password, model.RememberMe);
            if (!success)
            {
                SetStatusMessage(Global.InvalidUserNameOrPassword, StatusMessageType.Error);
                return View(model);
            }

            await SetNameAndEmailCookies(model.Email);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToLocal(returnUrl);
            }

            return RedirectToAction("MyAccount", "User");
        }

        private async Task SetNameAndEmailCookies(string email)
        {
            var user = await AuthService.GetUserByName(email);
            CookieService.Add(CookieKeys.DisplayName, user.FirstName, DateTime.Today.AddYears(10));
            CookieService.Add(CookieKeys.LastSignInEmail, user.Email, DateTime.Today.AddYears(10));
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
    }
}