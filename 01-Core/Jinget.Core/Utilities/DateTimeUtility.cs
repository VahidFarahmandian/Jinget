using System.Globalization;

namespace Jinget.Core.Utilities;

public static class DateTimeUtility
{
    /// <summary>
    /// Converts given Gregorian date to its Solar equivalent.
    /// Minimum supported Gregorian date is: year: 622,month: 3,day: 22. 
    /// See also: <seealso cref="PersianCalendar.MinSupportedDateTime"/>
    /// </summary>
    public static string? ToSolarDate(DateTime? gregorianDate)
    {
        if (!gregorianDate.HasValue)
            return null;
        var calendar = new PersianCalendar();
        if (gregorianDate < calendar.MinSupportedDateTime)
            throw new ArgumentOutOfRangeException($"Date should be after {calendar.MinSupportedDateTime} ");
        return
        $"{calendar.GetYear(gregorianDate.Value)}/{calendar.GetMonth(gregorianDate.Value):D2}/{calendar.GetDayOfMonth(gregorianDate.Value):D2}";
    }

    /// <summary>
    /// Converts given Solar date to its Gregorian equivalent.
    /// </summary>
    public static DateTime? ToGregorianDate(string persianDate)
    {
        if (string.IsNullOrWhiteSpace(persianDate))
            return null;
        if (persianDate.Length == 8 && !persianDate.Contains('/'))
            persianDate = $"{persianDate[..4]}/{persianDate.Substring(4, 2)}/{persianDate.Substring(6, 2)}";
        return DateTime.Parse(persianDate, new CultureInfo("fa-IR"));
    }

    /// <summary>
    /// convert minute to hour and minute representation. for example 150 minutes is equal to 2 hours and 30 minutes
    /// </summary>
    public static TimeSpan ParseToTime(int minute) => TimeSpan.FromMinutes(minute);

    /// <summary>
    /// check if given persian date is a valid date or not. 
    /// <paramref name="persianDate"/> should have exactly 8 numerical characters, and
    ///  it should be also greater than the zero
    /// You can check the date validity using minimum acceptable date nad maximum acceptable date ranges
    /// </summary>
    public static bool IsValidPersianDate(string persianDate, string minAcceptableDate = "", string maxAcceptableDate = "")
    {
        if (string.IsNullOrWhiteSpace(persianDate) || !StringUtility.IsDigitOnly(persianDate) || Convert.ToInt32(persianDate) < 0 || persianDate.Length != 8)
            return false;

        DateTime? givenDate = ToGregorianDate(persianDate);
        DateTime? minDate = null;
        DateTime? maxDate = null;
        if (minAcceptableDate != "")
            minDate = ToGregorianDate(minAcceptableDate);
        if (maxAcceptableDate != "")
            maxDate = ToGregorianDate(maxAcceptableDate);
        if ((minDate == null || givenDate >= minDate) && (maxDate == null || givenDate <= maxDate))
            return true;

        return false;
    }

    /// <summary>
    /// format given input based on the given format string.
    /// </summary>
    public static string Format(string input, string currentFormat = "yyyyMMdd", string newFormat = "yyyy/MM/dd")
    {
        var isDate = DateTime.TryParseExact(
        input,
        currentFormat,
        CultureInfo.InvariantCulture,
        DateTimeStyles.None,
        out DateTime date);

        return isDate ? date.ToString(newFormat) : input;
    }

    public static string[] GetJalaliDayNames() => ["یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه"];

    public static string[] GetJalaliDayAbbrNames() => ["ی", "د", "س", "چ", "پ", "ج", "ش"];

    public static string[] GetJalaliMonthNames() => [
            "فروردین","اردیبهشت","خرداد",
            "تیر","مرداد","شهریور",
            "مهر","آبان","آذر",
            "دی","بهمن","اسفند",
            "",
        ];

    public static string[] GetArabicDayNames() => ["الأحَد", "الإثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت"];

    public static string[] GetArabicDayAbbrNames() => ["أح", "إث", "ث", "أر", "خ", "ج", "س"];

    public static string[] GetArabicMonthNames() => [
            "ٱلْمُحَرَّم","صَفَر","رَبِيع ٱلْأَوَّل",
            "رَبِيع ٱلثَّانِي","جُمَادَىٰ ٱلْأُولَىٰ","جُمَادَىٰ ٱلثَّانِيَة",
            "رَجَب","شَعْبَان","رَمَضَان",
            "شَوَّال","ذُو ٱلْقَعْدَة","ذُو ٱلْحِجَّة",
            "",
        ];
}