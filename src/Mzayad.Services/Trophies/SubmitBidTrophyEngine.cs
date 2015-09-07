using System;
using System.Linq;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services.Identity;
using System.Collections.Generic;
using System.Linq;

namespace Mzayad.Services.Trophies
{
    public class SubmitBidTrophyEngine : ITrophyEngine
    {
        private readonly UserService _userService;
        private readonly TrophyService _trophyService;
        private readonly BidService _bidService;

        private readonly IslamicCalendar _calendar;

        public SubmitBidTrophyEngine(IDataContextFactory dataContextFactory)
        {
            _userService = new UserService(dataContextFactory);
            _trophyService = new TrophyService(dataContextFactory);

            var islamicCalendarService = new IslamicCalendarService(dataContextFactory);
            _bidService = new BidService(dataContextFactory);

            _calendar = islamicCalendarService.GetByDate(DateTime.UtcNow.Date).Result;
        }

        public IEnumerable<TrophyKey> GetEarnedTrophies(ApplicationUser user)
        {
            return TryGetEarnedTrophies(user)
                .Where(i => i.HasValue)
                .Select(i => i.Value);
        }

        private IEnumerable<TrophyKey?> TryGetEarnedTrophies(ApplicationUser user)
        {
            yield return CheckBidOnNewYear(user.Id);
            yield return CheckBidOnIslamicNewYear(user.Id);
            yield return CheckBidOnEid(user.Id);
			yield return CheckBidOnAnniversary(user);

            //Bid 3 days in a row
			yield return CheckBid3DaysInRow(user.Id);

            //Bid 7 days in a row
            yield return CheckBid7DaysInRow(user.Id);
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
            var bids = _bidService.GetByUser(userId, DateTime.Now.AddDays(-2)).Result;
            for (var x = -2; x < 0; x++)
            {
                if (!bids.Any(i => i.CreatedUtc.Date == DateTime.Now.AddDays(x).Date))
                {
                    return null;
                }
            }
            if (!GainBidInRowTrophyBefore(TrophyKey.BidDayStreak3, userId))
            {
                return TrophyKey.BidDayStreak3;
            }
            return null;
        }
        private Trophy CheckBid7DaysInRow(string userId)
        {
            var bids = _bidService.GetByUser(userId, DateTime.Now.AddDays(-6)).Result;
            for (var x = -6; x < 0; x++)
            {
                if (!bids.Any(i => i.CreatedUtc.Date == DateTime.Now.AddDays(x).Date))
                {
                    return null;
                }
            }
            if (!GainBidInRowTrophyBefore(TrophyKey.BidDayStreak7, userId))
            {
                return new Trophy() { TrophyId = (int)TrophyKey.BidDayStreak7 };
            }
            return null;
        }

        private bool GainBidInRowTrophyBefore(TrophyKey key, string userId)
        {
            var userTrophy = _trophyService.GetLastEarnedTrophy(key, userId).Result;
            if (userTrophy == null)
            {
                return false;
            }
            if (userTrophy.CreatedUtc.Date == DateTime.Now.Date)
            {
                return true;
            }
            return false;
        }
    }
}
