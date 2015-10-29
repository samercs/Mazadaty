﻿using System;
using System.Collections.Generic;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services.Identity;
using Mzayad.Data;

namespace Mzayad.Services.Trophies
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

        protected override IEnumerable<TrophyKey?> TryGetEarnedTrophies(ApplicationUser user)
        {
            //check auto bid 10 times
            yield return CheckAutoBid10(user.Id);

            //check auto bid 25 times
            yield return CheckAutoBid25(user.Id);
        }

        private TrophyKey? CheckAutoBid10(string userId)
        {
            var lastTime = _trophyService.GetLastEarnedTrophy(TrophyKey.AutoBid10, userId).Result;
            var autoBids = _autoBidService.CountUserAutoBids(userId, lastTime?.CreatedUtc).Result;
            if (autoBids == 10)
            {
                return TrophyKey.AutoBid10;
            }
            return null;
        }

        private TrophyKey? CheckAutoBid25(string userId)
        {
            var lastTime = _trophyService.GetLastEarnedTrophy(TrophyKey.AutoBid25, userId).Result;
            var autoBids = _autoBidService.CountUserAutoBids(userId, lastTime?.CreatedUtc).Result;
            if (autoBids == 25)
            {
                return TrophyKey.AutoBid25;
            }
            return null;
        }
    }
}
