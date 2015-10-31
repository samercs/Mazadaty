using System;
using System.Collections.Generic;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services.Identity;
using Mzayad.Data;

namespace Mzayad.Services.Trophies
{
    public class WinAuctionTrophyEngine : TrophyEngine
    {
        private readonly UserService _userService;
        private readonly AuctionService _auctionService;

        public WinAuctionTrophyEngine(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
            _userService = new UserService(dataContextFactory);
            _auctionService = new AuctionService(dataContextFactory);
        }

        protected override IEnumerable<TrophyKey?> TryGetEarnedTrophies(ApplicationUser user)
        {
            //check win auction one time
            yield return CheckWinAuction1(user.Id);

            //check win 5 auctions
            yield return CheckWinAuction5(user.Id);
        }

        private TrophyKey? CheckWinAuction1(string userId)
        {
            var lastTime = _trophyService.GetLastEarnedTrophy(TrophyKey.WinAuction1, userId).Result;
            var auctions = _auctionService.CountAuctionsWon(userId, lastTime?.CreatedUtc).Result;
            if (auctions == 1)
            {
                return TrophyKey.WinAuction1;
            }
            return null;
        }

        private TrophyKey? CheckWinAuction5(string userId)
        {
            var lastTime = _trophyService.GetLastEarnedTrophy(TrophyKey.WinAuction5, userId).Result;
            var auctions = _auctionService.CountAuctionsWon(userId, lastTime?.CreatedUtc).Result;
            if (auctions == 5)
            {
                return TrophyKey.WinAuction5;
            }
            return null;
        }
    }
}
