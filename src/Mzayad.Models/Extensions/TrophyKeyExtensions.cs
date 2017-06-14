using Mzayad.Models.Enum;
using System.Collections.Generic;

namespace Mzayad.Models.Extensions
{
    public static class TrophyKeyExtensions
    {
        public static bool TryAdd(this IList<TrophyKey> trophyKeys, TrophyKey? key)
        {
            if (!key.HasValue)
            {
                return false;
            }

            trophyKeys.Add(key.Value);

            return true;
        }
    }
}
