using Microsoft.AspNet.Identity;
using Mindscape.Raygun4Net;
using Mzayad.Core.Exceptions;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Services.Activity;
using Mzayad.Services.Identity;
using Mzayad.Web.Areas.Api.Models.User;
using Mzayad.Web.Areas.Api.Models.Users;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Extensions;
using Mzayad.Web.Models.Account;
using Mzayad.Web.Models.Shared;
using Mzayad.Web.Models.User;
using Mzayad.Web.Resources;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Base.Core.Security;
using OrangeJetpack.Localization;
using OrangeJetpack.Services.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebGrease.Css.Extensions;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApplicationApiController
    {
        private readonly UserService _userService;
        private readonly AddressService _addressService;
        private readonly SessionLogService _sessionLogService;
        private readonly AvatarService _avatarService;
        private readonly IActivityQueueService _activityQueueService;

        public UsersController(IAppServices appServices) : base(appServices)
        {
            _userService = new UserService(DataContextFactory);
            _addressService = new AddressService(DataContextFactory);
            _sessionLogService = new SessionLogService(DataContextFactory);
            _avatarService = new AvatarService(DataContextFactory);
            _activityQueueService =
                new ActivityQueueService(ConfigurationManager.ConnectionStrings["QueueConnection"].ConnectionString);
        }

        [HttpGet, Route("{username}")]
        public async Task<IHttpActionResult> Get(string username)
        {
            var applicationUser = await _userService.GetUserByName(username);
            if (applicationUser == null)
            {
                return NotFound();
            }

            var user = new UserModel
            {
                UserId = applicationUser.Id,
                FullName = NameFormatter.GetFullName(applicationUser.FirstName, applicationUser.LastName),
                UserName = applicationUser.UserName,
                Email = applicationUser.Email,
                AvatarUrl = applicationUser.AvatarUrl
            };

            return Ok(user);
        }

        [HttpGet, Route("action/validate-username")]
        public async Task<IHttpActionResult> ValidateUserName(string username)
        {
            var exists = await AuthService.UserExists(username);
            var results = new
            {
                IsValid = !exists
            };
            return Ok(results);
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
                Birthdate = model.Birthdate,
                Level = 1
            };

            var result = await AuthService.CreateUser(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest("can't create this user");
            }

            var address = await _addressService.SaveAddress(model.Address);
            user.AddressId = address.AddressId;
            await _userService.UpdateUser(user);

            await SendNewUserWelcomeEmail(user);
            return Ok();
        }

        [HttpPost, Route("action/password-reset")]
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

        [Route("current")]
        [Authorize]
        public async Task<IHttpActionResult> Get()
        {
            var user = await AuthService.CurrentUser();
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var address = await _addressService.GetAddress(user.AddressId);
            var model = new UserAccountViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneCountryCode = user.PhoneCountryCode,
                PhoneNumber = user.PhoneNumber,
                Address = new AddressViewModel
                {
                    AddressId = address.AddressId,
                    AddressLine1 = address.AddressLine1,
                    AddressLine2 = address.AddressLine2,
                    AddressLine3 = address.AddressLine3,
                    AddressLine4 = address.AddressLine4,
                    CityArea = address.CityArea,
                    CountryCode = address.CountryCode,
                    PostalCode = address.PostalCode,
                    StateProvince = address.StateProvince,
                    Floor = address.Floor,
                    FlatNumber = address.FlatNumber
                }
            };

            return Ok(model);
        }

        [Route("current")]
        [Authorize]
        public async Task<IHttpActionResult> Put(UserAccountViewModel model)
        {
            ModelState.Remove("model.Address");

            if (!ModelState.IsValid)
            {
                return ModelStateError(ModelState);
            }

            var user = await AuthService.CurrentUser();
            var originalEmail = user.Email;
            var emailChanged = false;

            user.FirstName = string.IsNullOrEmpty(model.FirstName) ? user.FirstName : model.FirstName;
            user.LastName = string.IsNullOrEmpty(model.LastName) ? user.LastName : model.LastName;
            if (!string.IsNullOrEmpty(model.Email))
            {
                user.Email = model.Email;
                emailChanged = originalEmail != model.Email;
            }
            user.PhoneCountryCode = string.IsNullOrEmpty(model.PhoneCountryCode)
                ? user.PhoneCountryCode
                : model.PhoneCountryCode;
            user.PhoneNumber = string.IsNullOrEmpty(model.PhoneNumber) ? user.PhoneNumber : model.PhoneNumber;
            await _userService.UpdateUser(user);

            if (user.AddressId.HasValue && model.Address != null)
            {
                var address = await _addressService.GetAddress(user.AddressId.Value);
                address.AddressLine1 = string.IsNullOrEmpty(model.Address.AddressLine1)
                    ? address.AddressLine1
                    : model.Address.AddressLine1;
                address.AddressLine2 = string.IsNullOrEmpty(model.Address.AddressLine2)
                    ? address.AddressLine2
                    : model.Address.AddressLine2;
                address.AddressLine3 = string.IsNullOrEmpty(model.Address.AddressLine3)
                    ? address.AddressLine3
                    : model.Address.AddressLine3;
                address.AddressLine4 = string.IsNullOrEmpty(model.Address.AddressLine4)
                    ? address.AddressLine4
                    : model.Address.AddressLine4;
                address.CityArea = string.IsNullOrEmpty(model.Address.CityArea)
                    ? address.CityArea
                    : model.Address.CityArea;
                address.CountryCode = string.IsNullOrEmpty(model.Address.CountryCode)
                    ? address.CountryCode
                    : model.Address.CountryCode;
                address.PostalCode = string.IsNullOrEmpty(model.Address.PostalCode)
                    ? address.PostalCode
                    : model.Address.PostalCode;
                address.StateProvince = string.IsNullOrEmpty(model.Address.StateProvince)
                    ? address.StateProvince
                    : model.Address.StateProvince;
                address.Floor = string.IsNullOrEmpty(model.Address.Floor)
                     ? address.Floor
                     : model.Address.Floor;
                address.FlatNumber = string.IsNullOrEmpty(model.Address.FlatNumber)
                     ? address.FlatNumber
                     : model.Address.FlatNumber;
                await _addressService.Update(address);
            }
            if (emailChanged)
            {
                await SendEmailChangedEmail(user, originalEmail);
            }

            return Ok();
        }

        [Route("current/profile")]
        [HttpGet, Authorize]
        public async Task<IHttpActionResult> GetProfile()
        {
            var user = await AuthService.CurrentUser();
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var profileUrl = $"https://www.zeedli.com/profiles/{user.UserName.ToLowerInvariant()}";
            var avatars = (await GetAvatars(user)).Select(i => new
            {
                i.AvatarId,
                i.Url,
                i.IsPremium,
                Selected=i.Url.Equals(user.AvatarUrl)
            });

            return Ok(new
            {
                user.ProfileStatus,
                profileUrl,
                user.Gender,
                user.Birthdate,
                user.AvatarUrl,
                avatars
            });
        }

        [Route("current/profile")]
        [HttpPut, Authorize]
        public async Task<IHttpActionResult> UpdateProfile(UserProfileModel model)
        {
            var user = await AuthService.CurrentUser();
            var hostAddress = AuthService.UserHostAddress();

            user.ProfileStatus = model.ProfileStatus;
            user.Gender = model.Gender;
            user.Birthdate = model.Birthdate;

            var avatar = model.AvatarId.HasValue ? await _avatarService.GetById(model.AvatarId.Value) : new Avatar();
            var avatarChanged = model.AvatarId.HasValue && !user.AvatarUrl.Equals(avatar.Url);
            if (avatarChanged)
            {
                var userOwnAvatar = await _avatarService.UserHasAvatar(user, avatar);
                if (avatar.IsPremium && !userOwnAvatar)
                {
                    try
                    {
                        await _avatarService.BuyAvatar(user, avatar, hostAddress);
                    }
                    catch (InsufficientTokensException)
                    {
                        return InsufficientTokensError();
                    }
                }

                await _avatarService.ChangeAvatar(user, avatar, hostAddress);
                user.AvatarUrl = avatar.Url;
            }

            await _userService.UpdateUser(user);
            await _activityQueueService.QueueActivityAsync(ActivityType.CompleteProfile, user.Id);
            return Ok();
        }

        private async Task SendEmailChangedEmail(ApplicationUser user, string originalEmail)
        {
            var emailTemplate = await EmailTemplateService.GetByTemplateType(EmailTemplateType.EmailChanged);
            var email = new Email
            {
                ToAddress = originalEmail,
                Subject = emailTemplate.Localize("en", i => i.Subject).Subject,
                Message = string.Format(emailTemplate.Localize("en", i => i.Message).Message, user.FirstName, AppSettings.SiteName)
            };

            await MessageService.Send(email.WithTemplate());
        }

        private async Task SendNewUserWelcomeEmail(ApplicationUser user)
        {
            var template = await EmailTemplateService.GetByTemplateType(EmailTemplateType.AccountRegistration, "en");
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
                template = await EmailTemplateService.GetByTemplateType(EmailTemplateType.NoAccount, "en");
                email.Subject = template.Subject;
                email.Message = string.Format(template.Message, emailAddress);
            }
            else
            {
                template = await EmailTemplateService.GetByTemplateType(EmailTemplateType.PasswordReset, "en");
                email.Subject = template.Subject;
                email.Message = string.Format(template.Message, user.FirstName, GetPasswordResetUrl(user.Email));
            }

            try
            {
                await MessageService.Send(email.WithTemplate());
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

        private async Task<IReadOnlyCollection<Avatar>> GetAvatars(ApplicationUser user)
        {
            var allAvatars = await _avatarService.GetAll();
            var userAvatars = (await _avatarService.GetUserAvatars(user))
                .OrderByDescending(i => i.IsPremium);
            var unOwnedAvatars = allAvatars.Where(i => !userAvatars.Select(j => j.AvatarId).Contains(i.AvatarId))
            .OrderByDescending(i => i.IsPremium);

            return userAvatars.Concat(unOwnedAvatars).ToSafeReadOnlyCollection();
        }

        [Route("current/password")]
        [HttpPut, Authorize]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ModelStateError(ModelState);
            }

            var user = await AuthService.CurrentUser();
            if (user == null)
            {
                return BadRequest("user not found");
            }
            var result = await _userService.ChangePassword(user.Id, model.CurrentPassword, model.NewPassword);
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
                return BadRequest();
            }

            return BadRequest(ModelState);
        }

        [HttpPost, Route("session/log")]
        public void LogSession()
        {
            _sessionLogService.Insert(AuthService.GetSessionLog());
        }
    }
}
