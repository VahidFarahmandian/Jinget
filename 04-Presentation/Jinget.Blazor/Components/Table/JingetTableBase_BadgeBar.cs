
namespace Jinget.Blazor.Components.Table
{
    public abstract partial class JingetTableBase<T> : JingetBaseComponent
    {
        /// <summary>
        /// Defines whether to show to badge bar or not
        /// </summary>
        [Parameter] public bool ShowBadgeBar { get; set; } = true;
    }
}
