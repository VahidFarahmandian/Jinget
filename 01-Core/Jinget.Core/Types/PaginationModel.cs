namespace Jinget.Core.Types
{
    /// <summary>
    /// represents a filter criteria. if no operator specified, then "=" operator will be used by default
    /// </summary>
    public class PaginationModel
    {
        private int page;
        private int pageSize;

        public required int Page { get => page < 0 ? 1 : page; set => page = value; }
        public required int PageSize { get => pageSize <= 0 || pageSize > 50 ? 10 : pageSize; set => pageSize = value; }
        public string? SearchString { get; set; }

    }
}
