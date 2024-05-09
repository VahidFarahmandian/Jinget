namespace Jinget.Blazor.Components.Picker.CultureService;

internal class FaIRCultureService
{
    internal static CultureInfo GetCulture()
    {
        var culture = new CultureInfo("fa-IR");
        culture.NumberFormat.NumberDecimalSeparator = "/";
        culture.NumberFormat.DigitSubstitution = DigitShapes.NativeNational;
        culture.NumberFormat.NumberNegativePattern = 0;

        DateTimeFormatInfo formatInfo = culture.DateTimeFormat;
        formatInfo.AbbreviatedDayNames = DateTimeUtility.GetJalaliDayAbbrNames();
        formatInfo.DayNames = DateTimeUtility.GetJalaliDayNames(); 

        formatInfo.AbbreviatedMonthNames =
        formatInfo.MonthNames =
        formatInfo.MonthGenitiveNames = formatInfo.AbbreviatedMonthGenitiveNames = DateTimeUtility.GetJalaliMonthNames();
        formatInfo.AMDesignator = "ق.ظ";
        formatInfo.PMDesignator = "ب.ظ";
        formatInfo.ShortDatePattern = "yyyy/MM/dd";
        formatInfo.LongDatePattern = "dddd, dd MMMM,yyyy";
        formatInfo.FirstDayOfWeek = DayOfWeek.Saturday;

        Calendar cal = new PersianCalendar();

        FieldInfo fieldInfo = culture.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
        fieldInfo?.SetValue(culture, cal);
        FieldInfo info = formatInfo.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
        info?.SetValue(formatInfo, cal);

        return culture;
    }

}
