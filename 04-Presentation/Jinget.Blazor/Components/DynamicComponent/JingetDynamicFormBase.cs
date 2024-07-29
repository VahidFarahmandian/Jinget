using Jinget.Blazor.Components.Picker;

namespace Jinget.Blazor.Components.DynamicComponent;

public class JingetDynamicFormBase<T> : ComponentBase
{
    protected internal string style = "";

    /// <summary>
    /// Model which is used to bind to the elements
    /// </summary>
    [Parameter] public T? Model { get; set; }

    /// <summary>
    /// Defines whether the form should be right-to-left or not
    /// </summary>
    [Parameter] public bool Rtl { get; set; } = true;

    /// <summary>
    /// Custom css style used for the whole form
    /// </summary>
    [Parameter] public string CustomStyle { get; set; }

    /// <summary>
    /// This event raised whenever a member's value changed
    /// </summary>
    [Parameter] public EventCallback<ObjectChangedModel<T>> OnModelChanged { get; set; }

    /// <summary>
    /// This event raised whenever a field rendered on page
    /// </summary>
    [Parameter] public EventCallback<JingetDynamicFieldBase> OnFieldReady { get; set; }

    /// <summary>
    /// return the list of fields rendered on page
    /// </summary>
    public List<JingetDynamicFieldBase> DynamicFields { get; private set; } = [];

    /// <summary>
    /// return the list of elements used to rednder the dynamic form
    /// </summary>
    public HashSet<(PropertyInfo Property, JingetFormElement? Attribute)> Properties { get; set; } = new();

    protected override void OnInitialized()
    {
        IOrderedEnumerable<PropertyInfo> props = typeof(T).GetProperties()
            .Where(x => x.IsDefined(typeof(JingetFormElement)))
            .OrderBy(x => x.GetCustomAttribute<JingetFormElement>().Order);
        foreach (var p in props)
        {
            Properties.Add(new(
                p,
                p.GetCustomAttribute<JingetFormElement>()));
        }
        style = Rtl ? "font-family:IRANSans" : "";
    }

    protected async Task OnDynamicFieldAdded(JingetDynamicFieldBase field)
    {
        DynamicFields.RemoveAll(x => (x.Binding != null && x.Binding.Name == field.Binding.Name) || x.Id == field.Id);
        DynamicFields.Add(field);
        await OnFieldReady.InvokeAsync(field);
    }

    public List<JingetDynamicFieldBase> FindElement(string id)
    {
        return DynamicFields.Where(x => x.Id == id).ToList();
    }
    public List<JingetDynamicFieldBase> FindElement<TFormElementType>() where TFormElementType : JingetFormElement
    {
        return DynamicFields.Where(x => x.Attribute is TFormElementType).ToList();
    }

    protected void OnValueChanged(string prop, object? value)
    {
        if (Model != null)
        {
            var p = Model.GetType().GetProperty(prop);
            var previousValue = p?.GetValue(Model);
            object? convertedValue = null;

            try
            {
                Type t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                if (value is not IConvertible && t == typeof(string))
                {
                    value = value == null ? null : value?.ToString();
                }

                if (value is SelectedDateRangeModel dateRangeValue)
                {
                    if (t == typeof(DateRange))
                    {
                        convertedValue = Convert.ChangeType(dateRangeValue.DateRange, t);
                    }
                    else
                    {
                        convertedValue = Convert.ChangeType(dateRangeValue, t);
                    }
                }
                else
                {
                    if (value is IConvertible)
                    {
                        convertedValue = (value == null) ? null : Convert.ChangeType(value, t);
                    }
                    else
                    {
                        convertedValue = (value == null) ? null : Convert.ChangeType(value.ToString(), t);
                    }
                }
                p?.SetValue(Model, convertedValue);
                StateHasChanged();
            }
            catch { }
            OnModelChanged.InvokeAsync(new ObjectChangedModel<T>(Model, prop, previousValue, value, convertedValue));
        }
    }

    protected internal string GetId(JingetFormElement? element)
    {
        return string.IsNullOrWhiteSpace(element?.Id) ? $"jinget-{element.GetHashCode()}" : element.Id;
    }
    protected internal object? GetValue(PropertyInfo property)
    {
        object? value = null;
        if (Model != null)
            value = property.DeclaringType == typeof(string) && property.GetValue(Model) == null ? "" : property.GetValue(Model);
        return value;
    }
}
