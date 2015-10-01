using System;
using System.Linq;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services.Identity;
using System.Collections.Generic;

namespace Mzayad.Services.Trophies
{
    public class SubmitBidTrophyEngine : TrophyEngine
    {
        private readonly UserService _userService;
        private readonly BidService _bidService;

        private readonly IslamicCalendar _calendar;

        public SubmitBidTrophyEngine(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
            _userService = new UserService(dataContextFactory);

            var islamicCalendarService = new IslamicCalendarService(dataContextFactory);
            _bidService = new BidService(dataContextFactory);

            _calendar = islamicCalendarService.GetByDate(DateTime.UtcNow.Date).Result;
        }

        protected override IEnumerable<TrophyKey?> TryGetEarnedTrophies(ApplicationUser user)
        {
            yield return CheckBidOnNewYear(user.Id);
            yield return CheckBidOnIslamicNewYear(user.Id);
            yield return CheckBidOnEid(user.Id);
            yield return CheckBidOnAnniversary(user);

            //Bid 3 days in a row
            yield return CheckBid3DaysInRow(user.Id);

            //Bid 7 days in a row
            yield return CheckBid7DaysInRow(user.Id);

            //Bid 30 days in a row
            yield return CheckBid30DaysInRow(user.Id);

            //Bid 90 days in a row
            yield return CheckBid90DaysInRow(user.Id);

            //Bid 180 days in a row
            yield return CheckBid180DaysInRow(user.Id);

            //Bid 365 days in a row
            yield return CheckBid365DaysInRow(user.Id);

            //Bid 10 Times
            yield return CheckBid10(user.Id);

            //Bid 50 Times
            yield return CheckBid50(user.Id);

            //Bid 100 Times
            yield return CheckBid100(user.Id);

            //Bid 250 Times
            yield return CheckBid250(user.Id);
        }

        private TrophyKey? CheckBidOnNewYear(string userId)
        {
            if (DateTime.Now.Month != 1 || DateTime.Now.Day != 1)
            {
                return null;
            }
            var userTrophy = _trophyService.GetLastEarnedTrophy(TrophyKey.BidOnNewYear, userId).Result;
            if (userTrophy.CreatedUtc.Year == DateTime.Now.Year)
            {
                return null;
            }
            return TrophyKey.BidOnNewYear;
        }

        private TrophyKey? CheckBidOnIslamicNewYear(string userId)
        {
            if (_calendar == null)
            {
                return null;
            }
            var userTrophy = _trophyService.GetLastEarnedTrophy(TrophyKey.BidOnIslamicNewYear, userId).Result;
            if (userTrophy.CreatedUtc.Subtract(_calendar.NewYear.Date).TotalDays < 354) // 354 is total days in Islamic(Hijri) year
            {
                return null;
            }
            return TrophyKey.BidOnNewYear;
        }

        private TrophyKey? CheckBidOnEid(string userId)
        {
            if (_calendar == null)
            {
                return null;
            }
            var userTrophy = _trophyService.GetLastEarnedTrophy(TrophyKey.BidOnEid, userId).Result;
            if ((userTrophy.CreatedUtc.Date >= _calendar.EidAdhaFrom.Date && userTrophy.CreatedUtc.Date <= _calendar.EidAdhaTo.Date)
                || (userTrophy.CreatedUtc.Date >= _calendar.EidFetrFrom.Date && userTrophy.CreatedUtc.Date <= _calendar.EidFetrTo.Date))
            {
                return null;
            }
            return TrophyKey.BidOnEid;
        }

        private TrophyKey? CheckBidOnAnniversary(ApplicationUser user)
        {
            if (user.CreatedUtc.Day != DateTime.Now.Day || user.CreatedUtc.Month != DateTime.Now.Month)
            {
                return null;
            }
            var userTrophy = _trophyService.GetLastEarnedTrophy(TrophyKey.BidOnAnniversary, user.Id).Result;
            if (userTrophy != null)
            {
                if (userTrophy.CreatedUtc.Date == DateTime.Now.Date || userTrophy.CreatedUtc.Year == user.CreatedUtc.Date.Year)
                {
                    return null;
                }
            }
            return TrophyKey.BidOnAnniversary;
        }

