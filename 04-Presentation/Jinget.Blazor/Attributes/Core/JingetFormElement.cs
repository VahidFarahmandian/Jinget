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
    /// Defines whether to show the label for element or not
    /// </summary>
    public bool HasLabel { get; set; } = false;

    /// <summary>
    /// CSS class for label element
    /// </summary>
    public string LabelCssClass { get; set; }

    /// <summary>
    /// Css class for parent div. default is 'form-horizontal'
    /// </summary>
    public string DivCssClass { get; set; } = "form-horizontal";

    /// <summary>
    /// Css class for element itself. default is 'form-control'
    /// </summary>
    public string CssClass { get; set; } = "form-control";

    //public bool IsVisible { get; set; } = true;
    public bool IsDisabled { get; set; } = false;
    public bool IsReadOnly { get; set; } = false;
    public bool IsRequired { get; set; } = false;
    public string RequiredError { get; set; } = "*";
    public string HelperText { get; set; }

}