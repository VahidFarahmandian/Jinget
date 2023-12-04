namespace Jinget.Blazor.Components.Picker.CultureService
{
    public class CalendarService
    {
        public static CultureInfo GetCulture(string culture)
        {
            return culture switch
            {
                "fa-IR" => FaIRCultureService.GetCulture(),
                _ => EnUSCultureService.GetCulture(),
            };
        }

    }
}