        private TrophyKey? CheckBid3DaysInRow(string userId)
        {
            var streak = _bidService.GetConsecutiveBidDays(userId).Result;
            if (streak < 3)
            {
                return null;
            }

            if (!GainTrophyToday(TrophyKey.BidDayStreak3, userId))
            {
                return TrophyKey.BidDayStreak3;
            }
            return null;
        }

        private TrophyKey? CheckBid7DaysInRow(string userId)
        {
            var streak = _bidService.GetConsecutiveBidDays(userId).Result;
            if (streak < 7)
            {
                return null;
            }

            if (!GainTrophyToday(TrophyKey.BidDayStreak7, userId))
            {
                return TrophyKey.BidDayStreak7;
            }
            return null;
        }

        private TrophyKey? CheckBid30DaysInRow(string userId)
        {
            var streak = _bidService.GetConsecutiveBidDays(userId).Result;
            if (streak < 30)
            {
                return null;
            }

            if (!GainTrophyToday(TrophyKey.BidDayStreak30, userId))
            {
                return TrophyKey.BidDayStreak30;
            }
            return null;
        }

        private TrophyKey? CheckBid90DaysInRow(string userId)
        {
            var streak = _bidService.GetConsecutiveBidDays(userId).Result;
            if (streak < 90)
            {
                return null;
            }

            if (!GainTrophyToday(TrophyKey.BidDayStreak90, userId))
            {
                return TrophyKey.BidDayStreak90;
            }
            return null;
        }

        private TrophyKey? CheckBid180DaysInRow(string userId)
        {
            var streak = _bidService.GetConsecutiveBidDays(userId).Result;
            if (streak < 180)
            {
                return null;
            }

            if (!GainTrophyToday(TrophyKey.BidDayStreak180, userId))
            {
                return TrophyKey.BidDayStreak180;
            }
            return null;
        }

        private TrophyKey? CheckBid365DaysInRow(string userId)
        {
            var streak = _bidService.GetConsecutiveBidDays(userId).Result;
            if (streak < 365)
            {
                return null;
            }

            if (!GainTrophyToday(TrophyKey.BidDayStreak365, userId))
            {
                return TrophyKey.BidDayStreak365;
            }
            return null;
        }

        private TrophyKey? CheckBid10(string userId)
        {
            var lastTime = _trophyService.GetLastEarnedTrophy(TrophyKey.Bid10, userId).Result;
            var bids = _bidService.CountUserBids(userId, lastTime?.CreatedUtc).Result;
            if (bids == 10)
            {
                return TrophyKey.Bid10;
            }
            return null;
        }

        private TrophyKey? CheckBid50(string userId)
        {
            var lastTime = _trophyService.GetLastEarnedTrophy(TrophyKey.Bid50, userId).Result;
            var bids = _bidService.CountUserBids(userId, lastTime?.CreatedUtc).Result;
            if (bids == 50)
            {
                return TrophyKey.Bid50;
            }
            return null;
        }

        private TrophyKey? CheckBid100(string userId)
        {
            var lastTime = _trophyService.GetLastEarnedTrophy(TrophyKey.Bid100, userId).Result;
            var bids = _bidService.CountUserBids(userId, lastTime?.CreatedUtc).Result;
            if (bids == 100)
            {
                return TrophyKey.Bid100;
            }
            return null;
        }

        private TrophyKey? CheckBid250(string userId)
        {
            var lastTime = _trophyService.GetLastEarnedTrophy(TrophyKey.Bid250, userId).Result;
            var bids = _bidService.CountUserBids(userId, lastTime?.CreatedUtc).Result;
            if (bids == 250)
            {
                return TrophyKey.Bid250;
            }
            return null;
        }
    }
}
