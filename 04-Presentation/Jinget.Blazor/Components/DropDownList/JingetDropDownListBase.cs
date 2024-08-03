
namespace Jinget.Blazor.Components.DropDownList;

public abstract class JingetDropDownListBase : JingetDropDownListBaseComponent<JingetDropDownItemModel>
{
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
