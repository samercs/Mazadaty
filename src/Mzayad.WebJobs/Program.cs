using Microsoft.Azure.WebJobs;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Services.Activity;
using Mzayad.Services.Identity;
using Mzayad.Services.Trophies;
using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mzayad.WebJobs
{
    internal class Program
    {
        private static void Main()
        {
            new JobHost().RunAndBlock();
        }
    }

    public class Functions
    {
        private static void LogMessage(TextWriter log, string message)
        {
            log.WriteLine(message);
            Console.WriteLine(message);
            Trace.TraceInformation(message);
        }

        private static async Task LogMessageAsync(TextWriter log, string message)
        {
            await log.WriteLineAsync(message);
            Console.WriteLine(message);
            Trace.TraceInformation(message);
        }

        public static async Task ProcessActivity([QueueTrigger("activities")] ActivityEvent activityEvent, TextWriter log)
        {
            var message = string.Format("Processing activity {0} for user ID {1}...", activityEvent.Type, activityEvent.UserId);
            await LogMessageAsync(log, message);

            var dataContextFactory = new DataContextFactory();
            var userService = new UserService(dataContextFactory);
            var trophyService = new TrophyService(dataContextFactory);
            var trophyEngine = TrophyEngineFactory.CreateInstance(activityEvent.Type, dataContextFactory);            
            var emailTemplateService = new EmailTemplateService(dataContextFactory);

            try
            {
                var user = await userService.GetUserById(activityEvent.UserId);
                if (user == null)
                {
                    await LogMessageAsync(log, string.Format("No user exists for user ID {0}.", activityEvent.UserId));
                    return;
                }

                IEnumerable<TrophyKey> trophies;

                EmailTemplate emailTemplate;

                switch (activityEvent.Type)
                {
                    case ActivityType.SubmitBid:
                        trophies = trophyEngine.GetEarnedTrophies(user).ToList();
                        trophyService.AwardTrophyToUser(trophies, user.Id);
                        emailTemplate = await emailTemplateService.GetByTemplateType(EmailTemplateType.TrohpyEarned);
                        message = string.Format(emailTemplate.Localize(activityEvent.Language, i => i.Message).Message, user.FirstName, TrophiesHtmlTable(trophies, trophyService));
                        await SendEmail(user, emailTemplate.Localize(activityEvent.Language, i => i.Subject).Subject, message);
                        break;

                    case ActivityType.VisitSite:
                        trophies = trophyEngine.GetEarnedTrophies(user);
                        trophyService.AwardTrophyToUser(trophies, user.Id);
                        break;

                    case ActivityType.EarnXp:
                        user.Xp += activityEvent.XP;
                        var newLevel = LevelService.GetLevelByXp(user.Xp).Index;
                        if (newLevel > user.Level)
                        {
                            user.Level = newLevel;
                            emailTemplate = await emailTemplateService.GetByTemplateType(EmailTemplateType.LevelUp);
                            message = string.Format(emailTemplate.Localize(activityEvent.Language, i => i.Message).Message, user.FirstName, user.Level);
                            await SendEmail(user, emailTemplate.Localize(activityEvent.Language, i => i.Subject).Subject, message);
                        }
                        await userService.UpdateUser(user);
                        break;

                    case ActivityType.AutoBid:
                        trophies = trophyEngine.GetEarnedTrophies(user).ToList();
                        trophyService.AwardTrophyToUser(trophies, user.Id);
                        emailTemplate = await emailTemplateService.GetByTemplateType(EmailTemplateType.TrohpyEarned);
                        message = string.Format(emailTemplate.Localize(activityEvent.Language, i => i.Message).Message, user.FirstName, TrophiesHtmlTable(trophies, trophyService));
                        await SendEmail(user, emailTemplate.Localize(activityEvent.Language, i => i.Subject).Subject, message);
                        break;

                    case ActivityType.WinAuction:
                        trophies = trophyEngine.GetEarnedTrophies(user);
                        trophyService.AwardTrophyToUser(trophies, user.Id);
                        emailTemplate = await emailTemplateService.GetByTemplateType(EmailTemplateType.TrohpyEarned);
                        message = string.Format(emailTemplate.Localize(activityEvent.Language, i => i.Message).Message, user.FirstName, TrophiesHtmlTable(trophies, trophyService));
                        await SendEmail(user, emailTemplate.Localize(activityEvent.Language, i => i.Subject).Subject, message);
                        break;

                    case ActivityType.CompleteProfile:
                        trophies = trophyEngine.GetEarnedTrophies(user).ToList();
                        trophyService.AwardTrophyToUser(trophies, user.Id);
                        emailTemplate = await emailTemplateService.GetByTemplateType(EmailTemplateType.TrohpyEarned);
                        message = string.Format(emailTemplate.Localize(activityEvent.Language, i => i.Message).Message, user.FirstName, TrophiesHtmlTable(trophies, trophyService));
                        await SendEmail(user, emailTemplate.Localize(activityEvent.Language, i => i.Subject).Subject, message);
                        break;

                    default:
                        await LogMessageAsync(log, string.Format("No event handling for activity {0}.", activityEvent.Type));
                        break;
                }

                var activityEventService = new ActivityEventService(dataContextFactory);
                await activityEventService.AddActivity(activityEvent);
            }
            catch (Exception ex)
            {
                LogMessage(log, ex.Message);
                LogMessage(log, ex.StackTrace);
                throw;
            }
        }

        private static async Task SendEmail(ApplicationUser user, string subject, string message)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://mzayad.azurewebsites.net/area/api/");

                var requestContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("FromAddress", "noreply@mzayad.com"),
                    new KeyValuePair<string, string>("FromName", "Mzayad"),
                    new KeyValuePair<string, string>("Message", message),
                    new KeyValuePair<string, string>("Subject",subject),
                    new KeyValuePair<string, string>("ToAddress",user.Email)
                });

                await client.PostAsync("messages", requestContent);
            }
        }

        private static async Task<string> TrophiesHtmlTable(IEnumerable<TrophyKey> keys, TrophyService trophyService)
        {
            var table = new StringBuilder();
            table.Append("<table>");
            foreach (var key in Enum.GetValues(typeof(TrophyKey)).Cast<TrophyKey>())
            {
                var trophy = await trophyService.GetTrophy(key);
                if (trophy != null)
                {
                    table.Append("<tr>");
                    table.Append("<td>" + trophy.Name + "</td>");
                    table.Append("<td>" + trophy.Description + "</td>");
                    table.Append("<td><img src='" + trophy.IconUrl + "' style='width:50px;'/></td>");
                    table.Append("/<tr>");
                }
            }
            table.Append("</table>");
            return table.ToString();
        }
    }
}
