using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mzayad.Models.Extensions;

namespace Mzayad.Services.Trophies
{
    public class SubmitBidTrophyEngine : TrophyEngine, ITrophyEngine
    {
        private readonly BidService _bidService;
        private readonly IslamicCalendar _calendar;

        public SubmitBidTrophyEngine(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
            var islamicCalendarService = new IslamicCalendarService(dataContextFactory);
            _bidService = new BidService(dataContextFactory, null);

            _calendar = islamicCalendarService.GetByDate(DateTime.UtcNow.Date).Result;
        }

        public async Task<IEnumerable<TrophyKey>> TryGetEarnedTrophies(ApplicationUser user)
        {
            var userTrophies = await TrophyService.GetUserTrophies(user.Id);
            var trophyKeys = new List<TrophyKey>();

            trophyKeys.TryAdd(CheckBidOnNewYear(userTrophies));

            trophyKeys.TryAdd(CheckBidOnIslamicNewYear(userTrophies));
            trophyKeys.TryAdd(CheckBidOnEid(userTrophies));
            trophyKeys.TryAdd(CheckBidOnAnniversary(userTrophies, user));

            var bidStreak = await _bidService.GetConsecutiveBidDays(user.Id);
            trophyKeys.TryAdd(CheckBidStreak(userTrophies, bidStreak, 3, TrophyKey.BidDayStreak3));
            trophyKeys.TryAdd(CheckBidStreak(userTrophies, bidStreak, 7, TrophyKey.BidDayStreak7));
            trophyKeys.TryAdd(CheckBidStreak(userTrophies, bidStreak, 30, TrophyKey.BidDayStreak30));
            trophyKeys.TryAdd(CheckBidStreak(userTrophies, bidStreak, 90, TrophyKey.BidDayStreak90));
            trophyKeys.TryAdd(CheckBidStreak(userTrophies, bidStreak, 180, TrophyKey.BidDayStreak180));
            trophyKeys.TryAdd(CheckBidStreak(userTrophies, bidStreak, 3365, TrophyKey.BidDayStreak365));

            var bidCount = await _bidService.CountUserBids(user.Id);
            trophyKeys.TryAdd(CheckBidCount(userTrophies, 10, bidCount, TrophyKey.AutoBid10));
            trophyKeys.TryAdd(CheckBidCount(userTrophies, 25, bidCount, TrophyKey.AutoBid25));
            trophyKeys.TryAdd(CheckBidCount(userTrophies, 50, bidCount, TrophyKey.AutoBid50));
            trophyKeys.TryAdd(CheckBidCount(userTrophies, 100, bidCount, TrophyKey.AutoBid100));
            trophyKeys.TryAdd(CheckBidCount(userTrophies, 250, bidCount, TrophyKey.AutoBid250));
            trophyKeys.TryAdd(CheckBidCount(userTrophies, 500, bidCount, TrophyKey.AutoBid500));
            trophyKeys.TryAdd(CheckBidCount(userTrophies, 1000, bidCount, TrophyKey.AutoBid1000));
            trophyKeys.TryAdd(CheckBidCount(userTrophies, 2000, bidCount, TrophyKey.AutoBid2000));
            trophyKeys.TryAdd(CheckBidCount(userTrophies, 5000, bidCount, TrophyKey.AutoBid5000));

            return trophyKeys;
        }

        private static TrophyKey? CheckBidOnNewYear(IEnumerable<UserTrophy> userTrophies)
        {
            if (DateTime.Now.Month != 1 || DateTime.Now.Day != 1)
            {
                return null;
            }

            var userTrophy = userTrophies.GetLastEarned(TrophyKey.BidOnNewYear);
            if (userTrophy.CreatedUtc.Year == DateTime.Today.Year)
            {
                return null;
            }

            return TrophyKey.BidOnNewYear;
        }

        private TrophyKey? CheckBidOnIslamicNewYear(IEnumerable<UserTrophy> userTrophies)
        {
            if (_calendar == null || DateTime.Today != _calendar.NewYear.Date)
            {
                return null;
            }

            var userTrophy = userTrophies.GetLastEarned(TrophyKey.BidOnIslamicNewYear);
            if (userTrophy.CreatedUtc.Year == DateTime.Today.Year)
            {
                return null;
            }

            return TrophyKey.BidOnNewYear;
        }

        private TrophyKey? CheckBidOnEid(IEnumerable<UserTrophy> userTrophies)
        {
            if (_calendar == null || !(DateTime.Today >= _calendar.EidAdhaFrom.Date && DateTime.Today <= _calendar.EidAdhaTo.Date))
            {
                return null;
            }

            var userTrophy = userTrophies.GetLastEarned(TrophyKey.BidOnIslamicNewYear);
            if (userTrophy.CreatedUtc.Year == DateTime.Today.Year)
            {
                return null;
            }

            return TrophyKey.BidOnEid;
        }

        private static TrophyKey? CheckBidOnAnniversary(IEnumerable<UserTrophy> userTrophies, ApplicationUser user)
        {
            if (user.CreatedUtc.Year == DateTime.Today.Year || user.CreatedUtc.Day != DateTime.Today.Day || user.CreatedUtc.Month != DateTime.Today.Month)
            {
                return null;
            }

            var userTrophy = userTrophies.GetLastEarned(TrophyKey.BidOnAnniversary);
            if (userTrophy != null)
            {
                if (userTrophy.CreatedUtc.Date == DateTime.Today.Date || userTrophy.CreatedUtc.Year == user.CreatedUtc.Date.Year)
                {
                    return null;
                }
            }

            return TrophyKey.BidOnAnniversary;
        }

        private static TrophyKey? CheckBidStreak(IEnumerable<UserTrophy> userTrophies, int actualBidStreak, int expectedBidStreak, TrophyKey trophyKey)
        {
            if (actualBidStreak < expectedBidStreak)
            {
                return null;
            }

            var lastEarned = userTrophies.GetLastEarned(trophyKey);
            if (lastEarned != null && lastEarned.WasEarnedToday())
            {
                return null;
            }

            return trophyKey;
        }

        private static TrophyKey? CheckBidCount(IEnumerable<UserTrophy> userTrophies, int actualBidCount, int targetBidCount, TrophyKey trophyKey)
        {
            var wasEarned = userTrophies.WasEarned(trophyKey);
            if (!wasEarned && actualBidCount >= targetBidCount)
            {
                return trophyKey;
            }

            return null;
        }
    }
}
