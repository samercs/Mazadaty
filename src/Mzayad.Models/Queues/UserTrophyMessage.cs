using Mzayad.Models.Enum;

namespace Mzayad.Models.Queues
{
    public class UserTrophyMessage : QueueMessageBase
    {
        public string UserId { get; set; }
        public TrophyKey TrophyKey { get; set; }

        public UserTrophyMessage(string userId, TrophyKey trophyKey)
        {
            UserId = userId;
            TrophyKey = trophyKey;
        }
    }
}
