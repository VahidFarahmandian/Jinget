namespace Jinget.Blazor.Attributes.DropDownList;

public class JingetDropDownListTreeElement : JingetDropDownListElementBase
{
    public async Task<List<JingetDropDownTreeItemModel>> BindAsync<T>(Func<Task<List<T>>> GetData)
        where T : BaseTypeTreeModel
    {
        return await BindAsync<T, int>(GetData).ConfigureAwait(false);
    }

    public async Task<List<JingetDropDownTreeItemModel>> BindAsync<T, TCode>(Func<Task<List<T>>> GetData)
        where T : BaseTypeTreeModel<TCode>
    {
        List<JingetDropDownTreeItemModel> result = [];

        List<T> data = await GetData.Invoke().ConfigureAwait(false);

        if (data != null)
        {
            foreach (T item in data)
            {
                result.Add(new(item.Id, item.ParentId, item.Title));
            }
        }
        return result;
    }
}