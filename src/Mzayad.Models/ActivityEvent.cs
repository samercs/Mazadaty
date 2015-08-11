﻿using Mzayad.Models.Enums;

namespace Mzayad.Models
{
    public class ActivityEvent : ModelBase
    {
        public int ActivityEventId { get; set; }
        public ActivityType Type { get; set; }
        public string UserId { get; set; }
    }
}
