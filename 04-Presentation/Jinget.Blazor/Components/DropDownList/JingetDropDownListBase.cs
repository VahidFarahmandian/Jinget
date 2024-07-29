namespace Jinget.Blazor.Components.DropDownList;

public abstract class JingetDropDownListBase : JingetDropDownListBaseComponent<JingetDropDownItemModel>
{
    protected override async Task OnInitializedAsync()
    {
        if (DataProviderFunc != null)
        {
            //exec given delegate and populate data in Items
            Items = await DataProviderFunc();
            await OnDataBound.InvokeAsync();
        }
        //data is loaded to Items, so the component can start rendering
        connected = true;
        await base.OnInitializedAsync();
    }

    /// <summary>
    /// Set <seealso cref="SelectedItem"/> using item index in <seealso cref="Items"/>
    /// </summary>
    public override async Task SetSelectedIndexAsync(int index)
    {
        if (index < Items.Count)
        {
            await SetSelectedItemAsync(Items[index].Value);
        }
    }
}
