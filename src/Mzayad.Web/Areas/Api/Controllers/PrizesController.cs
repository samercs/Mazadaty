using Mzayad.Models;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Services.Identity;
using Mzayad.Web.Areas.Api.Models.Prizes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Extensions;
using Newtonsoft.Json;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Services.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/prizes")]
    public class PrizesController : ApplicationApiController
    {
        private readonly PrizeService _prizeService;
        private readonly SubscriptionService _subscriptionService;
        private readonly UserService _userService;
        private readonly AvatarService _avatarService;
        public PrizesController(IAppServices appServices) : base(appServices)
        {
            _prizeService = new PrizeService(DataContextFactory);
            _subscriptionService = new SubscriptionService(DataContextFactory);
            _userService = new UserService(DataContextFactory);
            _avatarService = new AvatarService(DataContextFactory);
        }
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            var prizes = await _prizeService.GetAvaliablePrize(Language);
            return Ok(prizes.Select(PrizeViewModel.Create));
        }

        [Route("{prizeLogId:int}/random-prize")]
        [Authorize]
        public async Task<IHttpActionResult> RandomPrize(int prizeLogId)
        {
            var user = await AuthService.CurrentUser();
            var result = await _prizeService.GetRandomPrize(prizeLogId, user, Language, NotifyAdminForWinning,
                AddUserSubscription);
            if (string.IsNullOrEmpty(result))
            {
                return NotFound();
            }
            return Ok(JsonConvert.DeserializeObject(result));
        }

        [Route("select-avatar"), HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> SelectAvatar()
        {
            var user = await AuthService.CurrentUser();
            var userHasPrize = await _prizeService.UserHasFreeAvatar(user);
            if (!userHasPrize)
            {
                return NotFound();
            }
            var avatars = await _avatarService.GetPremiumAvatar(user);
            return Ok(avatars.Select(AvatarViewModel.Create));
        }

        [Route("{avatarId:int}/select-avatar"), HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> SelectAvatar(int avatarId)
        {
            var user = await AuthService.CurrentUser();
            var userHasPrize = await _prizeService.UserHasFreeAvatar(user);
            if (!userHasPrize)
            {
                return NotFound();
            }
            
            var avatar = await _avatarService.GetById(avatarId);
            if (avatar == null)
            {
                return NotFound();
            }
            await _avatarService.AddAvatarToUser(user, avatar);
            user.AvatarUrl = avatar.Url;
            await _userService.UpdateUser(user);
            await _prizeService.UpdateUserHasFreeAvatar(user);
            return Ok(new { message = "Your avatar image has been set successfully." });
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
