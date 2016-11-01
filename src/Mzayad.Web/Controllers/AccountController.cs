using Mindscape.Raygun4Net;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services;
using Mzayad.Services.Identity;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Extensions;
using Mzayad.Web.Models.Account;
using Mzayad.Web.Models.Shared;
using Mzayad.Web.Resources;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Base.Core.Security;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Services.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/account")]
    public class AccountController : ApplicationController
    {
        private readonly UserService _userService;
        private readonly AddressService _addressService;
        private readonly SessionLogService _sessionLogService;
        private readonly AvatarService _avatarService;

        public AccountController(IAppServices appServices) : base(appServices)
        {
            _userService = new UserService(DataContextFactory);
            _addressService = new AddressService(DataContextFactory);
            _sessionLogService = new SessionLogService(DataContextFactory);
            _avatarService = new AvatarService(DataContextFactory);
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

            var sessionLog = AuthService.GetSessionLog();
            sessionLog.UserId = user.Id;
            _sessionLogService.Insert(sessionLog);

            return !string.IsNullOrEmpty(returnUrl)
                ? RedirectToLocal(returnUrl)
                : RedirectToAction("Dashboard", "User", new { Language });
        }

        private void SetNameAndEmailCookies(ApplicationUser user, string usernameOrEmail)
        {
            CookieService.Add(CookieKeys.DisplayName, user.FirstName, DateTime.Today.AddYears(10));
            CookieService.Add(CookieKeys.LastSignInEmail, usernameOrEmail, DateTime.Today.AddYears(10));
        }

        [Route("sign-out")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            AuthService.SignOut();
            return RedirectToAction("Index", "Home", new { Language });
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

        public async Task<ActionResult> Register()
        {
            var viewModel = new RegisterViewModel
            {
                PhoneCountryCode = "+965",
                Avatars = await _avatarService.GetAll(),
                Address = new AddressViewModel(new Address
                {
                    CountryCode = "KW"
                }).Hydrate()
            };

            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Avatars = await _avatarService.GetAll();
                return View(model);
            }

            model.PhoneCountryCode = "+" + StringFormatter.StripNonDigits(model.PhoneCountryCode);
            model.PhoneNumber = StringFormatter.StripNonDigits(model.PhoneNumber);

            var avatar = await _avatarService.GetById(model.SelectedAvatar);

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneCountryCode = model.PhoneCountryCode,
                PhoneNumber = model.PhoneNumber,
                ProfileStatus = UserProfileStatus.Private,
                AvatarUrl = avatar.Url,
                Gender = model.Gender,
                Birthdate = model.Birthdate
            };

            var result = await AuthService.CreateUser(user, model.Password);
            if (!result.Succeeded)
            {
                SetStatusMessage(string.Format(Global.RegistrationErrorMessage));
                return View(model);
            }

            var address = await _addressService.SaveAddress(model.Address);
            user.AddressId = address.AddressId;
            await _userService.UpdateUser(user);

            //await SendNewUserWelcomeEmail(user);
            SetNameAndEmailCookies(user, "");

            SetStatusMessage(string.Format(Global.RegistrationWelcomeMessage, user.FirstName, Url.Action("Index", "Home", new { area = "" })));

            return RedirectToAction("Dashboard", "User", new { Language });
        }

        private async Task SendNewUserWelcomeEmail(ApplicationUser user)
        {
            var template = await EmailTemplateService.GetByTemplateType(EmailTemplateType.AccountRegistration, Language);
            if (template != null)
            {
                var email = new Email
                {
                    ToAddress = user.Email,
                    Subject = template.Subject,
                    Message = string.Format(template.Message, user.FirstName)
                };

                try
                {
                    await MessageService.Send(email.WithTemplate());
                }
                catch (Exception ex)
                {
                    new RaygunClient().Send(ex);
                }
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

            var user = await _userService.GetUserByEmail(emailAddress);
            if (user == null)
            {
                template = await EmailTemplateService.GetByTemplateType(EmailTemplateType.NoAccount, Language);
                email.Subject = template.Subject;
                email.Message = string.Format(template.Message, emailAddress);
            }
            else
            {
                template = await EmailTemplateService.GetByTemplateType(EmailTemplateType.PasswordReset, Language);
                email.Subject = template.Subject;
                email.Message = string.Format(template.Message, user.FirstName, GetPasswordResetUrl(user.Email));
            }

            //try
            //{
            await MessageService.Send(email.WithTemplate());
            //}
            //catch (Exception ex)
            //{
            //    new RaygunClient().Send(ex);
            //}
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

            var user = await _userService.GetUserByEmail(tokenParameters.Email);
            if (user == null)
            {
                return Error(Global.ResetPasswordCannotFindUserAccount);
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (await AuthService.IsLocked(user.Id))
            {
                return RedirectToAction("Index", "Home");
            }

            user.PasswordHash = _userService.HashPassword(viewModel.NewPassword);
            await _userService.UpdateUser(user);
            await AuthService.SignIn(user);

            SetStatusMessage(Global.PasswordSuccessfullyChanged);

            return RedirectToAction("Dashboard", "User", new { Language });
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