using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Results;
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
using OrangeJetpack.Services.Client.Messaging;
using OrangeJetpack.Services.Models;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly IAuthService _authService;
        private readonly AddressService _addressService;
        private readonly EmailTemplateService _emailTemplateService;
        private readonly IMessageService _messageService;

        public UserController(IControllerServices controller)
        {
            _authService = controller.AuthService;
            _addressService=new AddressService(controller.DataContextFactory);
            _emailTemplateService=new EmailTemplateService(controller.DataContextFactory);
            _messageService = controller.MessageService;
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

        [HttpGet, Authorize, Route("test")]
        public IHttpActionResult Test()
        {
            return Ok("yup");
        }

        [Route("change-password")]
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
