using System;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using Mzayad.Models;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Identity;
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

        [AllowAnonymous]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await AuthService.CreateUser(user, model.Password);
            if (!result.Succeeded)
            {
                SetStatusMessage(string.Format(Global.RegistrationErrorMessage));
                return View(model);
            }

            await SetNameAndEmailCookies(user.Email);

            SetStatusMessage(string.Format(Global.RegistrationWelcomeMessage, user.FirstName));

            if (await TryAddUserAsAdmin(user.Email))
            {
                SetStatusMessage(string.Format("Welcome to Mzayad {0}! Your account has been set as a site administrator account, to access admin features you'll need to sign out and back in again.", user.FirstName));
            }

            return RedirectToAction("MyAccount", "User");
        }

        private async Task<bool> TryAddUserAsAdmin(string email)
        {
            var shouldBeAdmins = new[]
            {
                "andy.mehalick@orangejetpack.com", 
                "samer_mail_2006@yahoo.com",
                "badder.alghanim@alawama.com",
                "alghanim@mzayad.com",
                "alsarraf@mzayad.com",
                "alghanim.a@alghanimequipment.com"
            };

            email = email.ToLowerInvariant();

            if (shouldBeAdmins.Contains(email) || email.EndsWith("@mzayad.com"))
            {
                var user = await AuthService.GetUserByName(email);          
                await AuthService.AddUserToRole(user.Id, Role.Administrator.ToString());

                return true;
            }

            return false;
        }
    }
}