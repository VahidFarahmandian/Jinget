using Jinget.Core.ExtensionMethods;
using System.Linq;

namespace Jinget.Blazor.Components.DropDownList;

public abstract class JingetDropDownListTreeBase : JingetBaseComponent
{
    /// <summary>
    /// Default string used to be shown in dropdownlisttree, whenever user choose nothing.
    /// </summary>
    [Parameter] public string DefaultText { get; set; }

    /// <summary>
    /// if set to true, then dropdownllist will have searching mechanism
    /// </summary>
    [Parameter] public bool IsSearchable { get; set; }

    /// <summary>
    /// if set to true, then dropdownllist items will be rendered in right to left direction
    /// </summary>
    [Parameter] public bool IsRtl { get; set; }

    /// <summary>
    /// Delegate used to bind data to dropdownlisttree.
    /// </summary>
    [Parameter, EditorRequired] public Func<Task<List<JingetDropDownTreeItemModel>>>? DataProviderFunc { get; set; }

    /// <summary>
    /// Raised whenever the <seealso cref="Items"/> changed.
    /// </summary>
    [Parameter] public EventCallback OnDataBound { get; set; }

    /// <summary>
    /// Data binded to the drop down list
    /// </summary>
    public List<JingetDropDownTreeItemModel> Items { get; set; } = [];

    /// <summary>
    /// Currently selected item among <seealso cref="Items"/> members
    /// </summary>
    public JingetDropDownTreeItemModel? SelectedItem { get; protected set; }

    /// <summary>
    /// This field is only used when <seealso cref="IsSearchable"/> is true, to prevent rendering element before binding data into <seealso cref="Items"/>
    /// </summary>
    protected internal bool connected = false;

    static string Traverse(IEnumerable<JingetDropDownTreeItemModel> set, JingetDropDownTreeItemModel sample, out int level)
    {
        string route = "";
        if (sample == null || sample.Value == null)
            route = "";
        else if (
            sample.ParentValue == null ||
            !set.Any(x => string.Equals(x.Value.ToString(), sample.ParentValue.ToString(), StringComparison.OrdinalIgnoreCase))
            )
            route = sample.Value.ToString();
        else
            route = Traverse(
                set,
                set.First(x => string.Equals(
                                    x.Value.ToString(),
                                    sample.ParentValue.ToString(),
                                    StringComparison.OrdinalIgnoreCase)),
                out level) + "/" + sample.Value.ToString();
        level = route.Split('/', StringSplitOptions.RemoveEmptyEntries).Length;
        return route;
    }

    protected override async Task OnInitializedAsync()
    {
        if (DataProviderFunc != null)
        {
            //exec given delegate and populate data in Items
            var data = await DataProviderFunc();
            Items = data
                .Select(x => new
                {
                    x.Value,
                    x.ParentValue,
                    x.Text,
                    Route = Traverse(data, x, out int level),
                    Level = level
                })
                .OrderBy(x => x.Route).ThenBy(x => x.Text)
                .Select(x => new JingetDropDownTreeItemModel(x.Value, x.ParentValue, x.Text))
                .ToList();
            await OnDataBound.InvokeAsync();
        }
        //data is loaded to Items, so the component can start rendering
        connected = true;
        await base.OnInitializedAsync();
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        //When _initialized is true, it means that parent completed the OnInitializedAsync event so that the OnAfterRenderAsync can start
        if (_initialized)
        {
            //setting _initialized to false is mandatory to prevent reinitializing
            _initialized = false;
            await OnRendered.InvokeAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Set <seealso cref="SelectedItem"/> using item value(<seealso cref="JingetDropDownItemModel.Value"/>) in <seealso cref="Items"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task SetSelectedItemAsync(object? value)
    {
        Value = value;
        await OnSelectedItemChangedAsync(value);
    }

    /// <summary>
    /// Set <seealso cref="SelectedItem"/> using item index in <seealso cref="Items"/>
    /// </summary>
    public async Task SetSelectedIndexAsync(int index)
    {
        if (index < Items.Count)
        {
            await SetSelectedItemAsync(Items[index].Value);
        }
    }

    /// <summary>
    /// This method is being invoked by jinget.custom.js. whenever searchable dropdownlisttree's selected item changed
    /// </summary>
    [JSInvokable]
    public void OnJSDropDownListTreeSelectedItemChanged(object? e) => OnSelectedItemChangedAsync(e);

    protected async Task OnSelectedItemChangedAsync(object? e)
    {
        Value = e;

        if (e == null)
        {
            SelectedItem = null;
        }
        else
        {
            SelectedItem = Items.FirstOrDefault(x => x.Value?.ToString() == e.ToString());
        }
        StateHasChanged();
        await OnChange.InvokeAsync(new ChangeEventArgs { Value = e });
    }
}
