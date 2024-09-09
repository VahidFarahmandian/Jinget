namespace Jinget.Blazor.Components;

public abstract class JingetBaseComponent : ComponentBase
{
    [Inject] protected IJSRuntime JS { get; set; }

    /// <summary>
    /// Component identifier. default is set to <code>Guid.NewGuid().ToString("N")</code>
    /// </summary>
    [Parameter] public virtual required string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// if set to true, then element will be rendered in right to left direction. default is true.
    /// </summary>
    [Parameter] public bool IsRtl { get; set; } = true;

    [Parameter] public string? DisplayName { get; set; }

    /// <summary>
    /// Css class used for label(display name) element  
    /// </summary>
    [Parameter] public string? LabelCssClass { get; set; }

    [Parameter] public virtual string? CssClass { get; set; } = "form-control";

    /// <summary>
    /// if set to true, then component will be disabled.
    /// </summary>
    [Parameter] public bool IsDisabled { get; set; }

    /// <summary>
    /// if set to true, then component will be rendered as readonly.
    /// </summary>
    [Parameter] public bool IsReadOnly { get; set; }

    [Parameter] public string? HelperText { get; set; }

    /// <summary>
    /// Css class used for helper text element
    /// </summary>
    [Parameter] public string? HelperTextCss { get; set; } = "form-text text-muted";

    /// <summary>
    /// if set to true, then component will be rendered as required input.
    /// </summary>
    [Parameter] public bool IsRequired { get; set; } = false;

    /// <summary>
    /// MEssage to be shown whenever the <seealso cref="IsRequired"/> is true, and no value is supplied for component
    /// </summary>
    [Parameter] public string? RequiredError { get; set; } = "*";

    [Parameter] public string? RequiredErrorCssClass { get; set; }

    private object? _value;
    /// <summary>
    /// Component's value. This property is two-way bindable
    /// </summary>
    [Parameter]
    public virtual object? Value
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
    [Parameter] public virtual EventCallback<object?> ValueChanged { get; set; }

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
