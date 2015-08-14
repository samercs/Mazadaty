using System;
using System.Threading.Tasks;
using Mzayad.Models.Enum;
using Mzayad.Services;

namespace Mzayad.Web.Core.Trophies
{
    public class BidOnNewYearTrophyChecker : TrophiesChecker
    {
        public BidOnNewYearTrophyChecker(TrophyService trophyService)
            : base(trophyService)
        {
        }

        public override async Task<bool> Check(string userId)
        {
            if (DateTime.Now.Month != 1 || DateTime.Now.Day != 1)
            {
                return false;
            }
            var userTrophy = await TrophyService.GetLastEarnedTrophy(TrophyKey.BidOnNewYear, userId);
            return userTrophy.CreatedUtc.Year == DateTime.Now.Year;
        }
    }
}