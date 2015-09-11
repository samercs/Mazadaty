using System;
using System.Linq;
using System.Collections.Generic;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Data;

namespace Mzayad.Services.Trophies
{
    public class VisitSiteTrophyEngine : TrophyEngine
    {
        private readonly SessionLogService _sessionLogService;
        public VisitSiteTrophyEngine(IDataContextFactory dataContextFactory)
            :base(dataContextFactory)
        {
            _sessionLogService = new SessionLogService(dataContextFactory);
        }

        protected override IEnumerable<TrophyKey?> TryGetEarnedTrophies(ApplicationUser user)
        {
            //Check Return to Mzayad after 30 days of inactivity;
            yield return CheckReturnAfterInactivity(user);

            //Check visit site 3 days in row
            yield return CheckVisit3ConsecutiveDays(user.Id);

            //Check visit site 7 days in row
            yield return CheckVisit7ConsecutiveDays(user.Id);

            //Check visit site 30 days in row
            yield return CheckVisit30ConsecutiveDays(user.Id);

            //Check visit site 90 days in row
            yield return CheckVisit90ConsecutiveDays(user.Id);

            //Check visit site 180 days in row
            yield return CheckVisit180ConsecutiveDays(user.Id);

            //Check visit site 365 days in row
            yield return CheckVisit365ConsecutiveDays(user.Id);
        }

        private TrophyKey? CheckReturnAfterInactivity(ApplicationUser user)
        {
            var lastSession = _sessionLogService.GetLastVisitBeforeToday(user.Id);
            if(lastSession != null || lastSession.CreatedUtc.Date.AddDays(30) <= DateTime.UtcNow.Date)
            {
                if(!GainTrophyToday(TrophyKey.ReturnAfterInactivity , user.Id))
                {
                    return TrophyKey.ReturnAfterInactivity;
                }
            }
            return null;
        }

        private TrophyKey? CheckVisit3ConsecutiveDays(string userId)
        {
            var streak = _sessionLogService.GetConsecutiveVisitDays(userId).Result;
            if (streak < 3)
            {
                return null;
            }

            if (!GainTrophyToday(TrophyKey.VisitDayStreak3, userId))
            {
                return TrophyKey.VisitDayStreak3;
            }
            return null;
        }

        private TrophyKey? CheckVisit7ConsecutiveDays(string userId)
        {
            var streak = _sessionLogService.GetConsecutiveVisitDays(userId).Result;
            if (streak < 7)
            {
                return null;
            }

            if (!GainTrophyToday(TrophyKey.VisitDayStreak7, userId))
            {
                return TrophyKey.VisitDayStreak7;
            }
            return null;
        }

        private TrophyKey? CheckVisit30ConsecutiveDays(string userId)
        {
            var streak = _sessionLogService.GetConsecutiveVisitDays(userId).Result;
            if (streak < 30)
            {
                return null;
            }

            if (!GainTrophyToday(TrophyKey.VisitDayStreak30, userId))
            {
                return TrophyKey.VisitDayStreak30;
            }
            return null;
        }

        private TrophyKey? CheckVisit90ConsecutiveDays(string userId)
        {
            var streak = _sessionLogService.GetConsecutiveVisitDays(userId).Result;
            if (streak < 90)
            {
                return null;
            }

            if (!GainTrophyToday(TrophyKey.VisitDayStreak90, userId))
            {
                return TrophyKey.VisitDayStreak90;
            }
            return null;
        }

        private TrophyKey? CheckVisit180ConsecutiveDays(string userId)
        {
            var streak = _sessionLogService.GetConsecutiveVisitDays(userId).Result;
            if (streak < 180)
            {
                return null;
            }

            if (!GainTrophyToday(TrophyKey.VisitDayStreak180, userId))
            {
                return TrophyKey.VisitDayStreak180;
            }
            return null;
        }

        private TrophyKey? CheckVisit365ConsecutiveDays(string userId)
        {
            var streak = _sessionLogService.GetConsecutiveVisitDays(userId).Result;
            if (streak < 365)
            {
                return null;
            }

            if (!GainTrophyToday(TrophyKey.VisitDayStreak365, userId))
            {
                return TrophyKey.VisitDayStreak365;
            }
            return null;
        }
    }
}
