using Mazadaty.Models;
using Mazadaty.Models.Enums;
using Mazadaty.Services;
using Mazadaty.Services.Identity;
using Mazadaty.Web.Core.Identity;
using Mazadaty.Web.Core.Services;
using Mazadaty.Web.Models.Prize;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Base.Web;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mazadaty.Services.Messaging;
using Mazadaty.Web.Extensions;

namespace Mazadaty.Web.Controllers
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
            if (!_prizeService.ValidatePrize(user, prizeLog))
            {
                return HttpNotFound();
            }

            var prizes = await _prizeService.GetAvaliablePrize();
            if (!prizes.Any())
            {
                throw new Exception("No prize found.");
            }
            Random rnd = new Random();
            var data = prizes.Select(i => new { i.Title, i.Weight, i.PrizeId, i.PrizeType, SortOrder = rnd.Next(1, 100) });
            data = data.OrderBy(i => i.SortOrder);
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
            var result = await _prizeService.GetRandomPrize(id, user, Language, NotifyAdminForWinning,
                AddUserSubscription);
            if (string.IsNullOrEmpty(result))
            {
                return HttpNotFound();
            }
            return Content(result);
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


    }
}
