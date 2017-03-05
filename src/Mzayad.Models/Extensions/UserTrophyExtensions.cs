using Mzayad.Models.Enum;
using System.Collections.Generic;
using System.Linq;

namespace Mzayad.Models.Extensions
{
    public static class UserTrophyExtensions
    {
        public static UserTrophy GetLastEarned(this IEnumerable<UserTrophy> userTrophies, TrophyKey trophyKey)
        {
            return userTrophies
                .OrderBy(i => i.CreatedUtc)
                .LastOrDefault(i => i.Trophy.Key == trophyKey);
        }
    }
}
