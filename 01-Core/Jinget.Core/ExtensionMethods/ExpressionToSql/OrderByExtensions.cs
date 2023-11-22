using System.Collections.Generic;
using System.Linq;
using Jinget.Core.ExpressionToSql.Internal;

namespace Jinget.Core.ExtensionMethods.ExpressionToSql
{
    public static class OrderByExtensions
    {
        static string Stringfy(this List<OrderBy> lstOrderBy) =>
            lstOrderBy.Any()
                ? $"ORDER BY {string.Join(",", lstOrderBy.Select(x => x.ToString()))}"
                : string.Empty;

        public static string GetSorting(this List<OrderBy> lstOrderBy)
        {
            lstOrderBy ??= [];

            return lstOrderBy.Any() ? lstOrderBy.Stringfy() : string.Empty;
        }
    }
}
