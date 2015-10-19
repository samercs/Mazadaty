using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Services.Activity;
using Mzayad.Services.Identity;
using Mzayad.Services.Trophies;
using Mzayad.Models.Enum;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Mzayad.WebJobs
{
    class Program
    {
        static void Main()
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

            try
            {
                var user = await userService.GetUserById(activityEvent.UserId);
                if (user == null)
                {
                    await LogMessageAsync(log, string.Format("No user exists for user ID {0}.", activityEvent.UserId));
                    return;
                }

                IEnumerable<TrophyKey> trophies;

                switch (activityEvent.Type)
                {
                    case ActivityType.SubmitBid:
                        trophies = trophyEngine.GetEarnedTrophies(user);
                        trophyService.AwardTrophyToUser(trophies, user.Id);
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
                            await SendEmail(user.Email, "Congratulations!! you have been leveled up!!!", "Congratulations");
                        }
                        await userService.UpdateUser(user);
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
                throw;
            }
        }
        private static async Task SendEmail(string toEmail, string message, string subject)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44300/area/api/");

                var requestContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("FromAddress", "admin@mzayad.com"),
                    new KeyValuePair<string, string>("FromName", "Mzayad"),
                    new KeyValuePair<string, string>("Message", message),
                    new KeyValuePair<string, string>("Subject",subject),
                    new KeyValuePair<string, string>("ToAddress",toEmail)
                });

                await client.PostAsync("messages", requestContent);
            }
        }
    }
}
