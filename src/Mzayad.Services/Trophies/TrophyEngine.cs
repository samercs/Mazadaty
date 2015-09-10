using System;
using System.Linq;
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

        public IEnumerable<TrophyKey> GetEarnedTrophies(ApplicationUser user)
        {
            return TryGetEarnedTrophies(user)
                .Where(i => i.HasValue)
                .Select(i => i.Value);
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
        
        protected abstract IEnumerable<TrophyKey?> TryGetEarnedTrophies(ApplicationUser user);
    }
}