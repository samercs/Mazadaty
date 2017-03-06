using System;
using System.Collections.Generic;
using System.Linq;

namespace Mzayad.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToLocalTime(this DateTime dateTime)
        {
            return dateTime.AddHours(3); // UTC => AST
        }

        public static DateTime? ToLocalTime(this DateTime? dateTime)
        {
            return dateTime?.AddHours(3); // UTC => AST
        }

        ///<summary>
        /// Get number of consecutive days
        /// </summary>
        public static int Consecutive(this List<DateTime> input)
        {
            if (!input.Any() || input.First().Date != DateTime.UtcNow.Date)
            {
                return 0;
            }

            var inputGroupedByDate = input.GroupBy(i => i.Date).ToList();

            var streak = 1;

            for (var i = 0; i < input.Count - 1; i++)
            {
                var dateDifference = inputGroupedByDate[i].Key.Subtract(inputGroupedByDate[i + 1].Key).Days;
                if (dateDifference != 1)
                {
                    return streak;
                }

                streak++;
            }

            return streak;
        }
    }
}
