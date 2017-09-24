using Mazadaty.Models.Enum;
using System.Collections.Generic;

namespace Mazadaty.Models.Extensions
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
