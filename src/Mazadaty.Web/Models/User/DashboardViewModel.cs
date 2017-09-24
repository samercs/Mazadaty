using Mazadaty.Models;
using Mazadaty.Services;
using System;
using System.Collections.Generic;

namespace Mazadaty.Web.Models.User
{
    public class DashboardViewModel
    {
        public ApplicationUser User { get; private set; }
        public IReadOnlyCollection<ApplicationUser> Friends { get; set; }
        public IReadOnlyCollection<Bid> Bids { get; set; }
        public IReadOnlyCollection<Trophy> Trophies { get; set; }
        public IReadOnlyCollection<Auction> Auctions { get; set; }
        public IReadOnlyCollection<Mazadaty.Models.WishList> WishLists { get; set; }
        public string PrizeUrl { get; set; }

        public Level CurrentLevel { get; private set; }
        public Level NextLevel { get; private set; }
        public int LevelPercentage { get; private set; }

        public DashboardViewModel(ApplicationUser user)
        {
            User = user;

            CurrentLevel = LevelService.GetLevelByXp(user.Xp);
            NextLevel = LevelService.GetLevel(CurrentLevel.Index + 1);

            var percentage = (double)user.Xp / NextLevel.XpRequired * 100d;

            LevelPercentage = (int)Math.Floor(percentage);
        }
    }
}
