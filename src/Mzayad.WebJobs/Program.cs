using Microsoft.Azure.WebJobs;
using Mindscape.Raygun4Net;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Models.Enums;
using Mzayad.Models.Queues;
using Mzayad.Services;
using Mzayad.Services.Activity;
using Mzayad.Services.Identity;
using Mzayad.Services.Queues;
using Mzayad.Services.Trophies;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            var host = new JobHost(new JobHostConfiguration
            {
                NameResolver = new QueueNameResolver()
            });
            host.RunAndBlock();
        }
    }

    public class Functions
    {
        private readonly IDataContextFactory _dataContextFactory;
        private readonly UserService _userService;
        private readonly TrophyService _trophyService;
        private readonly IQueueService _queueService;

        public Functions()
        {
            _dataContextFactory = new DataContextFactory();
            _userService = new UserService(_dataContextFactory);
            _trophyService = new TrophyService(_dataContextFactory);
            _queueService = new QueueService("DefaultEndpointsProtocol=https;AccountName=mzayad;AccountKey=dwZYAeSDA4lz8qixfZGdjNrohWnY59DtyTBHrnSOFoPgC4w/ueV232lq6H9n6WoieJDu+GNHUd1QuCT7Xa/5mw==");
        }

        public async Task HandleBid([QueueTrigger("%bids%")] BidMessage bid, TextWriter log)
        {
            await log.WriteLineAsync($"Handling bid: {bid.ToJson()}...");

            var user = await _userService.GetUserById(bid.UserId);
            if (user == null)
            {
                await log.WriteLineAsync($"No user exists for user ID {bid.UserId}.");
                return;
            }

            await AddBidXp(user, bid);

            if (bid.Type == BidType.Manual)
            {
                var trophyEngine = new SubmitBidTrophyEngine(_dataContextFactory);
                var earnedTrophyKeys = await trophyEngine.TryGetEarnedTrophies(user);
                foreach (var trophyKey in earnedTrophyKeys)
                {
                    await _queueService.LogTrophy(user, trophyKey);
                }
            }
        }

        private async Task AddBidXp(ApplicationUser user, BidMessage bid)
        {
            var xp = bid.Type == BidType.Manual ? Bid.ManualBidXp : Bid.AutoBidXp;
            user.Xp += xp;

            var newLevel = LevelService.GetLevelByXp(user.Xp).Index;
            if (newLevel > user.Level)
            {
                user.Level = newLevel;
            }

            await _userService.UpdateUser(user);
        }

        public static async Task ProcessActivity([QueueTrigger("%activities%")] ActivityEvent activityEvent, TextWriter log)
        {
            var message = $"Processing activity {activityEvent.Type} for user ID {activityEvent.UserId}...";
            await log.WriteLineAsync(message);

            try
            {
                var dataContextFactory = new DataContextFactory();
                var userService = new UserService(dataContextFactory);
                var trophyService = new TrophyService(dataContextFactory);
                var trophyEngine = TrophyEngineFactory.CreateInstance(activityEvent.Type, dataContextFactory);
                var emailTemplateService = new EmailTemplateService(dataContextFactory);

                var user = await userService.GetUserById(activityEvent.UserId);
                if (user == null)
                {
                    await log.WriteLineAsync($"No user exists for user ID {activityEvent.UserId}.");
                    return;
                }

                IEnumerable<TrophyKey> trophies;

                EmailTemplate emailTemplate;

                switch (activityEvent.Type)
                {
                    case ActivityType.SubmitBid:
                        //trophies = trophyEngine.GetEarnedTrophies(user).ToList();
                        //trophyService.AwardTrophyToUser(trophies, user.Id);
                        //emailTemplate = await emailTemplateService.GetByTemplateType(EmailTemplateType.TrophyEarned);
                        //message = string.Format(emailTemplate.Localize(activityEvent.Language, i => i.Message).Message, user.FirstName, TrophiesHtmlTable(trophies, trophyService));
                        //await SendEmail(user, emailTemplate.Localize(activityEvent.Language, i => i.Subject).Subject, message);
                        break;

                    case ActivityType.VisitSite:
                        //trophies = trophyEngine.GetEarnedTrophies(user);
                        //trophyService.AwardTrophyToUser(trophies, user.Id);
                        break;

                    case ActivityType.EarnXp:
                        user.Xp += activityEvent.XP;
                        var newLevel = LevelService.GetLevelByXp(user.Xp).Index;
                        if (newLevel > user.Level)
                        {
                            user.Level = newLevel;
                            //emailTemplate = await emailTemplateService.GetByTemplateType(EmailTemplateType.LevelUp);
                            //message = string.Format(emailTemplate.Localize(activityEvent.Language, i => i.Message).Message, user.FirstName, user.Level);
                            //await SendEmail(user, emailTemplate.Localize(activityEvent.Language, i => i.Subject).Subject, message);
                        }
                        await userService.UpdateUser(user);
                        break;

                    case ActivityType.AutoBid:
                        //trophies = trophyEngine.GetEarnedTrophies(user).ToList();
                        //trophyService.AwardTrophyToUser(trophies, user.Id);
                        //emailTemplate = await emailTemplateService.GetByTemplateType(EmailTemplateType.TrophyEarned);
                        //message = string.Format(emailTemplate.Localize(activityEvent.Language, i => i.Message).Message, user.FirstName, TrophiesHtmlTable(trophies, trophyService));
                        //await SendEmail(user, emailTemplate.Localize(activityEvent.Language, i => i.Subject).Subject, message);
                        break;

                    case ActivityType.WinAuction:
                        //trophies = trophyEngine.GetEarnedTrophies(user);
                        //trophyService.AwardTrophyToUser(trophies, user.Id);
                        //emailTemplate = await emailTemplateService.GetByTemplateType(EmailTemplateType.TrophyEarned);
                        //message = string.Format(emailTemplate.Localize(activityEvent.Language, i => i.Message).Message, user.FirstName, TrophiesHtmlTable(trophies, trophyService));
                        //await SendEmail(user, emailTemplate.Localize(activityEvent.Language, i => i.Subject).Subject, message);
                        break;

                    case ActivityType.CompleteProfile:
                        //trophies = trophyEngine.GetEarnedTrophies(user).ToList();
                        //trophyService.AwardTrophyToUser(trophies, user.Id);
                        //emailTemplate = await emailTemplateService.GetByTemplateType(EmailTemplateType.TrophyEarned);
                        //message = string.Format(emailTemplate.Localize(activityEvent.Language, i => i.Message).Message, user.FirstName, TrophiesHtmlTable(trophies, trophyService));
                        //await SendEmail(user, emailTemplate.Localize(activityEvent.Language, i => i.Subject).Subject, message);
                        break;

                    default:
                        await log.WriteLineAsync($"No event handling for activity {activityEvent.Type}.");
                        break;
                }

                var activityEventService = new ActivityEventService(dataContextFactory);
                await activityEventService.AddActivity(activityEvent);
            }
            catch (Exception ex)
            {
                new RaygunClient().SendInBackground(ex);

                await log.WriteLineAsync(ex.Message);
                await log.WriteLineAsync(ex.StackTrace);
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
