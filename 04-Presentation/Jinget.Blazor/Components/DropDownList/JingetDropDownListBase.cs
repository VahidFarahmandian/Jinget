namespace Jinget.Blazor.Components.DropDownList;

public abstract class JingetDropDownListBase : JingetBaseComponent
{
    [Parameter] public string DefaultText { get; set; }
    [Parameter, EditorRequired] public Func<Task<List<JingetDropDownItemModel>>>? DataProviderFunc { get; set; }

    /// <summary>
    /// Raised whenever the <seealso cref="Items"/> changed.
    /// </summary>
    [Parameter] public EventCallback OnDataBound { get; set; }

    private List<JingetDropDownItemModel> items = [];
    public List<JingetDropDownItemModel> Items
    {
        get => items; set
        {
            if (items != value)
            {
                items = value;
                OnDataBound.InvokeAsync();
            }
        }
    }
    public JingetDropDownItemModel? SelectedItem { get; protected set; }

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

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (DataProviderFunc != null)
        {
            Items = await DataProviderFunc();
        }
    }

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
