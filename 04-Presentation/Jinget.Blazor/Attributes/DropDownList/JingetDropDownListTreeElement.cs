namespace Jinget.Blazor.Attributes.DropDownList;

public class JingetDropDownListTreeElement : JingetDropDownListElementBase
{
    /// <summary>
    /// Use this method whenever the Id property has int type
    /// </summary>
    public async Task<List<JingetDropDownTreeItemModel>> BindAsync<T>(Func<Task<List<T>>> GetData)
        where T : BaseTypeTreeModel => await BindAsync<T, int>(GetData).ConfigureAwait(false);

    /// <summary>
    /// Use this method whenever the Id property has type of struct
    /// </summary>
    public async Task<List<JingetDropDownTreeItemModel>> BindAsync<TModel, TId>(Func<Task<List<TModel>>> GetData)
        where TModel : BaseTypeTreeModel<TId>
        where TId : struct
    {
        List<JingetDropDownTreeItemModel> result = [];

        List<TModel> data = await GetData.Invoke().ConfigureAwait(false);

        if (data != null)
        {
            foreach (TModel item in data)
            {
                result.Add(new(item.Id, item.ParentId, item.Title));
            }
        }
        return result;
    }

    /// <summary>
    /// Use this method whenever the Id property has string type
    /// </summary>
    public async Task<List<JingetDropDownTreeItemModel>> BindStringAsync<TModel>(Func<Task<List<TModel>>> GetData)
    where TModel : BaseTypeStringTreeModel
    {
        List<JingetDropDownTreeItemModel> result = [];

        List<TModel> data = await GetData.Invoke().ConfigureAwait(false);

        if (data != null)
        {
            foreach (TModel item in data)
            {
                result.Add(new(item.Id, item.ParentId, item.Title));
            }
        }
        return result;
    }
}