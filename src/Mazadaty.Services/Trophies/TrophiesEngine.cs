using System;
using System.Collections.Generic;
using System.Linq;
using Mazadaty.Models.Enum;
using Mazadaty.Services.Identity;
using OrangeJetpack.Services.Client.Messaging;
using OrangeJetpack.Services.Models;

namespace Mazadaty.Services.Trophies
{
    public class TrophiesEngine
    {
        private readonly TrophyService _trophyService;
        private readonly UserService _userService;
        private readonly EmailTemplateService _emailTemplateService;
        private readonly MessageService _messageService;

        public TrophiesEngine(TrophyService trophyService, UserService userService, EmailTemplateService emailTemplateService, MessageService messageService)
        {
            _trophyService = trophyService;
            _userService = userService;
            _emailTemplateService = emailTemplateService;
            _messageService = messageService;
        }

        public async void EarnTrophy(string userId)
        {
            var earnedTrophies = new List<string>();
            var user = await _userService.GetUserById(userId);
            foreach (var key in from key in Enum.GetValues(typeof(TrophyKey)).Cast<TrophyKey>()
                                let checker = TrophiesChecker.CreateInstance(key, _trophyService)
                                where checker.Check(user.Id).Result select key)
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
                await _messageService.Send(email);
            }
            catch //(Exception ex)
            {
               // new RaygunClient().Send(ex);
            }
        }
    }
}
