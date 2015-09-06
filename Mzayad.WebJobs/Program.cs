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
using OrangeJetpack.Services.Client.Messaging;

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

                switch (activityEvent.Type)
                {
                    case ActivityType.SubmitBid:
                        var trophies = trophyEngine.GetEarnedTrophies(user);
                        trophyService.AwardTrophyToUser(trophies, user.Id);
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
    }
}
