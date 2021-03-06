using Microsoft.AspNet.Identity;
using Mazadaty.Core.Exceptions;
using Mazadaty.Models;
using Mazadaty.Models.Enum;
using Mazadaty.Models.Enums;
using Mazadaty.Services;
using Mazadaty.Services.Identity;
using Mazadaty.Services.Queues;
using Mazadaty.Web.Areas.Api.Models.Users;
using Mazadaty.Web.Core.Services;
using Mazadaty.Web.Extensions;
using Mazadaty.Web.Models.Account;
using Mazadaty.Web.Models.Shared;
using Mazadaty.Web.Models.User;
using Mazadaty.Web.Resources;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Base.Core.Security;
using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Mazadaty.Services.Messaging;
using WebGrease.Css.Extensions;

namespace Mazadaty.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApplicationApiController
    {
        private readonly UserService _userService;
        private readonly AddressService _addressService;
        private readonly SessionLogService _sessionLogService;
        private readonly AvatarService _avatarService;
        private readonly IQueueService _queueService;
        private readonly PrizeService _prizeService;
        private readonly FriendService _friendService;
        private readonly TrophyService _trophyService;
        private readonly AuctionService _auctionService;
        private readonly BidService _bidService;
        private readonly MessageService _messageService;
        private readonly WishListService _wishListService;
        private readonly CategoryService _categoryService;
        private readonly NotificationService _notificationService;

        public UsersController(IAppServices appServices) : base(appServices)
        {
            _userService = new UserService(DataContextFactory);
            _addressService = new AddressService(DataContextFactory);
            _sessionLogService = new SessionLogService(DataContextFactory);
            _avatarService = new AvatarService(DataContextFactory);
            _queueService =
                new QueueService(ConfigurationManager.ConnectionStrings["QueueConnection"].ConnectionString);
            _prizeService = new PrizeService(DataContextFactory);
            _friendService = new FriendService(DataContextFactory);
            _trophyService = new TrophyService(DataContextFactory);
            _auctionService = new AuctionService(DataContextFactory, _queueService);
            _bidService = new BidService(DataContextFactory, _queueService);
            _messageService = new MessageService(DataContextFactory);
            _wishListService = new WishListService(DataContextFactory);
            _categoryService = new CategoryService(DataContextFactory);
            _notificationService = new NotificationService(DataContextFactory);

        }

        [HttpGet, Route("{username}")]
        public async Task<IHttpActionResult> Get(string username)
        {
            var applicationUser = await _userService.GetUserByName(username);
            if (applicationUser == null)
            {
                return NotFound();
            }

            var address = await _addressService.GetAddress(applicationUser.AddressId);
            var user = new UserModel
            {
                UserId = applicationUser.Id,
                FullName = NameFormatter.GetFullName(applicationUser.FirstName, applicationUser.LastName),
                UserName = applicationUser.UserName,
                Email = applicationUser.Email,
                AvatarUrl = applicationUser.AvatarUrl,
                CountryCode = address.CountryCode
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

        [Route("")]
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
                Selected = i.Url.Equals(user.AvatarUrl)
            });

            var friends = await _friendService.GetFriends(user.Id);
            var friendsModel = friends.Select(i => new { i.FullName, i.UserName, i.Id, i.Gender });

            var trophies = await _trophyService.GetUserTrophies(user.Id);
            var trophiesModel =
                trophies.Select(
                    i => new { i.Trophy.Description, i.Trophy.IconUrl, i.Trophy.Name, i.Trophy.XpAward, i.Trophy.Key });

            var userPrizeLog = await _prizeService.GetUserAvilablePrize(user);

            return Ok(new
            {
                user.ProfileStatus,
                profileUrl,
                user.Gender,
                user.Birthdate,
                user.AvatarUrl,
                user.SubscriptionUtc,
                user.SubscriptionExpire,
                user.Tokens,
                user.Xp,
                user.Level,
                avatars,
                user.UserName,
                userPrizeLogId = userPrizeLog?.UserPrizeLogId,
                frinds = friendsModel,
                trophies = trophiesModel,
                XpNextLevel = LevelService.GetLevel(user.Level + 1).XpRequired
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
            await _queueService.QueueActivityAsync(ActivityType.CompleteProfile, user.Id);
            return Ok();
        }

        [Route("current/prize")]
        public async Task<IHttpActionResult> GetPrize()
        {
            var user = await AuthService.CurrentUser();
            var userPrize = await _prizeService.GetUserAvilablePrize(user);
            if (userPrize == null)
            {
                return NotFound();
            }
            return Ok(userPrize.UserPrizeLogId);
        }
        [Route("current/trophies")]
        public async Task<IHttpActionResult> GetTrophies()
        {
            var model = await GetTrophies(AuthService.CurrentUserId());
            return Ok(model);
        }

        [Route("current/auction-history")]
        public async Task<IHttpActionResult> GetAuctionHistory()
        {
            var userId = AuthService.CurrentUserId();
            var auctions = await _auctionService.GetAuctionsWon(userId, Language);
            return Ok(auctions);
        }
        [Route("current/bid-history")]
        public async Task<IHttpActionResult> GetBidHistory()
        {
            var userId = AuthService.CurrentUserId();
            var results = await _bidService.GetAuctionBidHistoryForUser(userId, Language);

            return Ok(results.Select(i => new
            {
                i.AuctionId,
                i.Title,
                i.ProductImageUrl,
                i.StartUtc,
                i.ClosedUtc,
                i.WonAmount,
                WonUser = new {
                    wonByUserId = i.WonUser?.Id,
                    i.WonUser?.UserName,
                    i.WonUser?.AvatarUrl
                },
                i.UserBidsCount,
                i.MaximumBid
            }));
        }

        private async Task<List<TrophieViewModel>> GetTrophies(string userId)
        {
            var userTophies = (await _trophyService.GetUserTrophies(userId, Language)).ToList();
            var trophies = await _trophyService.GetAll(Language);

            return (from trophy in trophies
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
            catch
            {
                // do nothing
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
            catch
            {
                // do nothing
            }
        }

        private string GetBaseUrl(string action)
        {
            var baseUri = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, string.Empty));
            var resourceRelative = "~/en/account/" + action;
            var resourceFullPath = new Uri(baseUri, VirtualPathUtility.ToAbsolute(resourceRelative));

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

        [Route("profiles/{userName}")]
        [HttpGet]
        public async Task<IHttpActionResult> UserProfile(string userName)
        {
            var user = await _userService.GetUserByName(userName);
            if (user == null)
            {
                return NotFound();
            }
            var nextLevel = LevelService.GetLevel(user.Level + 1);
            var frinds = await _friendService.GetFriends(user.Id);
            var result =
                new
                {
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.ProfileStatus,
                    user.AvatarUrl,
                    user.Xp,
                    user.Level,
                    user.UserName,
                    user.Gender,
                    user.Birthdate,
                    nextLevel,
                    Trophies = (await _trophyService.GetTrophies(user.Id, Language)).Count,
                    AreFriends = await _friendService.AreFriends(user.Id, AuthService.CurrentUserId()),
                    SentFriendRequestBefore = await _friendService.SentBefore(AuthService.CurrentUserId(), user.Id),
                    Friends = frinds.Select(i => new
                    {
                        i.FirstName,
                        i.LastName,
                        i.Email,
                        i.UserName,
                        i.AvatarUrl,
                        i.ProfileStatus,
                        i.Gender,
                        i.ProfileUrl,
                        i.Birthdate
                    }),
                    Me = user.UserName == (await AuthService.CurrentUser()).UserName
                };
            return Ok(result);
        }

        [Route("current/friends")]
        [HttpGet, Authorize]
        public async Task<IHttpActionResult> GetFriends()
        {
            var friends = await _friendService.GetFriends(AuthService.CurrentUserId());

            var model = await GetFriendsModel(friends);
            var result = model.Select(i => new
            {
                i.User.FirstName,
                i.User.LastName,
                i.User.Email,
                i.User.ProfileStatus,
                i.User.AvatarUrl,
                i.User.Xp,
                i.User.Level,
                i.User.UserName,
                i.User.Gender,
                i.User.Birthdate,
                i.NextLevel,
                Trophies = i.Trophies.Count,
                i.AreFriends,
                i.SentFriendRequestBefore,
                i.Me,
                Friends = i.Friends.Count
            });
            return Ok(result);
        }

        [Route("profiles/{userName}/trophies")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserTrophies(string userName)
        {
            var user = await _userService.GetUserByName(userName);
            if (user == null)
            {
                return NotFound();
            }
            var model = await GetTrophies(user.Id);
            return Ok(model);
        }

        [Route("profiles/{userName}/friends")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserFriends(string userName)
        {
            var user = await _userService.GetUserByName(userName);
            if (user == null)
            {
                return NotFound();
            }
            var friends = await _friendService.GetFriends(user.Id);

            var viewModel = await GetFriendsModel(friends);
            var result = viewModel.Select(i => new
            {
                i.User.FirstName,
                i.User.LastName,
                i.User.Email,
                i.User.ProfileStatus,
                i.User.AvatarUrl,
                i.User.Xp,
                i.User.Level,
                i.User.UserName,
                i.User.Gender,
                i.User.Birthdate,
                i.NextLevel,
                Trophies = i.Trophies.Count,
                i.AreFriends,
                i.SentFriendRequestBefore,
                i.Me,
                Friends = i.Friends.Count
            });

            return Ok(result);
        }

        [Route("current/inbox")]
        [HttpGet, Authorize]
        public async Task<IHttpActionResult> GetUserInbox()
        {
            var allMessages = await _messageService.GetByReceiver(AuthService.CurrentUserId());
            var messageGroup = allMessages.GroupBy(i => i.User, i => i, (key, g) => new
            {
                From = key,
                Message = g.ToList().FirstOrDefault()
            });

            var result = messageGroup.Select(i => new
            {
                From = new
                {
                    i.From.UserName,
                    i.From.FirstName,
                    i.From.LastName,
                    i.From.AvatarUrl
                },
                Message = new
                {
                    i.Message.Body,
                    i.Message.Summary,
                    i.Message.IsNew,
                    i.Message.MessageId,
                    i.Message.ReceiverId,
                    i.Message.UserId,
                    i.Message.CreatedUtc
                }
            });
            return Ok(result);
        }
        private async Task<List<UserProfileViewModel>> GetFriendsModel(IReadOnlyCollection<ApplicationUser> friends)
        {
            var viewModel = new List<UserProfileViewModel>();
            foreach (var friend in friends)
            {
                friend.AvatarUrl = friend.AvatarUrl;
                viewModel.Add(new UserProfileViewModel(friend)
                {
                    Trophies = await _trophyService.GetTrophies(friend.Id, Language),
                    AreFriends = await _friendService.AreFriends(friend.Id, AuthService.CurrentUserId()),
                    SentFriendRequestBefore = await _friendService.SentBefore(AuthService.CurrentUserId(), friend.Id),
                    Friends = await _friendService.GetFriends(friend.Id),
                    Me = friend.UserName == (await AuthService.CurrentUser()).UserName
                });
            }
            return viewModel;
        }

        [Route("current/wishlist")]
        [HttpGet, Authorize]
        public async Task<IHttpActionResult> Wishlist()
        {
            var user = await AuthService.CurrentUser();
            var userWishlist = await _wishListService.GetByUser(user.Id);
            var result = userWishlist.Select(i => new
            {
                i.NameEntered,
                i.NameNormalized,
                i.WishListId,
                i.CreatedUtc
            });
            return Ok(result);
        }

        [Route("current/wishlist")]
        [HttpPost, Authorize]
        public async Task<IHttpActionResult> AddWishlist(WishList wishList)
        {
            var user = await AuthService.CurrentUser();
            wishList.UserId = user.Id;
            if (ModelState.IsValid)
            {
                return ModelStateError(ModelState);
            }
            await _wishListService.Add(wishList);
            return Ok(new { message = "Wishlist has been added successfully." });
        }

        [Route("current/wishlist/{wishlistId:int}")]
        [HttpDelete, Authorize]
        public async Task<IHttpActionResult> DeleteWishlist(int wishlistId)
        {
            var wishlist = await _wishListService.GetById(wishlistId);
            if (wishlist == null)
            {
                return NotFound();
            }
            await _wishListService.Delete(wishlist);
            return Ok(new { message = "Wishlist has been removed successfully." });
        }

        [Route("current/notifications")]
        [HttpGet, Authorize]
        public async Task<IHttpActionResult> GetNotifications()
        {
            var user = await AuthService.CurrentUser();
            var model = await new NotificationModelView
            {
                AutoBidNotification = user.AutoBidNotification
            }.Hydrate(AuthService, _categoryService, _notificationService, Language);

            return Ok(model);
        }

        [Route("current/notifications")]
        [HttpPost, Authorize]
        public async Task<IHttpActionResult> UpdateNotifications(NotificationModelView model)
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
            return Ok(new { message = "notifications has been updated successfully" });
        }

        [Route("current/subscription")]
        [HttpGet, Authorize]
        public async Task<IHttpActionResult> GetSubscriptionStatus()
        {
            var user = await AuthService.CurrentUser();
            return Ok(user.Subscription == UserSubscriptionStatus.Active);
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
