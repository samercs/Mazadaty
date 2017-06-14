using OrangeJetpack.Base.Data;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
{
    public class IslamicCalendar : EntityBase
    {
        public int IslamicCalendarId { get; set; }

        [Required , Index("IX_HijriYear",IsUnique = true)]
        public int HijriYear { get; set; }

        [Required , DisplayName("New Year")]
        public DateTime NewYear { get; set; }

        [Required, DisplayName("Eid Fetr From")]
        public DateTime EidFetrFrom { get; set; }

        [Required, DisplayName("Eid Fetr To")]
        public DateTime EidFetrTo { get; set; }

        [Required, DisplayName("Eid Adha From")]
        public DateTime EidAdhaFrom { get; set; }

        [Required, DisplayName("Eid Adha To")]
        public DateTime EidAdhaTo { get; set; }

        public static IslamicCalendar CreateInstance(int hijriYear)
        {
            var instance = new IslamicCalendar() { HijriYear = hijriYear, NewYear = GregorianDate(hijriYear, 1, 1) };
            instance.EidFetrFrom = instance.NewYear.AddMonths(9);
            instance.EidFetrTo = instance.EidFetrFrom;
            instance.EidAdhaFrom = instance.NewYear.AddMonths(11).AddDays(10);
            instance.EidAdhaTo = instance.EidAdhaFrom.AddDays(3);
            return instance;

        }
        private static DateTime GregorianDate(int hijriYear, int hijriMonth, int hijriDay)
        {
            var hijriDate = new DateTime(hijriYear, hijriMonth, hijriDay);
            var hijriMonthDays = new int[] { 30, 29, 30, 29, 30, 29, 30, 29, 30, 29, 30, 29 };
            var miladiMonthDays = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            double hijriDays = 0;

            var index = 0;
            while (index < hijriDate.Month - 1)
            {
                hijriDays += hijriMonthDays[index];
                index++;
            }

            hijriDays += hijriDate.Day;

            var hjiriDecimal = hijriDate.Year + Math.Round((hijriDays / 354), 3);

            var miladiDecimal = Math.Round((hjiriDecimal * 0.970255) + 621.54, 3);

            double miladiDays = Math.Round((miladiDecimal - (int)miladiDecimal) * 365, 3);

            var miladiYear = miladiDecimal;
            if (miladiYear % 4 == 0)
            {
                miladiMonthDays[1] = 29;
            }

            index = 0;
            while (miladiMonthDays[index] <= miladiDays)
            {
                miladiDays -= miladiMonthDays[index];
                index++;
            }

            if ((int)miladiDays - 5 == 0)
            {
                miladiDays = 1;
                index++;
            }
            else if (miladiDays - 5 < 0)
            {
                index--;
                miladiDays = miladiDays + 25;
            }
            else
            {
                miladiDays = miladiDays - 5;
            }
            index++;
            if (index == 0)
            {
                index++;
            }
            else if (index > 12)
            {
                miladiYear++;
                index = index - 12;
            }

            return new DateTime((int)miladiYear, index, (int)miladiDays);
        }
    }
}
