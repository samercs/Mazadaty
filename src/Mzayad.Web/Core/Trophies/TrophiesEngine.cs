using System;
using System.Linq;
using Mzayad.Models.Enum;
using Mzayad.Services;

namespace Mzayad.Web.Core.Trophies
{
    public class TrophiesEngine
    {
        private readonly TrophyService _trophyService;

        public TrophiesEngine(TrophyService trophyService)
        {
            _trophyService = trophyService;
        }

        public async void EarnTrophy(string userId)
        {
            foreach (var key in from key in Enum.GetValues(typeof(TrophyKey)).Cast<TrophyKey>()
                                let checker = TrophiesChecker.CreateInstance(key, _trophyService)
                                where checker.Check(userId).Result select key)
            {
                await _trophyService.AddToUser((int)key, userId);
            }
        }
    }
}
