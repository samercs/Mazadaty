using System;
using System.Collections.Generic;
using System.Linq;

namespace Mzayad.Core.Extensions
{
    public static class ListExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }

            return !enumerable.Any();
        }

        public static void TryAdd<T>(this IList<T> list, T item)
        {
            if (item == null)
            {
                return;
            }

            list.Add(item);
        }
    }
}
