using Microsoft.AspNet.Identity;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Services.Activity;
using Mzayad.Services.Identity;
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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/user"), Authorize]
    public class UserController : ApplicationController
    {
        private readonly UserService _userService;
        private readonly AddressService _addressService;
        private readonly CategoryService _categoryService;
        private readonly NotificationService _notificationService;
        private readonly AvatarService _avatarService;
        private readonly BidService _bidService;
        private readonly TrophyService _trophyService;
        private readonly AuctionService _auctionService;
        private readonly WishListService _wishListService;
        private readonly IActivityQueueService _activityQueueService;
        private readonly FriendService _friendService;

        public UserController(IAppServices appServices)
            : base(appServices)
        {
            _userService = new UserService(DataContextFactory);
            _addressService = new AddressService(DataContextFactory);
            _categoryService = new CategoryService(DataContextFactory);
            _notificationService = new NotificationService(DataContextFactory);
            _avatarService = new AvatarService(DataContextFactory);
            _bidService = new BidService(DataContextFactory);
            _trophyService = new TrophyService(DataContextFactory);
            _auctionService = new AuctionService(DataContextFactory);
            _wishListService = new WishListService(DataContextFactory);
            _activityQueueService = new ActivityQueueService(ConfigurationManager.ConnectionStrings["QueueConnection"].ConnectionString);
            _friendService = new FriendService(appServices.DataContextFactory);
        }

        [Route("dashboard")]
        public async Task<ActionResult> Dashboard()
        {
            var user = await AuthService.CurrentUser();

            var viewModel = new DashboardViewModel(user)
            {
                Bids = await _bidService.GetRecentBidHistoryForUser(user.Id, Language),
                Trophies = await _trophyService.GetTrophies(user.Id, Language),
                Auctions = await _auctionService.GetAuctionsWon(user.Id, Language),
                WishLists = await _wishListService.GetByUser(user.Id),
                FriendRequests = await _friendService.GetFriendRequests(user.Id)
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
            var address = new Address();
            if (user.AddressId.HasValue)
            {
                address = await _addressService.GetAddress(user.AddressId.Value);
            }

            var model = new UserAccountViewModel()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = new AddressViewModel(address).Hydrate(),
                PhoneCountryCode = user.PhoneCountryCode,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Birthdate = user.Birthdate
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
            user.Gender = model.Gender;
            user.Birthdate = model.Birthdate;
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
                address.Floor = model.Address.Floor;
                address.FlatNumber = model.Address.FlatNumber;
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
            var user = await AuthService.CurrentUser();
            var model = await new NotificationModelView
            {
                AutoBidNotification = user.AutoBidNotification
            }.Hydrate(AuthService, _categoryService, _notificationService, Language);

            return View(model);
        }

        [Route("notifications")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Notifications(NotificationModelView model)
        {
            var user = await AuthService.CurrentUser();
            user.AutoBidNotification = model.AutoBidNotification;

            // clear all existing notifications for user
            var notifications = (await _notificationService.GetByUser(user.Id)).ToList();
            await _notificationService.DeleteList(notifications);
            await _userService.UpdateUser(user);

            // add back selected notifications
            if (model.SelectedCategories != null)
            {
                var newNotifications = model.SelectedCategories.Select(i => new CategoryNotification
                {
                    UserId = user.Id,
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
            var model = await new EditProfileModel().Hydrate(_avatarService, user);
            return View(model);
        }

        [Route("edit-profile"), HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(EditProfileModel model, int? selectedAvatar)
        {
            var user = await AuthService.CurrentUser();
            user.ProfileStatus = model.User.ProfileStatus;
            user.Gender = model.User.Gender;

            if (selectedAvatar.HasValue)
            {
                var avatar = await _avatarService.GetById(selectedAvatar.Value);
                user.AvatarUrl = avatar.Url;
            }

            if (model.BirthDay.HasValue && model.BirthMonth.HasValue && model.BirthYear.HasValue)
            {
                var birthDate = new DateTime(model.BirthYear.Value, model.BirthMonth.Value, model.BirthDay.Value);
                user.Birthdate = birthDate;
            }

            await _userService.UpdateUser(user);
            await _activityQueueService.QueueActivityAsync(ActivityType.CompleteProfile, user.Id);

            SetStatusMessage("Your profile has been saved successfully.");

            return RedirectToAction("Dashboard");
        }

        [Route("trophies")]
        public async Task<ActionResult> Trophies()
        {
            var userId = AuthService.CurrentUserId();
            var userTophies = (await _trophyService.GetUsersTrophies(userId, Language)).ToList();
            var trophies = await _trophyService.GetAll(Language);

            var model = (from trophy in trophies
                         let userTrophy = userTophies.FirstOrDefault(i => i.TrophyId == trophy.TrophyId)
                         select new TrophieViewModel
                         {
                             TrophyName = trophy.Name,
                             TrophyDescription = trophy.Description,
                             IconUrl = trophy.IconUrl,
                             XpEarned = userTrophy == null ? (int?)null : userTrophy.XpAwarded,
                             AwardDate = userTrophy == null ? (DateTime?)null : userTrophy.CreatedUtc,
                             Earned = userTrophy != null
                         }).ToList();


            return View(model);
        }

        [Route("bid-history")]
        public async Task<ActionResult> BidHistory()
        {
            var userId = AuthService.CurrentUserId();
            var bids = await _bidService.GetBidHistoryForUser(userId, Language);

            return View(bids);
        }

        [Route("auction-history")]
        public async Task<ActionResult> AuctionHistory()
        {
            var userId = AuthService.CurrentUserId();
            var auctions = await _auctionService.GetAuctionsWon(userId, Language);

            return View(auctions);
        }

        [Route("~/{language}/profiles/{userName}")]
        public async Task<ActionResult> UserProfile(string userName)
        {
            var user = await _userService.GetUserByName(userName);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (user.Id == AuthService.CurrentUserId())
            {
                return RedirectToAction("dashboard", "user", new { area = "", language = Language });
            }
            var viewModel = new DashboardViewModel(user)
            {
                Bids = await _bidService.GetRecentBidHistoryForUser(user.Id, Language),
                Trophies = await _trophyService.GetTrophies(user.Id, Language),
                Auctions = await _auctionService.GetAuctionsWon(user.Id, Language),
                WishLists = await _wishListService.GetByUser(user.Id)
            };
            return View(viewModel);
        }

        [Route("friends")]
        public async Task<ActionResult> Friends()
        {
            var viewModel = new FriendsViewModel()
            {
                UserRequests = await _friendService.GetUserRequests(AuthService.CurrentUserId()),
                OthersRequests = await _friendService.GetFriendRequests(AuthService.CurrentUserId()),
                Friends = await _friendService.GetFriends(AuthService.CurrentUserId())
            };
            return View(viewModel);
        }
    }
}