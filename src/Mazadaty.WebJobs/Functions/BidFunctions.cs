using Microsoft.Azure.WebJobs;
using Mazadaty.Models;
using Mazadaty.Models.Enums;
using Mazadaty.Models.Queues;
using Mazadaty.Services.Trophies;
using System.IO;
using System.Threading.Tasks;

namespace Mazadaty.WebJobs.Functions
{
    public class BidFunctions : FunctionsBase
    {
        public async Task ProcessMessage([QueueTrigger("%bids%")] BidMessage message, TextWriter log)
        {
            var user = await UserService.GetUserById(message.UserId);
            if (user == null)
            {
                await log.WriteLineAsync($"No user exists for user ID {message.UserId}.");
                return;
            }

            await AddBidXp(user, message);

            if (message.Type == BidType.Manual)
            {
                var trophyEngine = new SubmitBidTrophyEngine(DataContextFactory);
                var earnedTrophyKeys = await trophyEngine.TryGetEarnedTrophies(user);
                foreach (var trophyKey in earnedTrophyKeys)
                {
                    await QueueService.LogTrophy(user, trophyKey);
                }
            }
        }

        private async Task AddBidXp(ApplicationUser user, BidMessage bid)
        {
            var xp = bid.Type == BidType.Manual ? Bid.ManualBidXp : Bid.AutoBidXp;

            await UserService.AddXp(user.Id, xp);
        }
    }
}
