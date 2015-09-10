using System;
using System.Collections.Generic;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Data;

namespace Mzayad.Services.Trophies
{
    public abstract class TrophyEngine
    {
        protected readonly TrophyService _trophyService;

        public TrophyEngine(IDataContextFactory dataContextFactory)
        {
            _trophyService = new TrophyService(dataContextFactory);
        }

        protected bool GainTrophyToday(TrophyKey key, string userId)
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

        public abstract IEnumerable<TrophyKey> GetEarnedTrophies(ApplicationUser user);
    }
}