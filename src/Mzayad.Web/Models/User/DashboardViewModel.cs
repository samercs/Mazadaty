using System;
using System.Collections.Generic;
using Mzayad.Models;
using Mzayad.Services;

namespace Mzayad.Web.Models.User
{
    public class DashboardViewModel
    {
        public ApplicationUser User { get; private set; }
        public IReadOnlyCollection<Bid> Bids { get; set; }
        public IReadOnlyCollection<Trophy> Trophies { get; set; }
        public IReadOnlyCollection<Auction> Auctions { get; set; } 
        public IReadOnlyCollection<Mzayad.Models.WishList> WishLists { get; set; } 

        public Level CurrentLevel { get; private set; }
        public Level NextLevel { get; private set; }
        public int LevelPercentage { get; private set; }

        public DashboardViewModel(ApplicationUser user)
        {
            User = user;

            CurrentLevel = LevelService.GetLevelByXp(user.Xp);
            NextLevel = LevelService.GetLevel(CurrentLevel.Index + 1);

            double nextXp = NextLevel.XpRequired;
            
            LevelPercentage = (int)Math.Floor((nextXp - user.Xp) / nextXp * 100d);
        }
    }
}
