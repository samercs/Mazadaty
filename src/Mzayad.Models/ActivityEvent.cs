using Mzayad.Models.Enums;

namespace Mzayad.Models
{
    public class ActivityEvent : ModelBase
    {
        public int ActivityEventId { get; set; }
        public ActivityType Type { get; set; }
        public string UserId { get; set; }

        public int XP { get; set; }
        public string Language { get; set; }
    }
}
