using System;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Extensions;
using Mzayad.Web.Models.Account;
using Mzayad.Web.Models.Shared;
using Mzayad.Web.Resources;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Services.Models;
using OrangeJetpack.Base.Core.Security;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/account")]
    public class AccountController : ApplicationController
    {
        private readonly AddressService _addressService;
        
        public AccountController(IControllerServices controllerServices) : base(controllerServices)
        {
            _addressService = new AddressService(controllerServices.DataContextFactory);
        }

        [Route("sign-in")]
        public ActionResult SignIn(string returnUrl, int? shipmentId)
        {
            var viewModel = new SignInViewModel
            {
                ReturnUrl = returnUrl,
                UsernameOrEmail = CookieService.Get(CookieKeys.LastSignInEmail)
            };

            return View(viewModel);
        }

        [Route("sign-in")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(SignInViewModel model, string returnUrl, int? shipmentId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await AuthService.SignIn(model.UsernameOrEmail, model.Password, model.RememberMe);
            if (user == null)
            {
                SetStatusMessage(Global.InvalidUserNameOrPassword, StatusMessageType.Error);
                return View(model);
            }

            SetNameAndEmailCookies(user, model.UsernameOrEmail);

            return !string.IsNullOrEmpty(returnUrl) 
                ? RedirectToLocal(returnUrl) 
                : RedirectToAction("MyAccount", "User", new { Language });
        }

        private void SetNameAndEmailCookies(ApplicationUser user, string usernameOrEmail)
        {
            CookieService.Add(CookieKeys.DisplayName, user.FirstName, DateTime.Today.AddYears(10));
            CookieService.Add(CookieKeys.LastSignInEmail, usernameOrEmail, DateTime.Today.AddYears(10));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            AuthService.SignOut();
            return RedirectToAction("Index", "Home", new { Language });
        }

        public ActionResult Register()
        {
            var viewModel = new RegisterViewModel
            {
                PhoneCountryCode = "+965",
                Address = new AddressViewModel(TryGetGeolocatedAddress()).Hydrate()
            };

            return View(viewModel);
        }

        private Address TryGetGeolocatedAddress()
        {
            var address = new Address { CountryCode = "KW" };
            if (AuthService.IsLocal())
            {
                return address;
            }

            try
            {
                var country = GeolocationService.GetCountry(AuthService.UserHostAddress());
                address.CountryCode = country.IsoCode;
            }
            catch (Exception ex)
            {
                //_errorLogger.Log(ex);
            }
            return address;
        }

        public PartialViewResult ChangeCountry(string countryCode)
        {
            var viewName = AddressPartialResolver.GetViewName(countryCode);
            var viewModel = new AddressViewModel();

            ViewData.TemplateInfo = new TemplateInfo
            {
                HtmlFieldPrefix = "Address"
            };

            return PartialView(viewName, viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.PhoneCountryCode = "+" + StringFormatter.StripNonDigits(model.PhoneCountryCode);
            model.PhoneNumber = StringFormatter.StripNonDigits(model.PhoneNumber);

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneCountryCode = model.PhoneCountryCode,
                PhoneNumber = model.PhoneNumber
            };

            var result = await AuthService.CreateUser(user, model.Password);
            if (!result.Succeeded)
            {
                SetStatusMessage(string.Format(Global.RegistrationErrorMessage));
                return View(model);
            }

            var address = await _addressService.SaveAddress(model.Address);
            user.AddressId = address.AddressId;
            await AuthService.UpdateUser(user);

            await SendNewUserWelcomeEmail(user);
            SetNameAndEmailCookies(user, "");

            SetStatusMessage(string.Format(Global.RegistrationWelcomeMessage, user.FirstName));

            if (await TryAddUserAsAdmin(user.Email))
            {
                SetStatusMessage(string.Format("Welcome to Mzayad {0}! Your account has been set as a site administrator account, to access admin features you'll need to sign out and back in again.", user.FirstName));
            }

            return RedirectToAction("MyAccount", "User", new { Language });
        }

        private async Task<bool> TryAddUserAsAdmin(string email)
        {
            var shouldBeAdmins = new[]
            {
                "andy.mehalick@orangejetpack.com", 
                "samer_mail_2006@yahoo.com",
                "badder.alghanim@alawama.com",
                "alghanim.a@alghanimequipment.com"
            };

            email = email.ToLowerInvariant();

            if (shouldBeAdmins.Contains(email) || email.EndsWith("@mzayad.com"))
            {
                var user = await AuthService.GetUserByEmail(email);          
                await AuthService.AddUserToRole(user.Id, Role.Administrator.ToString());

                return true;
            }

            return false;
        }

        public async Task<ActionResult> Test()
        {
            await SendNewUserWelcomeEmail(await AuthService.CurrentUser());

            return Content("..");
        }

        private async Task SendNewUserWelcomeEmail(ApplicationUser user)
        {
            var template = await _EmailTemplateService.GetByTemplateType(EmailTemplateType.AccountRegistration, Language);
            var email = new Email
            {
                ToAddress = user.Email,
                Subject = template.Subject,
                Message = string.Format(template.Message, user.FirstName)
            };

            try
            {
                await EmailService.SendMessage(email.WithTemplate(this));
            }
            catch (Exception ex)
            {
                // TODO added exception logging, for example Raygun or ELMAH
            }
        }

        public async Task<JsonResult> ValidateUserName(string username)
        {
            var exists = await AuthService.UserExists(username);
            var results = new
            {
                IsValid = !exists
            };

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [Route("need-password")]
        public ActionResult NeedPassword()
        {
            var viewModel = new NeedPasswordViewModel();

            return View(viewModel);
        }

        [Route("need-password")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> NeedPassword(NeedPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(new NeedPasswordViewModel());
            }

            await SendPasswordResetNotification(viewModel.Email);

            SetStatusMessage(string.Format(Global.ResetPasswordEmailSentAcknowledgement, viewModel.Email));

            return RedirectToAction("SignIn", new { Language });
        }

        private async Task SendPasswordResetNotification(string emailAddress)
        {
            EmailTemplate template;
            var email = new Email
            {
                ToAddress = emailAddress,
                Subject = Global.ResetPassword
            };

            var user = await AuthService.GetUserByEmail(emailAddress);
            if (user == null)
            {
                template = await _EmailTemplateService.GetByTemplateType(EmailTemplateType.NoAccount, Language);
                email.Subject = template.Subject;
                email.Message = string.Format(template.Message, emailAddress);
            }
            else
            {
                template = await _EmailTemplateService.GetByTemplateType(EmailTemplateType.PasswordReset, Language);
                email.Subject = template.Subject;
                email.Message = string.Format(template.Message, user.FirstName, GetPasswordResetUrl(user.Email));
            }

            try
            {
                await MessageService.SendMessage(email.WithTemplate(this));
            }
            catch (Exception ex)
            {
                // TODO log exception
                throw ex;
            }
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(UrlTokenParameters tokenParameters)
        {
            string errorMessage;
            if (!ValidateTokenParameters(tokenParameters, out errorMessage))
            {
                return StatusMessage(errorMessage, StatusMessageType.Error);
            }

            return View(new ResetPasswordViewModel());
        }

        [AllowAnonymous]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(UrlTokenParameters tokenParameters, ResetPasswordViewModel viewModel)
        {
            string errorMessage;
            if (!ValidateTokenParameters(tokenParameters, out errorMessage))
            {
                return Error(errorMessage);
            }

            var user = await AuthService.GetUserByEmail(tokenParameters.Email);
            if (user == null)
            {
                return Error(Global.ResetPasswordCannotFindUserAccount);
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            user.PasswordHash = AuthService.HashPassword(viewModel.NewPassword);
            await AuthService.UpdateUser(user);
            await AuthService.SignIn(user);

            SetStatusMessage(Global.PasswordSuccessfullyChanged);

            return RedirectToAction("MyAccount", "User", new { Language });
        }

        private string GetRegistrationUrl()
        {
            return GetBaseUrl("Register");
        }

        private string GetBaseUrl(string action)
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            return urlHelper.Action(action, "Account", null, "https");
        }

        private string GetPasswordResetUrl(string email)
        {
            var baseUrl = GetBaseUrl("ResetPassword");

            return PasswordUtilities.GenerateResetPasswordUrl(baseUrl, email);
        }

        private static bool ValidateTokenParameters(UrlTokenParameters urlTokenParameters, out string errorMessage)
        {
            try
            {
                PasswordUtilities.ValidateResetPasswordParameters(urlTokenParameters);
            }
            catch (MissingParametersException)
            {
                errorMessage = Global.ResetPasswordMissingParametersException;
                return false;
            }
            catch (ExpiredTimestampException)
            {
                errorMessage = Global.ResetPasswordExpiredTimestampException;
                return false;
            }
            catch (InvalidTokenException)
            {
                errorMessage = Global.ResetPasswordInvalidTokenException;
                return false;
            }
            errorMessage = null;
            return true;
        }

        
    }
}