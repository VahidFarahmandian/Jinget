namespace Jinget.Blazor.Attributes;

public class JingetDropDownListElement : JingetFormElement
{
    public string DefaultText { get; set; } = "---انتخاب کنید---";

    public bool IsSearchable { get; set; }

    /// <summary>
    /// if set to true, then dropdownllist items will be rendered in right to left direction.
    /// default is <see cref="true"/>
    /// </summary>
    public bool IsRtl { get; set; } = true;

    /// <summary>
    /// set a method which will be called automatically while binding the element
    /// </summary>
    public string? BindingFunction { get; set; }

    public string? PreBindingFunction { get; set; }

    public string? PostBindingFunction { get; set; }

    public async Task<List<JingetDropDownItemModel>> BindAsync<T>(Func<Task<List<T>>> GetData)
        where T : BaseTypeModel
    {
        return await BindAsync<T, byte>(GetData).ConfigureAwait(false);
    }

    public async Task<List<JingetDropDownItemModel>> BindAsync<T, TCode>(Func<Task<List<T>>> GetData)
        where T : BaseTypeModel<TCode>
    {
        List<JingetDropDownItemModel> result = [];

        List<T> data = await GetData.Invoke().ConfigureAwait(false);

        if (data != null)
        {
            foreach (T item in data)
            {
                result.Add(new(item.Code, item.Title));
            }
        }
        return result;
    }
}