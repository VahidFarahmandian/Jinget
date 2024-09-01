using Jinget.Blazor.Models.JingetTable;

namespace Jinget.Blazor.Services;

public class JingetTableObjectFactory<T>
{
    public sealed class EmptyTableData : JingetTableModel<T>
    {
        static readonly Lazy<EmptyTableData> lazy = new(() => new EmptyTableData());

        public static EmptyTableData Instance => lazy.Value;

        private EmptyTableData() : base(Array.Empty<T>(), 0) { }
    }
}

