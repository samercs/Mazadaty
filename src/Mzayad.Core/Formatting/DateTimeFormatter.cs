using System;

namespace Mzayad.Core.Formatting
{
    public class DateTimeFormatter
    {
        public enum Format
        {
            Sortable,
            Full
        }

        /// <summary>
        /// Gets a formatted string version of the date and time adjusted to local Arabia Standard Time.
        /// </summary>
        public static string ToLocalTime(DateTime? dateTime, Format format = Format.Sortable)
        {
            if (!dateTime.HasValue)
            {
                return "";
            }

            var localTime = dateTime.Value.AddHours(3);

            switch (format)
            {
                case Format.Sortable:
                    return string.Format("{0:yyyy-MM-dd HH:mm} {1}", localTime, "AST");
                case Format.Full:
                    return string.Format("{0:dd MMM yyyy HH:mm}", localTime);
                default:
                    throw new NotImplementedException();
            }
        }

        public static string ToTimeOnly(DateTime? dateTime, Format format = Format.Sortable)
        {
            if (!dateTime.HasValue)
            {
                return "";
            }

            var localTime = dateTime.Value.AddHours(3);

            switch (format)
            {
                case Format.Sortable:
                    return string.Format("{0:hh:mm tt} {1}", localTime, "AST");
                case Format.Full:
                    return string.Format("{0:hh:mm tt}", localTime);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
