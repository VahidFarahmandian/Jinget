namespace Jinget.Blazor.Attributes;

public class JingetComboBox : JingetFormElement
{
    public string DefaultText { get; set; } = "---انتخاب کنید---";

    public bool Searchable { get; set; }
    /// <summary>
    /// set a method which will be called automatically while binding the element
    /// </summary>
    public string? BindingFunction { get; set; }

    public async Task<List<DropDownItemModel>> BindAsync<T>(Func<Task<List<T>>> GetData)
        where T : BaseTypeModel => await BindAsync<T, byte>(GetData).ConfigureAwait(false);

    public async Task<List<DropDownItemModel>> BindAsync<T, TCode>(Func<Task<List<T>>> GetData)
        where T : BaseTypeModel<TCode>
    {
        List<DropDownItemModel> result = [];
        var data = await GetData.Invoke().ConfigureAwait(false);


        if (data != null)
        {
            foreach (var item in data)
            {
                result.Add(new(item.Code, item.Title, false));
            }
        }
        return result;
    }
}