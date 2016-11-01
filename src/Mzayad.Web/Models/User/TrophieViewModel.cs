using System;

namespace Mzayad.Web.Models.User
{
    public class TrophieViewModel
    {
        public string TrophyName { get; set; }

        public string TrophyDescription { get; set; }

        public string IconUrl { get; set; }

        public DateTime? AwardDate { get; set; }

        public int? XpEarned { get; set; }

        public bool Earned { get; set; }
    }
}