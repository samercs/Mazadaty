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
        public override IEnumerable<TrophyKey> GetEarnedTrophies(ApplicationUser user)
        {
            return TryGetEarnedTrophies(user)
                .Where(i => i.HasValue)
                .Select(i => i.Value);
        }

        private IEnumerable<TrophyKey?> TryGetEarnedTrophies(ApplicationUser user)
        {
            //Check Return to Mzayad after 30 days of inactivity;
            yield return CheckReturnAfterInactivity(user);
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
    }
}
