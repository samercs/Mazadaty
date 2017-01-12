using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Services.Identity;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Extensions;
using Mzayad.Web.Models.Prize;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Localization;
using OrangeJetpack.Services.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/prize")]
    public class PrizeController : ApplicationController
    {
        private readonly PrizeService _prizeService;
        private readonly UserService _userService;
        private readonly SubscriptionService _subscriptionService;
        private readonly AvatarService _avatarService;

        public PrizeController(IAppServices appServices) : base(appServices)
        {
            _prizeService = new PrizeService(DataContextFactory);
            _userService = new UserService(DataContextFactory);
            _subscriptionService = new SubscriptionService(DataContextFactory);
            _avatarService = new AvatarService(DataContextFactory);
        }


        [Route("{id:int}"), Authorize]
        public async Task<ActionResult> Index(int id)
        {
            var user = await AuthService.CurrentUser();
            var prizeLog = await _prizeService.GetPrizeLogById(id);
            if (!ValidatePrize(user, prizeLog))
            {
                return HttpNotFound();
            }

            var prizes = await _prizeService.GetAvaliablePrize();
            if (!prizes.Any())
            {
                throw new Exception("No prize found.");
            }
            var data = prizes.Select(i => new { i.Title, i.Weight, i.PrizeId, i.PrizeType });
            var model = new IndexViewModel
            {
                PrizesJson =
                    JsonConvert.SerializeObject(data, Formatting.Indented,
                        new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                PrizeId = id
            };
            return View(model);
        }

        [Route("{id:int}/random-prize"), Authorize, HttpPost]
        public async Task<ActionResult> GetRandomPrize(int id)
        {
            var user = await AuthService.CurrentUser();
            var prizeLog = await _prizeService.GetPrizeLogById(id);
            if (!ValidatePrize(user, prizeLog))
            {
                return HttpNotFound();
            }

            Prize prize = null;
            var prizes = await _prizeService.GetAvaliablePrize();
            while (prize == null)
            {
                prize = await _prizeService.GetRandomPrize();
            }
            var index = -1;
            for (int i = 0; i < prizes.Count(); i++)
            {
                if (prize.PrizeId == prizes.ElementAt(i).PrizeId)
                {
                    index = i + 1;
                    break;
                }
            }

            if (index == -1)
            {
                return HttpNotFound();
            }
            var message = await ProccessPrize(user, prize);
            var isComplete = prize.PrizeType == PrizeType.Subscription;
            await _prizeService.LogUserPrize(prizeLog, prize.PrizeId, isComplete);
            var data = new { prizeId = prize.PrizeId, index, message, type = (int)prize.PrizeType };
            return Content(JsonConvert.SerializeObject(data, Formatting.Indented,
                        new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }


        private async Task<string> ProccessPrize(ApplicationUser user, Prize prize)
        {
            prize = prize.Localize(Language, i => i.Title);
            if (prize.Limit.HasValue)
            {
                prize.Limit = prize.Limit - 1;
                await _prizeService.Save(prize);
            }
            switch (prize.PrizeType)
            {
                case PrizeType.Product:
                    await NotifyAdminForWinning(user, prize);
                    return $"Congratulation ... you win {prize.Title}. We will conatct you to get your prize.";
                case PrizeType.Avatar:
                    return $"Congratulation ... you win {prize.Title}. We will redirect you to select your avatar.";
                case PrizeType.Subscription:
                    await AddUserSubscription(user, prize.SubscriptionDays);
                    return $"Congratulation ... you win {prize.Title}. The subscription has been added to your account.";
                default:
                    throw new Exception("Unsupport prize ...");
            }
            return "";
        }

        [Route("select-avatar")]
        public async Task<ActionResult> SelectAvatarPrize()
        {
            var user = await AuthService.CurrentUser();
            if (user == null)
            {
                return HttpNotFound();
            }
            var userHasPrize = await _prizeService.UserHasFreeAvatar(user);
            if (!userHasPrize)
            {
                return HttpNotFound();
            }
            var model = new SelectAvatarPrizeViewModel
            {
                PremiumAvatars = await _avatarService.GetPremiumAvatar(user)
            };
            return View(model);
        }

        [Route("select-avatar"), HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectAvatarPrize(SelectAvatarPrizeViewModel model)
        {
            var user = await AuthService.CurrentUser();
            if (user == null)
            {
                return HttpNotFound();
            }
            var userHasPrize = await _prizeService.UserHasFreeAvatar(user);
            if (!userHasPrize)
            {
                return HttpNotFound();
            }
            if (!model.SelectedAvatarId.HasValue)
            {
                SetStatusMessage("Please select your winning avatar.", StatusMessageType.Warning);
                return RedirectToAction("SelectAvatarPrize");
            }
            var avatar = await _avatarService.GetById(model.SelectedAvatarId.Value);
            if (avatar == null)
            {
                return HttpNotFound();
            }
            await _avatarService.AddAvatarToUser(user, avatar);
            user.AvatarUrl = avatar.Url;
            await _userService.UpdateUser(user);
            await _prizeService.UpdateUserHasFreeAvatar(user);
            SetStatusMessage("Your avatar image has been set successfully.");
            return RedirectToAction("Dashboard", "User", new { Language });

        }

        private async Task AddUserSubscription(ApplicationUser user, int? amount)
        {
            if (!amount.HasValue)
            {
                throw new Exception("Prize of type subscription without Subscription days");
            }
            var currentSubscription = user.SubscriptionUtc?.AddHours(-3) ?? DateTime.UtcNow;
            var newSubscription = currentSubscription.AddDays(amount.Value);
            var currentUserId = AuthService.CurrentUserId();
            var hostAddress = AuthService.UserHostAddress();
            await _subscriptionService.AddSubscriptionToUser(user, newSubscription, currentUserId, hostAddress);
        }

        private async Task NotifyAdminForWinning(ApplicationUser user, Prize prize)
        {

            var admins = await _userService.GetUsers("", Role.Administrator.ToString());
            var email = new Email
            {
                ToAddress = string.Join(";", admins.Select(i => i.Email))
            };

            EmailTemplate template = await EmailTemplateService.GetByTemplateType(EmailTemplateType.AdminWinningPrizeNotification, "en");
            email.Subject = string.Format(template.Subject, AppSettings.SiteName);
            email.Message = string.Format(template.Message, AppSettings.SiteName, NameFormatter.GetFullName(user.FirstName, user.LastName), user.Email, user.PhoneNumber, prize.Title);
            await MessageService.Send(email.WithTemplate());
        }

        private bool ValidatePrize(ApplicationUser user, UserPrizeLog userPrizeLog)
        {
            if (user == null)
            {
                return false;
            }
            if (userPrizeLog == null)
            {
                return false;
            }
            if (userPrizeLog.PrizeId.HasValue)
            {
                return false;
            }
            if (!userPrizeLog.UserId.Equals(user.Id))
            {
                return false;
            }
            return true;
        }


    }
}