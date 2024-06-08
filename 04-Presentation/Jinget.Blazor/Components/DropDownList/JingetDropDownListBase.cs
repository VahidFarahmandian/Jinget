namespace Jinget.Blazor.Components.DropDownList;

public abstract class JingetDropDownListBase : ComponentBase
{
    [Parameter, EditorRequired] public string Id { get; set; }
    [Parameter] public object? Value { get; set; }
    [Parameter] public bool IsDisabled { get; set; }
    [Parameter] public string CssClass { get; set; }
    [Parameter] public string DefaultText { get; set; }
    [Parameter] public EventCallback<ChangeEventArgs> OnChange { get; set; }
    [Parameter, EditorRequired] public Func<Task<List<DropDownItemModel>>> DataProviderFunc { get; set; }

    public List<DropDownItemModel> Items { get; set; } = [];

    public DropDownItemModel? SelectedItem { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Items = await DataProviderFunc();
    }
}
