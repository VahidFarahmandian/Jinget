using System;
using System.Globalization;

namespace Jinget.Core.Utilities
{
    public static class DateTimeUtility
    {
        /// <summary>
        /// Converts given Gregorian date to its Solar equalivant
        /// </summary>
        public static string ToSolarDate(DateTime gregorianDate)
        {
            var calendar = new PersianCalendar();
            return
                $"{calendar.GetYear(gregorianDate)}/{calendar.GetMonth(gregorianDate):D2}/{calendar.GetDayOfMonth(gregorianDate):D2}";
        }

        /// <summary>
        /// Converts given Solar date to its Gregorian equalivant
        /// </summary>
        public static DateTime ToGregorianDate(string persianDate)
        {
            if (persianDate.Length == 8 && !persianDate.Contains("/"))
                persianDate = $"{persianDate[..4]}/{persianDate.Substring(4, 2)}/{persianDate.Substring(6, 2)}";
            return DateTime.Parse(persianDate, new CultureInfo("fa-IR"));
        }

        /// <summary>
        /// convert minute to hour and minute representation. for example 150 minutes is equal to 2 hours and 30 minutes
        /// </summary>
        public static TimeSpan ParseToTime(int minute) => TimeSpan.FromMinutes(minute);

        /// <summary>
        /// check if given persian date is a valid date or not. 
        /// <paramref name="persianDate"/> should have exactly 8 numerical characters 
        /// and it should be also greater than the zero
        /// You can check the date validity using minimum acceptable date nad maximum accptable date ranges
        /// </summary>
        public static bool IsValidPersianDate(string persianDate, string minAcceptableDate = "", string maxAcceptableDate = "")
        {
            if (!StringUtility.IsDigitOnly(persianDate) || Convert.ToInt32(persianDate) < 0 || persianDate.Length != 8)
                return false;

            DateTime givenDate = ToGregorianDate(persianDate);
            DateTime? minDate = null;
            DateTime? maxDate = null;
            if (minAcceptableDate != "")
                minDate = ToGregorianDate(minAcceptableDate);
            if (maxAcceptableDate != "")
                maxDate = ToGregorianDate(maxAcceptableDate);
            if ((minDate == null || givenDate >= minDate) && (maxDate == null || givenDate <= maxDate))
                return true;

            else return false;
        }
    }
}