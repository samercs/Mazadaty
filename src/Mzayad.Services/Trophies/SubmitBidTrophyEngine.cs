using System;
using System.Linq;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services.Identity;
using System.Collections.Generic;

namespace Mzayad.Services.Trophies
{
    public class SubmitBidTrophyEngine : ITrophyEngine
    {
        private readonly UserService _userService;
        private readonly TrophyService _trophyService;
        private readonly IslamicCalendarService _islamicCalendarService;
        private readonly BidService _bidService;

        private IslamicCalendar calendar;

        public SubmitBidTrophyEngine(IDataContextFactory dataContextFactory)
        {
            _userService = new UserService(dataContextFactory);
            _trophyService = new TrophyService(dataContextFactory);
            _islamicCalendarService = new IslamicCalendarService(dataContextFactory);
            _bidService = new BidService(dataContextFactory);

            calendar = _islamicCalendarService.GetByDate(DateTime.UtcNow.Date).Result;
        }

        public IEnumerable<Trophy> EvaluateActivityForPossibleTrophy(ApplicationUser user)
        {
            // Bid on New Year
            yield return CheckBidOnNewYear(user.Id);

            // Bid on New Islamic Year
            yield return CheckBidOnIslamicNewYear(user.Id);

            // Bid on Eid
            yield return CheckBidOnEid(user.Id);

            //Bid 3 days in a row
            yield return CheckBid3DaysInRow(user.Id);
        }

        private Trophy CheckBidOnNewYear(string userId)
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
            return new Trophy() { TrophyId = (int)TrophyKey.BidOnNewYear };
        }
        private Trophy CheckBidOnIslamicNewYear(string userId)
        {
            if (calendar == null)
            {
                return null;
            }
            var userTrophy = _trophyService.GetLastEarnedTrophy(TrophyKey.BidOnIslamicNewYear, userId).Result;
            if (userTrophy.CreatedUtc.Subtract(calendar.NewYear.Date).TotalDays < 354) // 354 is total days in Islamic(Hijri) year
            {
                return null;
            }
            return new Trophy() { TrophyId = (int)TrophyKey.BidOnNewYear };
        }
        private Trophy CheckBidOnEid(string userId)
        {
            if (calendar == null)
            {
                return null;
            }
            var userTrophy = _trophyService.GetLastEarnedTrophy(TrophyKey.BidOnEid, userId).Result;
            if ((userTrophy.CreatedUtc.Date >= calendar.EidAdhaFrom.Date && userTrophy.CreatedUtc.Date <= calendar.EidAdhaTo.Date)
                || (userTrophy.CreatedUtc.Date >= calendar.EidFetrFrom.Date && userTrophy.CreatedUtc.Date <= calendar.EidFetrTo.Date))
            {
                return null;
            }
            return new Trophy() { TrophyId = (int)TrophyKey.BidOnEid };
        }
        private Trophy CheckBidOnAnniversary(string userId)
        {
            var user = _userService.GetUserById(userId).Result;
            if (user == null)
            {
                return null;
            }
            if (user.CreatedUtc.Day != DateTime.Now.Day || user.CreatedUtc.Month != DateTime.Now.Month)
            {
                return null;
            }
            var userTrophy = _trophyService.GetLastEarnedTrophy(TrophyKey.BidOnAnniversary, userId).Result;
            if (userTrophy != null)
            {
                if (userTrophy.CreatedUtc.Date == DateTime.Now.Date || userTrophy.CreatedUtc.Year == user.CreatedUtc.Date.Year)
                {
                    return null;
                }
            }
            return new Trophy() { TrophyId = (int)TrophyKey.BidOnAnniversary };
        }
        private Trophy CheckBid3DaysInRow(string userId)
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
                return new Trophy() { TrophyId = (int)TrophyKey.BidDayStreak3 };
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
