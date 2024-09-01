using Jinget.Blazor.Attributes;
using Jinget.Blazor.Enums;
using Jinget.Blazor.Models.JingetTable;

namespace Jinget.Blazor.Components.Table;

public abstract partial class JingetTableBase<T> : JingetBaseComponent
{
    /// <summary>
    /// Model used to bind the table
    /// </summary>
    [Parameter] public JingetTableModel<T>? Model { get; set; }

    /// <summary>
    /// Dynamic content used to load inside table body, whenever table contains no data
    /// </summary>
    [Parameter] public RenderFragment? NoRecordContent { get; set; }

    /// <summary>
    /// header text used to display as pre action content column header
    /// </summary>
    [Parameter] public string PreActionContentHeaderText { get; set; }

    /// <summary>
    /// Dynamic content which is rendered before rendering the data row
    /// </summary>
    [Parameter] public RenderFragment<T?>? PreActionContent { get; set; }

    /// <summary>
    /// header text used to display as post action content column header
    /// </summary>
    [Parameter] public string PostActionContentHeaderText { get; set; }

    /// <summary>
    /// Dynamic content which is rendered after rendering the data row
    /// </summary>
    [Parameter] public RenderFragment<T?>? PostActionContent { get; set; }

    /// <summary>
    /// Event which is fired whenever the table is being binded.
    /// </summary>
    [Parameter] public EventCallback<JingetTableDataBindModel> OnDataBind { get; set; }

    protected int _totalColumns { get => _columns.Count + (PreActionContent != null ? 1 : 0) + (PostActionContent != null ? 1 : 0); }
    internal List<JingetTableColumnModel> _columns = [];
    protected bool _showFooter { get => ShowPagination && GetPageIndexes().Any(); }
    protected string _parentDivCssClass { get => IsRtl ? "rtl" : "ltr"; }
    protected string _tableCssClass { get => IsRtl ? "jinget-farsi-font" : "jinget-default-font"; }

    protected bool HasData() => Model != null && Model.Items != null && (Model.Items.Any() || Model.TotalItems > 0);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Model ??= JingetTableObjectFactory<T>.EmptyTableData.Instance;
            selectedPageSizeOption = (int)(await GetPaginationPageSizeOptionsAsync()).First().Value;
            await OnDataBind.InvokeAsync(new JingetTableDataBindModel(
                currentSearchTerm,
                currentPageIndex,
                selectedPageSizeOption,
                GetSortColumn(),
                EventType: JingetTableEventType.None)
            {
                SortDirection = GetSortDirection()
            });
        }
    }

    protected async override Task OnParametersSetAsync()
    {
        _columns = [];
        Type modelType = typeof(T);
        if (modelType.IsDefined(typeof(JingetTableElement), true))
        {
            var properties = modelType.GetProperties()
                    .Where(x => x.IsDefined(typeof(JingetTableMember), true))
                    .OrderBy(x => x.GetCustomAttribute<JingetTableMember>()?.Order).ToList();

            foreach (var item in properties)
            {
                var attr = item.GetCustomAttribute<JingetTableMember>();
                if (attr != null)
                {
                    var cellColumn = attr.DisplayName;
                    var sortable = attr.Sortable;
                    cellColumn = string.IsNullOrWhiteSpace(cellColumn) ? item.Name : cellColumn;
                    string cssClass = sortable ? "sortable" : "";
                    _columns.Add(new JingetTableColumnModel(cellColumn, item.Name, sortable, cssClass));
                }
            }
        }
        await Task.CompletedTask;
    }

    private async Task BindAsync(JingetTableEventType eventType)
        => await OnDataBind.InvokeAsync(
            new JingetTableDataBindModel(
                GetSearchTerm(),
                GetSelectedPageIndex(),
                GetSelectedPageSize(),
                GetSortColumn(),
                GetSortDirection(),
                eventType));
}