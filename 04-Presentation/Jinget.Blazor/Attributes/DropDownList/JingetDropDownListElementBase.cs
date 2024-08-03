namespace Jinget.Blazor.Attributes.DropDownList;

public abstract class JingetDropDownListElementBase : JingetFormElement
{
    /// <summary>
    /// Defines where to attach the dropdown html in page. By default element will be attached to body tag in page.
    /// But in some cases like Bootstrap modals, which tend to steal focus from other elements outside of the modal. 
    /// Since by default, Select2 attaches the dropdown menu to the body element, it is considered "outside of the modal". 
    /// To avoid this problem, you may attach the dropdown to the modal itself by setting the modal id in this parameter.
    /// </summary>
    [Parameter] public string ParentElementId { get; set; } = "";

    /// <summary>
    /// Default string used to be shown in dropdownlist, whenever user choose nothing.
    /// </summary>
    public string DefaultText { get; set; } = "---Choose---";

    /// <summary>
    /// if set to true, then dropdownllist will have searching mechanism
    /// </summary>
    public bool IsSearchable { get; set; }

    /// <summary>
    /// if set to true, then dropdownllist items will be rendered in right to left direction.
    /// default is <see cref="true"/>
    /// </summary>
    public bool IsRtl { get; set; } = true;

    /// <summary>
    /// Text used to be shown when search returns no result, while using searchable DropDownListTree
    /// </summary>
    public string NoResultText { get; set; } = "Nothing to display!";

    /// <summary>
    /// Placeholder text used for search input element, while using searchable DropDownListTree
    /// </summary>
    public string SearchPlaceholderText { get; set; } = "";

    /// <summary>
    /// set a method which will be called automatically while binding the element
    /// </summary>
    public string? BindingFunction { get; set; }

    /// <summary>
    /// set a method which will be called automatically while binding the element and before calling the <seealso cref="BindingFunction"/>
    /// </summary>
    public string? PreBindingFunction { get; set; }

    /// <summary>
    /// set a method which will be called automatically while binding the element and after calling the <seealso cref="BindingFunction"/>
    /// </summary>
    public string? PostBindingFunction { get; set; }
}