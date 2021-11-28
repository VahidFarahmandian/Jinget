using System;
using System.Globalization;

namespace Jinget.Core.Utilities
{
    public static class DateTimeUtility
    {
        /// <summary>
        /// Converts given Gregorian date to its Solar equalivant
        /// </summary>
        public static string ToSolarDate(DateTime georgianDate)
        {
            var calendar = new PersianCalendar();
            return
                $"{calendar.GetYear(georgianDate)}/{calendar.GetMonth(georgianDate):D2}/{calendar.GetDayOfMonth(georgianDate):D2}";
        }

        /// <summary>
        /// Converts given Solar date to its Gregorian equalivant
        /// </summary>
        public static DateTime ToGeorgianDate(string persianDate) => DateTime.Parse(persianDate, new CultureInfo("fa-IR"));

        /// <summary>
        /// convert minute to hour and minute representation. for example 150 minutes is equal to 2 hours and 30 minutes
        /// </summary>
        public static TimeSpan ParseToTime(int minute) => TimeSpan.FromMinutes(minute);
    }
}