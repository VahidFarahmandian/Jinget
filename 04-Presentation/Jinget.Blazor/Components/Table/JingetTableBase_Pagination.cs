using Jinget.Blazor.Components.DropDownList;
using Jinget.Blazor.Enums;

namespace Jinget.Blazor.Components.Table;

public abstract partial class JingetTableBase<T> : JingetBaseComponent
{
    /// <summary>
    /// Defines whether to show the pagination bar or not. default is <see cref="true"/>.
    /// </summary>
    [Parameter] public bool ShowPagination { get; set; } = true;

    /// <summary>
    /// First page button text in pagination bar. default is 'اولین'
    /// </summary>
    [Parameter] public string? FirstPageText { get; set; } = "اولین";

    /// <summary>
    /// Prev page button text in pagination bar. default is 'قبلی'
    /// </summary>
    [Parameter] public string? PrevPageText { get; set; } = "قبلی";

    /// <summary>
    /// Next page button text in pagination bar. default is 'بعدی'
    /// </summary>
    [Parameter] public string? NextPageText { get; set; } = "بعدی";

    /// <summary>
    /// Last page button text in pagination bar. default is 'آخرین'
    /// </summary>
    [Parameter] public string? LastPageText { get; set; } = "آخرین";

    /// <summary>
    /// All items text inside the pagi size dropdownlist in pagination bar. default is 'همه'
    /// </summary>
    [Parameter] public string AllItemsText { get; set; } = "همه";

    /// <summary>
    /// Page size options inside the page size dropdownlist in pagination bar. default is [5,10,20,50,100]
    /// </summary>
    [Parameter] public ICollection<int> PaginationPageSizeOptions { get; set; } = [5, 10, 20, 50, 100];

    /// <summary>
    /// Page size label text in pagination bar. default is 'اندازه در هر صفحه:'
    /// </summary>
    [Parameter] public string RowsPerPageText { get; set; } = "اندازه در هر صفحه:";

    private string _pageInfoTextFormat = "{first_item}-{last_item} از {all_items}";
    /// <summary>
    /// page info label text in pagination bar. default is '{first_item}-{last_item} از {all_items}'
    /// </summary>
    [Parameter]
    public string PageInfoTextFormat
    {
        get
        {
            int all_items = Model == null ? 0 : _totalRows;
            int first_item = ((currentPageIndex - 1) * selectedPageSizeOption) + 1;
            int last_item = (currentPageIndex) * selectedPageSizeOption;
            last_item = selectedPageSizeOption == -1 || last_item > all_items ? all_items : last_item;
            return _pageInfoTextFormat
            .Replace("{first_item}", first_item.ToString())
            .Replace("{last_item}", last_item.ToString())
            .Replace("{all_items}", all_items.ToString());
        }
        set => _pageInfoTextFormat = value;
    }

    protected int currentPageIndex = 1;
    protected int lastPageIndex;
    protected JingetDropDownList? ddlPagination;
    protected int selectedPageSizeOption = 0;
    int GetSelectedPageSize() => selectedPageSizeOption == -1 ? _totalRows : selectedPageSizeOption;
    int GetSelectedPageIndex() => currentPageIndex;

    int _totalRows { get => Model == null ? 0 : Model.TotalItems; }
    protected bool _hasPrevButtons { get => currentPageIndex <= 1; }
    protected bool _hasNextButtons { get => currentPageIndex >= lastPageIndex; }
    protected bool IsCurrentPage(int item) => currentPageIndex == item;

    /// <summary>
    /// bind pagination page size options
    /// </summary>
    /// <returns></returns>
    protected async Task<List<JingetDropDownItemModel>> GetPaginationPageSizeOptionsAsync()
    {
        var items = PaginationPageSizeOptions.Select(x => new JingetDropDownItemModel(x, x.ToString())).ToList();
        items.Add(new JingetDropDownItemModel(-1, AllItemsText));
        return await Task.FromResult(items);
    }

    protected async Task PageSizeChangedAsync(ChangeEventArgs e)
    {
        selectedPageSizeOption = (int)ddlPagination.SelectedItem.Value;
        currentPageIndex = 1;
        await BindAsync(JingetTableEventType.Pagination);
    }

    protected List<int> GetPageIndexes()
    {
        List<int> indexes = [];

        if (selectedPageSizeOption == 0)
            selectedPageSizeOption = PaginationPageSizeOptions.FirstOrDefault();

        int pageCounts = _totalRows / selectedPageSizeOption;
        pageCounts += _totalRows % selectedPageSizeOption > 0 ? 1 : 0;
        if (pageCounts <= 6)
        {
            for (int i = 1; i <= pageCounts; i++)
            {
                indexes.Add(i);
            }
        }
        else
        {
            if (pageCounts - currentPageIndex > 6)
            {
                int from, to;
                if (currentPageIndex == 1)
                {
                    from = 1;
                    to = 3;
                }
                else
                {
                    from = currentPageIndex - 1;
                    to = currentPageIndex + 1;
                }
                for (int i = from; i <= to; i++)
                {
                    indexes.Add(i);
                }
                indexes.Add(-1);
                from = pageCounts - 2;
                to = pageCounts;
                for (int i = from; i <= to; i++)
                {
                    indexes.Add(i);
                }
            }
            else
            {
                int from = pageCounts - 7;
                int to = pageCounts;
                for (int i = from; i <= to; i++)
                {
                    indexes.Add(i);
                }
            }
        }
        lastPageIndex = pageCounts;

        return indexes;
    }

    protected async Task GotoPageAsync(int pageIndex)
    {
        currentPageIndex = pageIndex;
        await BindAsync(JingetTableEventType.Pagination);
    }
}