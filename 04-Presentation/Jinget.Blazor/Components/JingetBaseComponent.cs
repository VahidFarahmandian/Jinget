namespace Jinget.Blazor.Components;

public abstract class JingetBaseComponent : ComponentBase
{
    [Inject] protected IJSRuntime JS { get; set; }

    /// <summary>
    /// Component identifier. default is set to <code>Guid.NewGuid().ToString("N")</code>
    /// </summary>
    [Parameter, EditorRequired] public required string Id { get; set; } = Guid.NewGuid().ToString("N");
    [Parameter] public string DisplayName { get; set; }
    [Parameter] public string CssClass { get; set; }
    
    /// <summary>
    /// if set to true, then component will be disabled.
    /// </summary>
    [Parameter] public bool IsDisabled { get; set; }

    /// <summary>
    /// if set to true, then component will be rendered as readonly.
    /// </summary>
    [Parameter] public bool IsReadOnly { get; set; }
    [Parameter] public string HelperText { get; set; }

    /// <summary>
    /// if set to true, then component will be rendered as required input.
    /// </summary>
    [Parameter] public bool IsRequired { get; set; } = false;
    
    /// <summary>
    /// MEssage to be shown whenever the <seealso cref="IsRequired"/> is true, and no value is supplied for component
    /// </summary>
    [Parameter] public string RequiredError { get; set; } = "*";

    private object? _value;
    /// <summary>
    /// Component's value. This property is two-way bindable
    /// </summary>
    [Parameter]
    public object? Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            ValueChanged.InvokeAsync(value);
        }
    }

    /// <summary>
    /// Call callback whenever the <seealso cref="Value"/> is changed.
    /// </summary>
    [Parameter] public EventCallback<object?> ValueChanged { get; set; }

    /// <summary>
    /// Call callback whenever the <seealso cref="Value"/> is changed.
    /// </summary>
    [Parameter] public EventCallback<ChangeEventArgs> OnChange { get; set; }

    /// <summary>
    /// Call callback whenever the component rendered on the page.
    /// </summary>
    [Parameter] public EventCallback OnRendered { get; set; }

    /// <summary>
    /// This field is used to prevent calling OnAfterRenderAsync befor completing OnInitializedAsync
    /// </summary>
    protected bool _initialized = false;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        //when base.OnInitializedAsync is completed then _initialized can set to true to notify children components to continue rendering
        _initialized = true;
    }
}
