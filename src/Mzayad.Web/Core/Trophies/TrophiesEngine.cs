using System;
using System.Collections.Generic;
using System.Linq;
using Mzayad.Models.Enum;
using Mzayad.Services;
using Mzayad.Web.Extensions;
using OrangeJetpack.Services.Client.Messaging;
using OrangeJetpack.Services.Models;

namespace Mzayad.Web.Core.Trophies
{
    public class TrophiesEngine
    {
        private readonly TrophyService _trophyService;
        private readonly EmailTemplateService _emailTemplateService;
        private readonly UserProfileService _userProfileService;
        private readonly MessageService _messageService;

        public TrophiesEngine(TrophyService trophyService, EmailTemplateService emailTemplateService, UserProfileService userProfileService, MessageService messageService)
        {
            _trophyService = trophyService;
            _emailTemplateService = emailTemplateService;
            _userProfileService = userProfileService;
            _messageService = messageService;
        }

        public async void EarnTrophy(string userId)
        {
            var earnedTrophies = new List<string>();
            var user = await _userProfileService.GetByUserId(userId);
            foreach (var key in from key in Enum.GetValues(typeof(TrophyKey)).Cast<TrophyKey>()
                                let checker = TrophiesChecker.CreateInstance(key, _trophyService)
                                where checker.Check(user.UserId).Result select key)
            {
                await _trophyService.AddToUser((int)key, userId);
                earnedTrophies.Add(key.Name());
            }
            try
            {
                var template = await _emailTemplateService.GetByTemplateType(EmailTemplateType.TrohpyEarned);
                
                var email = new Email
                {
                    ToAddress = "[user.Email]",
                    Subject = template.Subject,
                    Message = string.Format(template.Message, "[user.FirstName]" , string.Join(" - ",earnedTrophies))
                };
                await _messageService.Send(email.WithTemplate());
            }
            catch //(Exception ex)
            {
               // new RaygunClient().Send(ex);
            }
        }
    }
}
