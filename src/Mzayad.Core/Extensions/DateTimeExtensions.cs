using System;

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
            if (!dateTime.HasValue)
            {
                return null;
            }
            
            return dateTime.Value.AddHours(3); // UTC => AST
        }
    }
}
