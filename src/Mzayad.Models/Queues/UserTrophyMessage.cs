using Mzayad.Models.Enum;

namespace Mzayad.Models.Queues
{
    public class UserTrophyMessage : QueueMessageBase
    {
        public ApplicationUser User { get; set; }
        public TrophyKey TrophyKey { get; set; }

        public UserTrophyMessage(ApplicationUser user, TrophyKey trophyKey)
        {
            User = user;
            TrophyKey = trophyKey;
        }
    }
}
