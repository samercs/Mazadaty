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

            try
            {
                var dataContextFactory = new DataContextFactory();

                var _trophyService = new TrophyService(dataContextFactory);
                var _emailTemplateService = new EmailTemplateService(dataContextFactory);
                var _userService = new UserService(dataContextFactory);
                var _messageService = new MessageService(new EmailSettings());

                var trophyEngine = new TrophiesEngine(_trophyService, _userService, _emailTemplateService, _messageService);

                var userService = new UserService(dataContextFactory);
                var user = await userService.GetUserById(activityEvent.UserId);
                if (user == null)
                {
                    await LogMessageAsync(log, string.Format("No user exists for user ID {0}.", activityEvent.UserId));
                    return;
                }

                switch (activityEvent.Type)
                {
                    case ActivityType.TestActivity:
                        await HandleTestActivity(user, log);
                        break;
                    case ActivityType.SubmitBid:
                        // Earn trophy
                       trophyEngine.EarnTrophy(user.Id);
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

        private static async Task HandleTestActivity(ApplicationUser user, TextWriter log)
        {
            await LogMessageAsync(log, string.Format("Handling activity for {0}.", user.Email));
        }
    }
}
