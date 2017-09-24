using System.Threading.Tasks;
using Mazadaty.Models;
using Mazadaty.Models.Enum;
using Mazadaty.Models.Enums;

namespace Mazadaty.Services.Queues
{
    public interface IQueueService
    {
        void LogBid(Bid bid);
        Task LogTrophy(ApplicationUser user, TrophyKey trophyKey);

        void QueueActivity(ActivityType activityType, string userId, string language = "en");
        Task QueueActivityAsync(ActivityType activityType, string userId, string language = "en");
        Task QueueActivityAsync(ActivityType activityType, string userId, int xp, string language = "en");
    }
}
