using System.Threading.Tasks;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Models.Enums;

namespace Mzayad.Services.Queues
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