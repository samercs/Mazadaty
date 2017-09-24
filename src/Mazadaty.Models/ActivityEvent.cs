using Mazadaty.Models.Enums;
using OrangeJetpack.Base.Data;

namespace Mazadaty.Models
{
    public class ActivityEvent : EntityBase
    {
        public int ActivityEventId { get; set; }
        public ActivityType Type { get; set; }
        public string UserId { get; set; }

        public int XP { get; set; }
        public string Language { get; set; }
    }
}
