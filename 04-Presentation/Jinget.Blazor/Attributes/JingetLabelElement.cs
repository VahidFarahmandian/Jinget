namespace Jinget.Blazor.Attributes;

public class JingetLabelElement : JingetFormElement
{
    /// <summary>
    /// While rendering a label on page, if HelperText is set, then
    /// a 'small' html element will also rendered to display the helper text.
    /// This property is used to set a css class for this 'small' html tag
    /// </summary>
    public string? HelperTextCss { get; set; }
}