using Jinget.Blazor.Attributes;
using Jinget.Blazor.Components.DropDownList;
using Jinget.Blazor.Components.Other;
using Jinget.Core.Enumerations;
using Jinget.Core.ExtensionMethods.Enums;
using Newtonsoft.Json;

namespace Jinget.Blazor.Components.Table
{
    public abstract class JingetTableBase<T> : JingetBaseComponent
    {
        protected int currentPageIndex = 1;
        protected int lastPageIndex;
        protected JingetDropDownList ddlPagination;
        protected JingetJsonVisualizer jingetJsonVisualizer;
        protected bool HasData() => Model != null && Model.Items != null && (Model.Items.Any() || Model.TotalItems > 0);
        protected bool HasSearchTerm() => !string.IsNullOrWhiteSpace(currentSearchTerm);
        protected bool HasOrderBy() => !string.IsNullOrWhiteSpace(currentSortName) && currentSortDirection != null;
        protected bool HasSelectedRow() => selectedRow != null;
        public int TotalRows { get => Model == null ? 0 : Model.TotalItems; }
        public int TotalColumns { get => columns.Count + (PreActionContent != null ? 1 : 0) + (PostActionContent != null ? 1 : 0); }

        [Parameter] public JingetTableModel<T>? Model { get; set; }

        [Parameter] public RenderFragment? NoRecordContent { get; set; }

        /// <summary>
        /// header text used to display as pre action content column header
        /// </summary>
        [Parameter] public string PreActionContentHeaderText { get; set; }
        /// <summary>
        /// action content which is rendered before rendering the data row
        /// </summary>
        [Parameter] public RenderFragment<T?>? PreActionContent { get; set; }

        /// <summary>
        /// header text used to display as post action content column header
        /// </summary>
        [Parameter] public string PostActionContentHeaderText { get; set; }
        /// <summary>
        /// action content which is rendered after rendering the data row
        /// </summary>
        [Parameter] public RenderFragment<T?>? PostActionContent { get; set; }

        #region Search

        protected string currentSearchTerm = "";

        [Parameter] public bool ShowSearchBar { get; set; } = true;
        [Parameter] public bool ShowBadgeBar { get; set; } = true;
        [Parameter] public bool Sortable { get; set; } = true;
        [Parameter] public string SearchPlaceHolderText { get; set; } = "Search";

        [Parameter] public RenderFragment? SearchBarContent { get; set; }

        protected async Task ClearSearchAsync()
        {
            await SearchAsync("");
        }

        protected async Task SearchAsync(string searchTerm)
        {
            currentSearchTerm = searchTerm;
            currentPageIndex = 1;
            await OnDataBind.InvokeAsync(new JingetTableDataBindModel
            {
                EventType = JingetTableEventType.Search,
                PageIndex = currentPageIndex,
                PageSize = selectedPageSizeOption,
                SearchTerm = currentSearchTerm,
                SortColumn = GetOrderByColumn(),
                SortDirection = GetOrderByDirection()
            });
        }

        OrderByDirection GetOrderByDirection() => currentSortDirection.HasValue ? currentSortDirection.Value : OrderByDirection.Ascending;
        string GetOrderByColumn() => string.IsNullOrWhiteSpace(currentSortName) ? "" : currentSortName;

        #endregion

        #region Pagination

        [Parameter] public string? FirstPageText { get; set; } = "<<";
        [Parameter] public string? PrevPageText { get; set; } = "<";
        [Parameter] public string? NextPageText { get; set; } = ">";
        [Parameter] public string? LastPageText { get; set; } = ">>";

        protected int selectedPageSizeOption = 0;
        protected async Task<List<JingetDropDownItemModel>> GetPaginationPageSizeOptionsAsync()
        {
            var items = PaginationPageSizeOptions.Select(x => new JingetDropDownItemModel(x, x.ToString())).ToList();
            items.Add(new JingetDropDownItemModel(-1, AllItemsText));
            return await Task.FromResult(items);
        }
        [Parameter] public ICollection<int> PaginationPageSizeOptions { get; set; } = [5, 10, 20, 50, 100];

        protected async Task PageSizeChangedAsync(ChangeEventArgs e)
        {
            selectedPageSizeOption = (int)ddlPagination.SelectedItem.Value;
            currentPageIndex = 1;
            await OnDataBind.InvokeAsync(new JingetTableDataBindModel
            {
                EventType = JingetTableEventType.Pagination,
                PageIndex = currentPageIndex,
                PageSize = selectedPageSizeOption == -1 ? TotalRows : selectedPageSizeOption,
                SearchTerm = currentSearchTerm,
                SortColumn = GetOrderByColumn(),
                SortDirection = GetOrderByDirection()
            });
        }

        protected List<int> GetPageIndexes()
        {
            List<int> indexes = [];

            if (selectedPageSizeOption == 0)
                selectedPageSizeOption = PaginationPageSizeOptions.FirstOrDefault();

            int pageCounts = TotalRows / selectedPageSizeOption;
            pageCounts += TotalRows % selectedPageSizeOption > 0 ? 1 : 0;
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
            //StateHasChanged();
            await OnDataBind.InvokeAsync(new JingetTableDataBindModel
            {
                EventType = JingetTableEventType.Pagination,
                PageIndex = currentPageIndex,
                PageSize = selectedPageSizeOption,
                SearchTerm = currentSearchTerm,
                SortColumn = GetOrderByColumn(),
                SortDirection = GetOrderByDirection()
            });
        }

