using System.Threading.Tasks;
using Mazadaty.Models.Enum;

namespace Mazadaty.Services.Trophies
{
    public abstract class TrophiesChecker
    {
        protected readonly TrophyService TrophyService;

        protected TrophiesChecker(TrophyService trophyService)
        {
            TrophyService = trophyService;
        }
        public abstract Task<bool> Check(string userId);

        public static TrophiesChecker CreateInstance(TrophyKey key, TrophyService trophyService)
        {
            switch (key)
            {
                case TrophyKey.BidOnNewYear:
                {
                    return new BidOnNewYearTrophyChecker(trophyService);
                }
                default:
                {
                    return null;
                }
            }
        }
    }
}
