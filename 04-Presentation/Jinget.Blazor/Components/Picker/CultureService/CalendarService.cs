namespace Jinget.Blazor.Components.Picker.CultureService;

public class CalendarService
{
    public static CultureInfo GetCulture(string culture) => culture.ToLowerInvariant() switch
    {
        "fa-ir" => FaIRCultureService.GetCulture(),
        "ar-sa" => ArSACultureService.GetCulture(),
        _ => CultureInfo.GetCultureInfo(culture),
    };

}
