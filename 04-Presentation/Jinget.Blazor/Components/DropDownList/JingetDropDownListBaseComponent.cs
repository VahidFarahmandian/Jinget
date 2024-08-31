using Jinget.Core.ExtensionMethods;

namespace Jinget.Blazor.Components.DropDownList;

public abstract class JingetDropDownListBaseComponent<T> : JingetBaseComponent where T : JingetDropDownItemModelBase
{
    /// <summary>
    /// Defines where to attach the dropdown html in page. By default element will be attached to body tag in page.
    /// But in some cases like Bootstrap modals, which tend to steal focus from other elements outside of the modal. 
    /// Since by default, Select2 attaches the dropdown menu to the body element, it is considered "outside of the modal". 
    /// /// To avoid this problem, you may attach the dropdown to the modal itself by setting the modal id in this parameter.
    /// </summary>
    [Parameter] public string ParentElementId { get; set; } = "";

    /// <summary>
    /// Default string used to be shown in dropdownlist, whenever user choose nothing.
    /// </summary>
    [Parameter] public string DefaultText { get; set; }

    /// <summary>
    /// if set to true, then dropdownllist will have searching mechanism
    /// </summary>
    [Parameter] public bool IsSearchable { get; set; }

    /// <summary>
    /// Text used to be shown when search returns no result, while using searchable DropDownListTree
    /// </summary>
    [Parameter] public string NoResultText { get; set; } = "Nothing to display!";

    /// <summary>
    /// Placeholder text used for search input element, while using searchable DropDownListTree
    /// </summary>
    [Parameter] public string SearchPlaceholderText { get; set; } = "";

    /// <summary>
    /// Delegate used to bind data to dropdownlist.
    /// </summary>
    [Parameter] public Func<Task<List<T>>>? DataProviderFunc { get; set; }

    /// <summary>
    /// Raised whenever the <seealso cref="Items"/> changed.
    /// </summary>
    [Parameter] public EventCallback OnDataBound { get; set; }

    private List<T> _items = [];
    /// <summary>
    /// Data binded to the drop down list
    /// </summary>
    public virtual List<T> Items
    {
        get => _items;
        set
        {
            if (_items.SequenceEqual(value))
                return;
            _items = value;
            ItemsChanged.InvokeAsync(value);
            OnDataBound.InvokeAsync();
        }
    }
    public EventCallback<List<T>> ItemsChanged { get; set; }
    //[Parameter] public virtual EventCallback<object?> ValueChanged { get; set; }

    /// <summary>
    /// Currently selected item among <seealso cref="Items"/> members
    /// </summary>
    public T? SelectedItem { get; protected set; }

    /// <summary>
    /// initialize component by calling initJingetDropDownList[Tree]. this functionality is mainly powered by select2.js library.
    /// </summary>
    internal async Task InitComponentAsync(string jsInitiatorFunction) => await JS.InvokeVoidAsync(jsInitiatorFunction,
            new
            {
                dotnet = DotNetObjectReference.Create(this),
                Id,
                IsSearchable,
                IsRtl,
                NoResultText,
                SearchPlaceholderText,
                ParentElementId
            });

    /// <summary>
    /// This field is only used when <seealso cref="IsSearchable"/> is true, to prevent rendering element before binding data into <seealso cref="Items"/>
    /// </summary>
    protected internal bool connected = false;
    protected override async Task OnInitializedAsync()
    {
        //await DataBind();
        if (DataProviderFunc != null)
        {
            //exec given delegate and populate data in Items
            Items = await DataProviderFunc();
        }
        //data is loaded to Items, so the component can start rendering
        connected = true;
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        //When _initialized is true, it means that parent completed the OnInitializedAsync event so that the OnAfterRenderAsync can start
        if (_initialized)
        {
            //setting _initialized to false is mandatory to prevent reinitializing
            _initialized = false;
            await OnRendered.InvokeAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    bool HasValue(object? value) => value != null && value.ToString() != "---";

    /// <summary>
    /// Set <seealso cref="SelectedItem"/> using item value(<seealso cref="JingetDropDownItemModel.Value"/>) in <seealso cref="Items"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task SetSelectedItemAsync(object? value)
    {
        Value = HasValue(value) ? value : null;
        await OnSelectedItemChangedAsync(Value);
    }

    /// <summary>
    /// Set <seealso cref="SelectedItem"/> using item index in <seealso cref="Items"/>
    /// </summary>
    public abstract Task SetSelectedIndexAsync(int index);

    /// <summary>
    /// This method is being invoked by jinget.custom.js. whenever searchable dropdownlist's selected item changed
    /// </summary>
    [JSInvokable]
    public void OnJSDropDownListSelectedItemChanged(object? e) => SetSelectedItemAsync(e);

    protected async Task OnSelectedItemChangedAsync(object? e)
    {
        Value = e;

        SelectedItem = e == null ? null : Items.FirstOrDefault(x => x.Value?.ToString() == e.ToString());

        StateHasChanged();
        await OnChange.InvokeAsync(new ChangeEventArgs { Value = e });
    }
    protected internal bool HasSelectedValue() =>
        SelectedItem != null &&
        !string.IsNullOrWhiteSpace(SelectedItem.Value?.ToString()) &&
        !SelectedItem.Value.HasDefaultValue();
}
