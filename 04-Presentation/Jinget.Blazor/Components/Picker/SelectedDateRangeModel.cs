namespace Jinget.Blazor.Components.Picker
{
    public record SelectedDateRangeModel(
        DateTime? StartDate,
        DateTime? EndDate,
        string? StartDateJalali,
        string? EndDateJalali);
}
