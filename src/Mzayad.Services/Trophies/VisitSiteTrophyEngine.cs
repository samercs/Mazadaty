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
    }
}
