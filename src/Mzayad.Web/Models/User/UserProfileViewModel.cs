using Mzayad.Models;
using Mzayad.Services;
using System;
using System.Collections.Generic;

namespace Mzayad.Web.Models.User
{
    public class UserProfileViewModel
    {
        public ApplicationUser User { get; private set; }
        public IReadOnlyCollection<ApplicationUser> Friends { get; set; }
        public IReadOnlyCollection<Trophy> Trophies { get; set; }

        public Level CurrentLevel { get; private set; }
        public Level NextLevel { get; private set; }
        public int LevelPercentage { get; private set; }

        public bool AreFriends { get; set; }
        public bool SentFriendRequestBefore { get; set; }

        public bool Me { get; set; }

        public UserProfileViewModel(ApplicationUser user)
        {
            User = user;

            CurrentLevel = LevelService.GetLevelByXp(user.Xp);
            NextLevel = LevelService.GetLevel(CurrentLevel.Index + 1);

            var percentage = (double)user.Xp / NextLevel.XpRequired * 100d;

            LevelPercentage = (int)Math.Floor(percentage);
        }
    }
}