namespace Jinget.Blazor.Components.Picker.CultureService;

internal class ArSACultureService
{
    internal static CultureInfo GetCulture()
    {
        var culture = new CultureInfo("ar-SA");
        culture.NumberFormat.NumberDecimalSeparator = "/";
        culture.NumberFormat.DigitSubstitution = DigitShapes.NativeNational;
        culture.NumberFormat.NumberNegativePattern = 0;

        DateTimeFormatInfo formatInfo = culture.DateTimeFormat;
        formatInfo.AbbreviatedDayNames = DateTimeUtility.GetArabicDayAbbrNames();
        formatInfo.DayNames = DateTimeUtility.GetArabicDayNames();

        formatInfo.AbbreviatedMonthNames =
        formatInfo.MonthNames =
        formatInfo.MonthGenitiveNames = formatInfo.AbbreviatedMonthGenitiveNames = DateTimeUtility.GetArabicMonthNames();
        formatInfo.AMDesignator = "ق.ظ";
        formatInfo.PMDesignator = "ب.ظ";
        formatInfo.ShortDatePattern = "yyyy/MM/dd";
        formatInfo.LongDatePattern = "dddd, dd MMMM,yyyy";
        formatInfo.FirstDayOfWeek = DayOfWeek.Saturday;

        Calendar cal = new HijriCalendar();

        FieldInfo fieldInfo = culture.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
        fieldInfo?.SetValue(culture, cal);
        FieldInfo info = formatInfo.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
        info?.SetValue(formatInfo, cal);

        return culture;
    }

}
