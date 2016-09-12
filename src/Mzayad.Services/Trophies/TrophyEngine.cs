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
        protected readonly TrophyService TrophyService;

        protected TrophyEngine(IDataContextFactory dataContextFactory)
        {
            TrophyService = new TrophyService(dataContextFactory);
        }

        public IEnumerable<TrophyKey> GetEarnedTrophies(ApplicationUser user)
        {
            return TryGetEarnedTrophies(user)
                .Where(i => i.HasValue)
                .Select(i => i.Value);
        }

        protected bool GainTrophyToday(TrophyKey key, string userId)
        {
            var userTrophy = TrophyService.GetLastEarnedTrophy(key, userId).Result;
            if (userTrophy == null)
            {
                return false;
            }

            return userTrophy.CreatedUtc.Date == DateTime.Now.Date;
        }
        
        protected abstract IEnumerable<TrophyKey?> TryGetEarnedTrophies(ApplicationUser user);
    }
}