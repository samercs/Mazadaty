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

            //check win 10 auctions
            yield return CheckWinAuction10(user.Id);

            //check win 20 auctions
            yield return CheckWinAuction20(user.Id);

            //check win 50 auctions
            yield return CheckWinAuction50(user.Id);

            //check win 100 auctions
            yield return CheckWinAuction100(user.Id);

            //check win auctions 3 days in row
            yield return CheckWinAuction3DaysInRow(user.Id);

            //check win auctions 7 days in row
            yield return CheckWinAuction7DaysInRow(user.Id);
        }

        private TrophyKey? CheckWinAuction1(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.WinAuction1, userId).Result;
            var auctions = _auctionService.CountAuctionsWon(userId, lastTime?.CreatedUtc).Result;
            if (auctions == 1)
            {
                return TrophyKey.WinAuction1;
            }
            return null;
        }

        private TrophyKey? CheckWinAuction5(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.WinAuction5, userId).Result;
            var auctions = _auctionService.CountAuctionsWon(userId, lastTime?.CreatedUtc).Result;
            if (auctions == 5)
            {
                return TrophyKey.WinAuction5;
            }
            return null;
        }

        private TrophyKey? CheckWinAuction10(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.WinAuction10, userId).Result;
            var auctions = _auctionService.CountAuctionsWon(userId, lastTime?.CreatedUtc).Result;
            if (auctions == 10)
            {
                return TrophyKey.WinAuction10;
            }
            return null;
        }

        private TrophyKey? CheckWinAuction20(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.WinAuction20, userId).Result;
            var auctions = _auctionService.CountAuctionsWon(userId, lastTime?.CreatedUtc).Result;
            if (auctions == 20)
            {
                return TrophyKey.WinAuction20;
            }
            return null;
        }

        private TrophyKey? CheckWinAuction50(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.WinAuction50, userId).Result;
            var auctions = _auctionService.CountAuctionsWon(userId, lastTime?.CreatedUtc).Result;
            if (auctions == 50)
            {
                return TrophyKey.WinAuction50;
            }
            return null;
        }

        private TrophyKey? CheckWinAuction100(string userId)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.WinAuction100, userId).Result;
            var auctions = _auctionService.CountAuctionsWon(userId, lastTime?.CreatedUtc).Result;
            if (auctions == 100)
            {
                return TrophyKey.WinAuction100;
            }
            return null;
        }

        private TrophyKey? CheckWinAuction3DaysInRow(string userId)
        {
            var streak = _auctionService.GetConsecutiveWonDays(userId).Result;
            if (streak == 3)
            {
                if (!GainTrophyToday(TrophyKey.WinDayStreak3, userId))
                {
                    return TrophyKey.WinDayStreak3;
                }
            }
            return null;
        }

        private TrophyKey? CheckWinAuction7DaysInRow(string userId)
        {
            var streak = _auctionService.GetConsecutiveWonDays(userId).Result;
            if (streak == 7)
            {
                if (!GainTrophyToday(TrophyKey.WinDayStreak7, userId))
                {
                    return TrophyKey.WinDayStreak7;
                }
            }
            return null;
        }

        private TrophyKey? CheckWinAuction30DaysInRow(string userId)
        {
            var streak = _auctionService.GetConsecutiveWonDays(userId).Result;
            if (streak == 30)
            {
                if (!GainTrophyToday(TrophyKey.WinDayStreak30, userId))
                {
                    return TrophyKey.WinDayStreak30;
                }
            }
            return null;
        }
    }
}
