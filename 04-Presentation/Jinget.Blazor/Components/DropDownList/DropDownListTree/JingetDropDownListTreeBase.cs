namespace Jinget.Blazor.Components.DropDownList.DropDownListTree;

public abstract class JingetDropDownListTreeBase : JingetDropDownListBaseComponent<JingetDropDownTreeItemModel>
{
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

    static object? NormalizeParentValue(object? parentValue)
    {
        if (parentValue == null)
            return null;
        if (
            parentValue.ToString() == Guid.Empty.ToString() ||
            parentValue.ToString() == "0" ||
            parentValue.ToString() == string.Empty)
            return null;
        return parentValue;
    }

    List<JingetDropDownTreeItemModel> OrganizeItems() => OriginalItems
                .Select(x => new
                {
                    x.Value,
                    ParentValue = NormalizeParentValue(x.ParentValue),
                    x.Text,
                    Route = Traverse(OriginalItems, x, out int level),
                    Level = level
                })
                .OrderBy(x => x.Route).ThenBy(x => x.Text)
                .Select(x => new JingetDropDownTreeItemModel(x.Value, x.ParentValue, x.Text) { Padding = x.Level })
                .ToList();

    /// <summary>
    /// Data binded to the drop down list
    /// </summary>
    public List<JingetDropDownTreeItemModel> OriginalItems { get; private set; } = [];

    public override List<JingetDropDownTreeItemModel> Items
    {
        get => base.Items;
        set
        {
            OriginalItems = value;
            base.Items = OrganizeItems();
        }
    }

    /// <summary>
    /// Set <seealso cref="SelectedItem"/> using item index in <seealso cref="OriginalItems"/>
    /// </summary>
    public override async Task SetSelectedIndexAsync(int index)
    {
        if (index < Items.Count)
        {
            await SetSelectedItemAsync(OriginalItems[index].Value);
        }
    }
}
