using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Mindscape.Raygun4Net;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services;
using Mzayad.Web.Areas.Api.Models;
using Mzayad.Web.Areas.Api.Models.User;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Extensions;
using Mzayad.Web.Models.Account;
using Mzayad.Web.Models.User;
using System.Threading.Tasks;
using System.Web.Http;
using Mzayad.Web.Resources;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Base.Core.Security;
using OrangeJetpack.Localization;
using OrangeJetpack.Services.Client.Messaging;
using OrangeJetpack.Services.Models;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [System.Web.Http.RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly IAuthService _authService;
        private readonly AddressService _addressService;
        private readonly EmailTemplateService _emailTemplateService;
        private readonly IMessageService _messageService;
        private readonly  IAppSettings AppSettings;

        public UserController(IControllerServices controller)
        {
            _authService = controller.AuthService;
            _addressService=new AddressService(controller.DataContextFactory);
            _emailTemplateService=new EmailTemplateService(controller.DataContextFactory);
            _messageService = controller.MessageService;
            AppSettings = controller.AppSettings;
        }

        public async Task<IHttpActionResult> Get(string id)
        {
            var curentUser = await _authService.GetUserById(id);
            var user = new UserGetModel()
            {
                Name = NameFormatter.GetFullName(curentUser.FirstName,curentUser.LastName),
                Email = curentUser.Email,
                CreatedDate=curentUser.CreatedUtc,
                PhoneNumber = curentUser.PhoneCountryCode + " " + curentUser.PhoneNumber,
                UserName=curentUser.UserName
            };

            return Ok<UserGetModel>(user);

        }

        
        public async Task<IHttpActionResult> Post(RegisterViewModel model)
        {
            ModelState.Remove("model.Address.CreatedUtc");
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                return BadRequest("Invalid user information : " + messages);
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

            var result = await _authService.CreateUser(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest("can't create this user");
            }

            var address = await _addressService.SaveAddress(model.Address);
            user.AddressId = address.AddressId;
            await _authService.UpdateUser(user);

            await SendNewUserWelcomeEmail(user);
            return Ok(string.Format("user created sucessfully"));

            


        }


        [System.Web.Http.HttpPost, System.Web.Http.Route("action/password-reset")]

        public async Task<IHttpActionResult> PasswordReset(NeedPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                
                return BadRequest(messages);
            }

            await SendPasswordResetNotification(model.Email);
            return Ok("Password has been send");
        }


        public async Task<IHttpActionResult> Put(string id, UserAccountViewModel model)
        {
            ModelState.Remove("model.Address.CreatedUtc");
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                return BadRequest(messages);
            }

            var user = await _authService.GetUserById(id);
            var originalEmail = user.Email;
            bool emailChanged = false;

            user.FirstName = string.IsNullOrEmpty(model.FirstName) ? user.FirstName : model.FirstName;
            user.LastName = string.IsNullOrEmpty(model.LastName) ? user.LastName : model.LastName;
            if (!string.IsNullOrEmpty(model.Email))
            {
                user.Email = model.Email;
                emailChanged = originalEmail != model.Email;
            }
            user.PhoneCountryCode = string.IsNullOrEmpty(model.PhoneCountryCode) ? user.PhoneCountryCode : model.PhoneCountryCode;
            user.PhoneNumber = string.IsNullOrEmpty(model.PhoneNumber) ? user.PhoneNumber : model.PhoneNumber;
            await _authService.UpdateUser(user);

            if (user.AddressId.HasValue && model.Address!=null)
            {
                var address = await _addressService.GetAddress(user.AddressId.Value);
                address.AddressLine1 = string.IsNullOrEmpty(model.Address.AddressLine1) ? address.AddressLine1 : model.Address.AddressLine1;
                address.AddressLine2 = string.IsNullOrEmpty(model.Address.AddressLine2) ? address.AddressLine2 :  model.Address.AddressLine2;
                address.AddressLine3 = string.IsNullOrEmpty(model.Address.AddressLine3) ? address.AddressLine3 : model.Address.AddressLine3;
                address.AddressLine4 = string.IsNullOrEmpty(model.Address.AddressLine4) ? address.AddressLine4 : model.Address.AddressLine4;
                address.CityArea = string.IsNullOrEmpty(model.Address.CityArea) ? address.CityArea : model.Address.CityArea;
                address.CountryCode = string.IsNullOrEmpty(model.Address.CountryCode) ? address.CountryCode : model.Address.CountryCode;
                address.PostalCode = string.IsNullOrEmpty(model.Address.PostalCode) ? address.PostalCode : model.Address.PostalCode;
                address.StateProvince = string.IsNullOrEmpty(model.Address.StateProvince) ? address.StateProvince : model.Address.StateProvince;
                await _addressService.Update(address);
            }
            if (emailChanged)
            {
                await SendEmailChangedEmail(user, originalEmail);
            }

            return Ok("User updated successfully.");

            
        }

        private async Task SendEmailChangedEmail(ApplicationUser user, string originalEmail)
        {
            var emailTemplate = await _emailTemplateService.GetByTemplateType(EmailTemplateType.EmailChanged);
            var email = new Email
            {
                ToAddress = originalEmail,
                Subject = emailTemplate.Localize("en", i => i.Subject).Subject,
                Message = string.Format(emailTemplate.Localize("en", i => i.Message).Message, user.FirstName, AppSettings.SiteName)
            };

            await _messageService.SendMessage(email.WithTemplate(this));
        }

        private async Task SendNewUserWelcomeEmail(ApplicationUser user)
        {
            var template = await _emailTemplateService.GetByTemplateType(EmailTemplateType.AccountRegistration,"en");
            var email = new Email
            {
                ToAddress = user.Email,
                Subject = template.Subject,
                Message = string.Format(template.Message, user.FirstName)
            };

            try
            {
                await _messageService.SendMessage(email.WithTemplate(this));
            }
            catch (Exception ex)
            {
                new RaygunClient().Send(ex);
            }
        }

        private async Task SendPasswordResetNotification(string emailAddress)
        {
            EmailTemplate template;
            var email = new Email
            {
                ToAddress = emailAddress,
                Subject = Global.ResetPassword
            };

            var user = await _authService.GetUserByEmail(emailAddress);
            if (user == null)
            {
                template = await _emailTemplateService.GetByTemplateType(EmailTemplateType.NoAccount, "en");
                email.Subject = template.Subject;
                email.Message = string.Format(template.Message, emailAddress);
            }
            else
            {
                template = await _emailTemplateService.GetByTemplateType(EmailTemplateType.PasswordReset, "en");
                email.Subject = template.Subject;
                email.Message = string.Format(template.Message, user.FirstName, GetPasswordResetUrl(user.Email));
            }

            try
            {
                await _messageService.SendMessage(email.WithTemplate(this));
            }
            catch (Exception ex)
            {
                new RaygunClient().Send(ex);
            }
        }

        private string GetBaseUrl(string action)
        {
            Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, String.Empty));

            string resourceRelative = "~/en/account/" + action;
            Uri resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(resourceRelative));

            return resourceFullPath.AbsoluteUri;
        }

        private string GetPasswordResetUrl(string email)
        {
            var baseUrl = GetBaseUrl("resetpassword");

            return PasswordUtilities.GenerateResetPasswordUrl(baseUrl, email);
        }

        [System.Web.Http.HttpGet, System.Web.Http.Authorize, System.Web.Http.Route("test")]
        public IHttpActionResult Test()
        {
            return Ok("yup");
        }

        [System.Web.Http.Route("change-password")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ChangePassword(User.Identity, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (result.Succeeded)
            {
                return null;
            }
            
            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            if (ModelState.IsValid)
            {
                // No ModelState errors are available to send, so just return an empty BadRequest.
                return BadRequest();
            }

            return BadRequest(ModelState);
        }
    }
}