        protected string GetSortIcon()
        {
            return currentSortDirection switch
            {
                null => "sort",
                OrderByDirection.Ascending => "sort-desc",
                _ => "sort-asc"
            };
        }

        [Parameter] public string SearchBadgeTextFormat { get; set; } = "Filtered by: {search_term}";
        public string GetSearchBadgeText()
        {
            return SearchBadgeTextFormat.Replace("{search_term}", currentSearchTerm);
        }

        protected OrderByDirection? currentSortDirection = null;
        protected string? currentSortName = null;

        [Parameter] public string SortBadgeTextFormat { get; set; } = "Sorted by: {sort_col} {sort_dir}";
        [Parameter] public string AscendingSortText { get; set; } = OrderByDirection.Ascending.GetDisplayName();
        [Parameter] public string DescendingSortText { get; set; } = OrderByDirection.Descending.GetDisplayName();

        [Parameter] public string SelectedRowBadgeTextFormat { get; set; } = "Selected row: ";

        public string GetSortBadgeText()
        {
            string colDisplayText = columns.FirstOrDefault(x => x.Name == currentSortName).DisplayText;
            string sortDirectionText = GetOrderByDirection() == OrderByDirection.Ascending ? AscendingSortText : DescendingSortText;
            return SortBadgeTextFormat
                .Replace("{sort_col}", colDisplayText)
                .Replace("{sort_dir}", sortDirectionText);
        }

        public string GetSelectedRowBadgeText()
        {
            return JsonConvert.SerializeObject(selectedRow, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
        protected async Task ClearSelectedRowAsync()
        {
            await SelectRowAsync(default);
        }

        protected async Task ClearSortAsync()
        {
            currentSortDirection = null;
            await SortAsync(true, "", true);
        }

        protected async Task SortAsync(bool sortable, string name, bool reset = false)
        {
            if (!sortable)
                return;
            currentSortName = name;
            if (!reset)
            {
                if (currentSortDirection == null || currentSortDirection == OrderByDirection.Descending)
                    currentSortDirection = OrderByDirection.Ascending;
                else
                    currentSortDirection = OrderByDirection.Descending;
            }

            currentPageIndex = 1;
            await OnDataBind.InvokeAsync(new JingetTableDataBindModel
            {
                EventType = JingetTableEventType.Sort,
                PageIndex = currentPageIndex,
                PageSize = selectedPageSizeOption,
                SearchTerm = currentSearchTerm,
                SortColumn = GetOrderByColumn(),
                SortDirection = GetOrderByDirection()
            });
        }

        [Parameter] public bool ShowPagination { get; set; } = true;
        [Parameter] public string RowsPerPageString { get; set; } = "Per page size:";

        private string _infoFormat = "{first_item}-{last_item} from {all_items}";
        [Parameter]
        public string InfoFormat
        {
            get
            {
                int all_items = Model == null ? 0 : TotalRows;
                int first_item = ((currentPageIndex - 1) * selectedPageSizeOption) + 1;
                int last_item = (currentPageIndex) * selectedPageSizeOption;
                last_item = selectedPageSizeOption == -1 || last_item > all_items ? all_items : last_item;
                return _infoFormat
                .Replace("{first_item}", first_item.ToString())
                .Replace("{last_item}", last_item.ToString())
                .Replace("{all_items}", all_items.ToString());
            }
            set => _infoFormat = value;
        }
        [Parameter] public string AllItemsText { get; set; } = "All";

        #endregion

        [Parameter] public EventCallback<JingetTableDataBindModel> OnDataBind { get; set; }

        [Parameter] public bool RowSelectable { get; set; }
        [Parameter] public EventCallback<T> OnRowSelected { get; set; }
        [Parameter] public string SelectedRowCss { get; set; } = "table-info";

        protected T selectedRow;
        protected async Task SelectRowAsync(T? row)
        {
            if (!RowSelectable)
            {
                await Task.CompletedTask;
            }
            else
            {
                selectedRow = row;
                await OnRowSelected.InvokeAsync(row);
            }
        }

        protected List<(string DisplayText, string Name, bool Sortable)> columns = [];

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Model ??= JingetTableObjectFactory<T>.EmptyTableData.Instance;
                selectedPageSizeOption = (int)(await GetPaginationPageSizeOptionsAsync()).First().Value;
                await OnDataBind.InvokeAsync(new JingetTableDataBindModel
                {
                    EventType = JingetTableEventType.None,
                    PageIndex = currentPageIndex,
                    PageSize = selectedPageSizeOption,
                    SearchTerm = currentSearchTerm,
                    SortColumn = GetOrderByColumn(),
                    SortDirection = GetOrderByDirection()
                });
            }
        }

        protected async override Task OnParametersSetAsync()
        {
            columns = [];
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
                        columns.Add((cellColumn, item.Name, sortable));
                    }
                }
            }
            await Task.CompletedTask;
        }
    }
}
