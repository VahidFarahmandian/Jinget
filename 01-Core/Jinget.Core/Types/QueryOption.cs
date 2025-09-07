namespace Jinget.Core.Types;

public class QueryOption<TModelType> where TModelType : class, IEntity
{
    public List<Expression<Func<TModelType, object>>>? IncludedNavigations { get; set; } = [];
    public Expression<Func<TModelType, object>>? Columns { get; set; } = null;
    public Expression<Func<TModelType, bool>>? Filter { get; set; } = null;

    /// <summary>
    /// if both `PageIndex` and `PageSize` set to  zero or lower than zero, then pagination will not be applied and all records will be returned. 
    /// </summary>
    public PaginationOptionModel PaginationOption { get; set; } = new();
    public SortOptionModel SortOption { get; set; } = new();
    public MiscOptionModel MiscOption { get; set; } = new();

    public EntryCacheOptions? CacheOptions { get; set; } = null;

    public class PaginationOptionModel
    {
        /// <summary>
        /// Page's index are started from 1
        /// </summary>
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 0;
    }
    public class SortOptionModel
    {
        public Func<IQueryable<TModelType>, IOrderedQueryable<TModelType>>? SortExpression { get; set; } = null;
        public string SortColumn { get; set; } = "";
        public string SortDirection { get; set; } = "";
    }
    public class MiscOptionModel
    {
        public bool AsSplitQuery { get; set; } = true;
        public bool ApplyTenantColumnFilter { get; set; } = true;
        public bool CalcTotalCount { get; set; } = true;
        public bool IgnoreAutoIncludes { get; set; } = true;
        public TemporalOptionModel? TemporalOption { get; set; } = null;
    }

    public class TemporalOptionModel
    {
        public TemporalType Type { get; set; } = TemporalType.All;
        public DateTime UtcFrom { get; set; }
        public DateTime UtcTo { get; set; }

        public enum TemporalType : byte
        {
            /// <summary>
            /// Returns all rows in the historical data.
            /// </summary>
            All,

            /// <summary>
            /// Returns all rows that were active between two given UTC times.
            /// </summary>
            FromTo,

            /// <summary>
            /// The same as <seealso cref="FromTo"/>, except that rows are included that became active on the upper boundary.
            /// </summary>
            Between,

            /// <summary>
            /// Returns rows that were active (current) at the given UTC time. This is a single row from the history table for a given primary key.
            /// </summary>
            AsOf,

            /// <summary>
            /// Returns all rows that started being active and ended being active between two given UTC times.
            /// </summary>
            ContainedIn
        }
    }
}
