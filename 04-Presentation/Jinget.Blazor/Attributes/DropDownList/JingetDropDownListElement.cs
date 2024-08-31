namespace Jinget.Blazor.Attributes.DropDownList;

public class JingetDropDownListElement : JingetDropDownListElementBase
{
    public async Task<List<JingetDropDownItemModel>> BindAsync<T>(Func<Task<List<T>>> GetData)
        where T : BaseTypeModel => await BindAsync<T, byte>(GetData).ConfigureAwait(false);

    public async Task<List<JingetDropDownItemModel>> BindAsync<T, TCode>(Func<Task<List<T>>> GetData)
        where T : BaseTypeModel<TCode>
    {
        List<JingetDropDownItemModel> result = [];

        List<T> data = await GetData.Invoke().ConfigureAwait(false);

        if (data != null)
        {
            foreach (T item in data)
            {
                result.Add(new(item.Id, item.Title));
            }
        }
        return result;
    }
}