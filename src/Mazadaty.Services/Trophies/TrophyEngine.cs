using Mazadaty.Data;
using Mazadaty.Models.Enum;
using System;

namespace Mazadaty.Services.Trophies
{
    public abstract class TrophyEngine
    {
        protected readonly TrophyService TrophyService;

        protected TrophyEngine(IDataContextFactory dataContextFactory)
        {
            TrophyService = new TrophyService(dataContextFactory);
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
    }
}
