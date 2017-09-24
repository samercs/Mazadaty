using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mazadaty.Models;
using Mazadaty.Models.Enum;
using Mazadaty.Services.Identity;
using Mazadaty.Data;

namespace Mazadaty.Services.Trophies
{
    public class AutoBidTrophyEngine : TrophyEngine
    {
        private readonly UserService _userService;
        private readonly AutoBidService _autoBidService;

        public AutoBidTrophyEngine(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
            _userService = new UserService(dataContextFactory);            
            _autoBidService = new AutoBidService(dataContextFactory);
        }

        protected async Task<IReadOnlyCollection<TrophyKey>> TryGetEarnedTrophies(ApplicationUser user)
        {
            return new List<TrophyKey>();

            ////check auto bid 10 times
            //yield return CheckAutoBid10(user.Id);

            ////check auto bid 25 times
            //yield return CheckAutoBid25(user.Id);

            ////check auto bid 50 times
            //yield return CheckAutoBid50(user.Id);

            ////check auto bid 100 times
            //yield return CheckAutoBid100(user.Id);

            ////check auto bid 250 times
            //yield return CheckAutoBid250(user.Id);

            ////check auto bid 500 times
            //yield return CheckAutoBid500(user.Id);

            ////check auto bid 1000 times
            //yield return CheckAutoBid1000(user.Id);

            ////check auto bid 2000 times
            //yield return CheckAutoBid2000(user.Id);

            ////check auto bid 5000 times
            //yield return CheckAutoBid5000(user.Id);
        }

        private TrophyKey? CheckAutoBid10(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.AutoBid10, userId).Result;
            var autoBids = _autoBidService.CountUserAutoBids(userId, lastTime?.CreatedUtc).Result;
            if (autoBids == 10)
            {
                return TrophyKey.AutoBid10;
            }
            return null;
        }

        private TrophyKey? CheckAutoBid25(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.AutoBid25, userId).Result;
            var autoBids = _autoBidService.CountUserAutoBids(userId, lastTime?.CreatedUtc).Result;
            if (autoBids == 25)
            {
                return TrophyKey.AutoBid25;
            }
            return null;
        }

        private TrophyKey? CheckAutoBid50(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.AutoBid50, userId).Result;
            var autoBids = _autoBidService.CountUserAutoBids(userId, lastTime?.CreatedUtc).Result;
            if (autoBids == 50)
            {
                return TrophyKey.AutoBid50;
            }
            return null;
        }

        private TrophyKey? CheckAutoBid100(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.AutoBid100, userId).Result;
            var autoBids = _autoBidService.CountUserAutoBids(userId, lastTime?.CreatedUtc).Result;
            if (autoBids == 100)
            {
                return TrophyKey.AutoBid100;
            }
            return null;
        }

        private TrophyKey? CheckAutoBid250(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.AutoBid250, userId).Result;
            var autoBids = _autoBidService.CountUserAutoBids(userId, lastTime?.CreatedUtc).Result;
            if (autoBids == 250)
            {
                return TrophyKey.AutoBid250;
            }
            return null;
        }

        private TrophyKey? CheckAutoBid500(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.AutoBid500, userId).Result;
            var autoBids = _autoBidService.CountUserAutoBids(userId, lastTime?.CreatedUtc).Result;
            if (autoBids == 500)
            {
                return TrophyKey.AutoBid500;
            }
            return null;
        }

        private TrophyKey? CheckAutoBid1000(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.AutoBid1000, userId).Result;
            var autoBids = _autoBidService.CountUserAutoBids(userId, lastTime?.CreatedUtc).Result;
            if (autoBids == 1000)
            {
                return TrophyKey.AutoBid1000;
            }
            return null;
        }

        private TrophyKey? CheckAutoBid2000(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.AutoBid2000, userId).Result;
            var autoBids = _autoBidService.CountUserAutoBids(userId, lastTime?.CreatedUtc).Result;
            if (autoBids == 2000)
            {
                return TrophyKey.AutoBid2000;
            }
            return null;
        }

        private TrophyKey? CheckAutoBid5000(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.AutoBid5000, userId).Result;
            var autoBids = _autoBidService.CountUserAutoBids(userId, lastTime?.CreatedUtc).Result;
            if (autoBids == 5000)
            {
                return TrophyKey.AutoBid5000;
            }
            return null;
        }
    }
}
