using Microsoft.Azure.WebJobs;
using Mzayad.Models.Queues;
using System.IO;
using System.Threading.Tasks;

namespace Mzayad.WebJobs.Functions
{
    public class TrophyFunctions : FunctionsBase
    {
        public async Task ProcessMessage([QueueTrigger("%trophies%")] UserTrophyMessage message, TextWriter log)
        {
            await log.WriteLineAsync($"Handling messing: {message.ToJson()}...");

            var user = await UserService.GetUserById(message.UserId);
            if (user == null)
            {
                await log.WriteLineAsync($"No user exists for user ID {message.UserId}.");
                return;
            }

            await TrophyService.AwardTrophyToUser(message.UserId, message.TrophyKey);
        }
    }
}
