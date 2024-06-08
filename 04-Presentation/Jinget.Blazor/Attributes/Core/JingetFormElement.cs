using Jinget.Blazor.Attributes.Input;
using System.Runtime.CompilerServices;

namespace Jinget.Blazor.Attributes.Core;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public abstract class JingetFormElement([CallerMemberName] string? propertyName = null) : Attribute
{
    public string? PropertyName { get; } = propertyName;

    /// <summary>
    /// Used as form element id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// form elements are being rendered on screen based on their order.
    /// If no order is specified, then default order will be assigned to it. default value is int.MaxValue;
    /// </summary>
    public int Order { get; set; } = int.MaxValue;

    /// <summary>
    /// Used as form element label text
    /// </summary>
    public string? DisplayName { get; set; } = null;

    /// <summary>
    /// CSS class. default is 'form-control'
    /// </summary>
    public string LabelCssClass { get; set; } = "col-3";
    public bool HasLabel { get; set; } = false;
    public string DivCssClass { get; set; } = "col-3";
    public string CssClass { get; set; } = "form-control";
    //public bool IsVisible { get; set; } = true;
    public bool IsDisabled { get; set; } = false;
    public bool IsReadOnly { get; set; } = false;
    public string HelperText { get; set; }

}