namespace Jinget.Core.ExpressionToSql.Internal
{
    /// <summary>
    /// Provides the paging functionality used in query handling
    /// </summary>
    public class Paging
    {
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the OFFSET-FETCH T-SQL code.
        /// </summary>
        public override string ToString()
        {
            if (PageNumber == 0 && PageSize == 0)
                return string.Empty;

            if (PageNumber <= 0)
                PageNumber = 1;
            if (PageSize <= 0)
                PageSize = 10;

            return $"OFFSET {(PageNumber - 1) * PageSize} ROWS FETCH NEXT {PageSize} ROWS ONLY";
        }
    }
}
