namespace Jinget.Blazor.Components.Picker.CultureService
{
    internal class EnUSCultureService
    {
        internal static CultureInfo GetCulture()
        {
            var culture = new CultureInfo("en-US");
            culture.NumberFormat.NumberDecimalSeparator = "/";
            culture.NumberFormat.DigitSubstitution = DigitShapes.NativeNational;
            culture.NumberFormat.NumberNegativePattern = 0;

            return culture;
        }

    }
}
