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
using OrangeJetpack.Base.Core.Security;
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

        public PrizeController(IAppServices appServices) : base(appServices)
        {
            _prizeService = new PrizeService(DataContextFactory);
            _userService = new UserService(DataContextFactory);
            _subscriptionService = new SubscriptionService(DataContextFactory);
        }


        [Route(""), Authorize]
        public async Task<ActionResult> Index(UrlTokenParameters parameters = null)
        {
            if (parameters == null)
            {
                return HttpNotFound();
            }
            if (!await ValidateParameter(parameters))
            {
                return HttpNotFound();
            }
            var prizes = await _prizeService.GetAvaliablePrize();
            if (!prizes.Any())
            {
                throw new Exception("No prize found.");
            }
            var data = prizes.Select(i => new { i.Title, i.Weight, i.PrizeId });
            var model = new IndexViewModel
            {
                PrizesJson =
                    JsonConvert.SerializeObject(data, Formatting.Indented,
                        new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Token = parameters
            };
            return View(model);
            //return Content(parameters.Timestamp.ToString() + " " + parameters.Hash);
        }

        [Route("random-prize"), HttpPost, Authorize]
        public async Task<ActionResult> GetRandomPrize(UrlTokenParameters token)
        {
            var user = await AuthService.CurrentUser();
            if (user == null)
            {
                return HttpNotFound();
            }

            if (!await ValidateParameter(token))
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
            await _prizeService.LogUserPrize(user.Id, prize.PrizeId, token.Token);
            var data = new { prizeId = prize.PrizeId, index, message, type = (int)prize.PrizeType };

            return Content(JsonConvert.SerializeObject(data, Formatting.Indented,
                        new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }

        [Route("test"), Authorize]
        public async Task<ActionResult> GetPrizeParameter()
        {
            UrlSecurity url = new UrlSecurity();
            var baseUrl = $"{AppSettings.CanonicalUrl}{Url.Action("Index", "Prize")}";
            var securyUrl = url.GenerateSecureUrl(baseUrl, null);
            return Redirect(securyUrl.OriginalString);
        }



        private async Task<string> ProccessPrize(ApplicationUser user, Prize prize)
        {
            prize = prize.Localize<Prize>(Language, i => i.Title);
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

                    //ToDo : let user select premium avatar
                    return $"Congratulation ... you win {prize.Title}. We will redirect you to select your avatar.";
                case PrizeType.Subscription:
                    await AddUserSubscription(user, prize.SubscriptionDays);
                    return $"Congratulation ... you win {prize.Title}. The subscription has been added to your account.";
                default:
                    throw new Exception("Unsupport prize ...");
            }
            return "";
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

        private async Task<bool> ValidateParameter(UrlTokenParameters token)
        {
            try
            {
                PasswordUtilities.ValidateResetPasswordParameters(token);
                return await _prizeService.ValidatePrizeHash(token.Token);

            }
            catch (Exception e)
            {
                return false;
            }
        }


    }
}