using Jinget.Blazor.Enums;

namespace Jinget.Blazor.Components.Table;

public abstract partial class JingetTableBase<T> : JingetBaseComponent
{
    /// <summary>
    /// Defines whether to show the search bar or not. default is true.
    /// </summary>
    [Parameter] public bool ShowSearchBar { get; set; } = true;

    /// <summary>
    /// Text used to bind as search input placeholder. default is 'جستجو'
    /// </summary>
    [Parameter] public string SearchPlaceHolderText { get; set; } = "جستجو";

    /// <summary>
    /// Search term badge text format. default is: 'فیلتر شده براساس مقدار: {search_term}'
    /// </summary>
    [Parameter] public string SearchBadgeTextFormat { get; set; } = "فیلتر شده براساس مقدار: {search_term}";

    /// <summary>
    /// Dynamic content used to load inside search bar
    /// </summary>
    [Parameter] public RenderFragment? SearchBarContent { get; set; }

    protected string currentSearchTerm = "";
    string GetSearchTerm() => currentSearchTerm;

    protected bool HasSearchTerm() => !string.IsNullOrWhiteSpace(currentSearchTerm);
    protected async Task ClearSearchAsync() => await SearchAsync("");
    protected string GetSearchBadgeText() => SearchBadgeTextFormat.Replace("{search_term}", currentSearchTerm);

    protected async Task SearchAsync(string searchTerm)
    {
        currentSearchTerm = searchTerm;
        currentPageIndex = 1;
        await BindAsync(JingetTableEventType.Search);
    }
}
