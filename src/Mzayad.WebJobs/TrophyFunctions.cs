using Microsoft.Azure.WebJobs;
using Mzayad.Models.Queues;
using System.IO;
using System.Threading.Tasks;

namespace Mzayad.WebJobs
{
    public class TrophyFunctions
    {
        public async Task ProcessQueueItem([QueueTrigger("%trophies%")] UserTrophyMessage message, TextWriter log)
        {
            await log.WriteLineAsync($"Handling messing: {message.ToJson()}...");

            
        }
    }
}
