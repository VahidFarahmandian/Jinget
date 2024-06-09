namespace Jinget.Blazor.Components.DropDownList;

public abstract class JingetDropDownListBase : JingetBaseComponent
{
    [Parameter] public string DefaultText { get; set; }
    [Parameter, EditorRequired] public Func<Task<List<JingetDropDownItemModel>>>? DataProviderFunc { get; set; }

    public List<JingetDropDownItemModel> Items { get; set; } = [];

    public JingetDropDownItemModel? SelectedItem { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (DataProviderFunc != null)
            Items = await DataProviderFunc();
    }
}
