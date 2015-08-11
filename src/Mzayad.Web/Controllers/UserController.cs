﻿using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Extensions;
using Mzayad.Web.Models.Account;
using Mzayad.Web.Models.Shared;
using Mzayad.Web.Models.User;
using Mzayad.Web.Resources;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Localization;
using OrangeJetpack.Services.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Mzayad.Services.Identity;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/user"), Authorize]
    public class UserController : ApplicationController
    {
        private readonly UserService _userService;
        private readonly AddressService _addressService;
        private readonly CategoryService _categoryService;
        private readonly NotificationService _notificationService;
        private readonly UserProfileService _userProfileService;
        private readonly AvatarService _avatarService;
        private readonly BidService _bidService;
        
        public UserController(IAppServices appServices)
            : base(appServices)
        {
            _userService = new UserService(DataContextFactory);
            _addressService = new AddressService(DataContextFactory);
            _categoryService = new CategoryService(DataContextFactory);
            _notificationService = new NotificationService(DataContextFactory);
            _userProfileService = new UserProfileService(appServices.DataContextFactory);
            _avatarService = new AvatarService(appServices.DataContextFactory);
            _bidService = new BidService(DataContextFactory);
        }

        [Route("dashboard")]
        public async Task<ActionResult> Dashboard()
        {
            var user = await AuthService.CurrentUser();

            var viewModel = new DashboardViewModel
            {
                ApplicationUser = user,
                UserProfile = await _userProfileService.GetByUser(user),
                BidHistory = await _bidService.GetRecentBidHistoryForUser(user, Language)
            };

            return View(viewModel);
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

            var result = await _userService.ChangePassword(User.Identity.GetUserId(), model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                SetStatusMessage(Global.PasswordChangeFailureMessage, StatusMessageType.Error);

                return View(model);
            }

            await SendPasswordChangedEmail();

            SetStatusMessage(Global.PasswordSuccessfullyChanged);

            return RedirectToAction("Dashboard");
        }

        private async Task SendPasswordChangedEmail()
        {
            var user = await AuthService.CurrentUser();
            var emailTeamplet = await EmailTemplateService.GetByTemplateType(EmailTemplateType.PasswordChanged);
            var email = new Email
            {
                ToAddress = user.Email,
                Subject = emailTeamplet.Localize(Language, i => i.Subject).Subject,
                Message = string.Format(emailTeamplet.Localize(Language, i => i.Message).Message, user.FirstName)
            };

            await MessageService.Send(email.WithTemplate());
        }

        [Route("edit-account")]
        public async Task<ActionResult> EditAccount()
        {
            var user = await AuthService.CurrentUser();
            Address address = null;
            if (user.AddressId.HasValue)
            {
                address = await _addressService.GetAddress(user.AddressId.Value);
            }

            var model = new UserAccountViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = new AddressViewModel(address).Hydrate(),
                PhoneCountryCode = user.PhoneCountryCode,
                PhoneNumber = user.PhoneNumber
            };
            return View(model);
        }

        [Route("edit-account")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAccount(UserAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Address.Hydrate();

                return View(model);
            }

            var user = await AuthService.CurrentUser();
            var originalEmail = user.Email;
            var emailChanged = originalEmail != model.Email;

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneCountryCode = model.PhoneCountryCode;
            user.PhoneNumber = model.PhoneNumber;
            await _userService.UpdateUser(user);

            if (user.AddressId.HasValue)
            {
                var address = await _addressService.GetAddress(user.AddressId.Value);
                address.AddressLine1 = model.Address.AddressLine1;
                address.AddressLine2 = model.Address.AddressLine2;
                address.AddressLine3 = model.Address.AddressLine3;
                address.AddressLine4 = model.Address.AddressLine4;
                address.CityArea = model.Address.CityArea;
                address.CountryCode = model.Address.CountryCode;
                address.PostalCode = model.Address.PostalCode;
                address.StateProvince = model.Address.StateProvince;
                await _addressService.Update(address);
            }


            CookieService.Add(CookieKeys.DisplayName, user.FirstName, DateTime.MaxValue);
            CookieService.Add(CookieKeys.LastSignInEmail, user.Email, DateTime.MaxValue);

            if (emailChanged)
            {
                await SendEmailChangedEmail(user, originalEmail);
            }

            SetStatusMessage(Global.EditAccountNameSuccessMessage);

            return RedirectToAction("Dashboard");
        }

        private async Task SendEmailChangedEmail(ApplicationUser user, string originalEmail)
        {
            var emailTemplate = await EmailTemplateService.GetByTemplateType(EmailTemplateType.EmailChanged);
            var email = new Email
            {
                ToAddress = originalEmail,
                Subject = emailTemplate.Localize(Language, i => i.Subject).Subject,
                Message = string.Format(emailTemplate.Localize(Language, i => i.Message).Message, user.FirstName, AppSettings.SiteName)
            };

            await MessageService.Send(email.WithTemplate());
        }


        [Route("notifications")]
        public async Task<ActionResult> Notifications()
        {
            var model = await new NotificationModelView().Hydrate(AuthService, _categoryService, _notificationService, Language);
            return View(model);
        }

        [Route("notifications")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Notifications(NotificationModelView model)
        {
            var userId = AuthService.CurrentUserId();

            // clear all existing notifications for user
            var notifications = (await _notificationService.GetByUser(userId)).ToList();
            await _notificationService.DeleteList(notifications);

            // add back selected notifications
            if (model.SelectedCategories != null)
            {
                var newNotifications = model.SelectedCategories.Select(i => new CategoryNotification
                {
                    UserId = userId,
                    CategoryId = i
                });

                await _notificationService.AddList(newNotifications);
            }

            SetStatusMessage(Global.CategoryNotificationSaveMessage);
            return RedirectToAction("Notifications");
        }

        [Route("edit-profile")]
        public async Task<ActionResult> EditProfile()
        {
            var user = await AuthService.CurrentUser();
            var userProfile = await _userProfileService.GetByUser(user);
            var model = await new EditProfileModel().Hydrate(_avatarService, userProfile);
            return View(model);
        }

        [Route("edit-profile"), HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(EditProfileModel model, int? selectedAvatar)
        {
            var user = await AuthService.CurrentUser();
            var userProfile = await _userProfileService.GetByUser(user);

            if (!ModelState.IsValid)
            {
                return View(await model.Hydrate(_avatarService, userProfile));
            }

            userProfile.Status = model.UserProfile.Status;
            //userProfile.ProfileUrl = model.UserProfile.ProfileUrl;
            if (selectedAvatar.HasValue)
            {
                userProfile.Avatar = null;
                userProfile.AvatarId = selectedAvatar.Value;
            }
            
            await _userProfileService.Update(userProfile);
            
            SetStatusMessage("Your profile has been saved successfully.");
            return RedirectToAction("Dashboard");

        }
    }
}