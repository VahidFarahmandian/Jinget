using System.ComponentModel;

namespace Jinget.Blazor.Enums;

public enum Severity
{
    [Description("primary")]
    Normal,
    [Description("info")]
    Info,
    [Description("success")]
    Success,
    [Description("warning")]
    Warning,
    [Description("danger")]
    Error
}
