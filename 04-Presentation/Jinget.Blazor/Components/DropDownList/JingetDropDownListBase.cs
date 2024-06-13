namespace Jinget.Blazor.Components.DropDownList;

public abstract class JingetDropDownListBase : JingetBaseComponent
{
    [Parameter] public string DefaultText { get; set; }
    [Parameter] public bool IsSearchable { get; set; }
    [Parameter, EditorRequired] public Func<Task<List<JingetDropDownItemModel>>>? DataProviderFunc { get; set; }

    /// <summary>
    /// Raised whenever the <seealso cref="Items"/> changed.
    /// </summary>
    [Parameter] public EventCallback OnDataBound { get; set; }
    public List<JingetDropDownItemModel> Items { get; set; } = [];
    public JingetDropDownItemModel? SelectedItem { get; protected set; }

    protected override async Task OnInitializedAsync()
    {
        if (DataProviderFunc != null)
        {
            Items = await DataProviderFunc();
            await OnDataBound.InvokeAsync();
        }
        await base.OnInitializedAsync();
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_initialized)
        {
            _initialized = false;
            await OnRendered.InvokeAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task SetSelectedItemAsync(object? value)
    {
        Value = value;
        StateHasChanged();
        await OnSelectedItemChangedAsync(value);
    }

    public async Task SetSelectedIndexAsync(int index)
    {
        if (index < Items.Count)
        {
            object item = Items[index].Value;
            await SetSelectedItemAsync(item);
        }
    }

    [JSInvokable]
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
        await OnChange.InvokeAsync(new ChangeEventArgs { Value = e });
    }
}
