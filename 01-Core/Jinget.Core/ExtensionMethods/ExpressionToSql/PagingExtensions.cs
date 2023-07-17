using System.Collections.Generic;
using Jinget.Core.Exceptions;
using Jinget.Core.ExpressionToSql.Internal;

namespace Jinget.Core.ExtensionMethods.ExpressionToSql
{
    public static class PagingExtensions
    {
        public static string GetPaging(this Paging paging, List<OrderBy> lstOrderBy)
        {
            string strOrderby = lstOrderBy.GetSorting();

            if (string.IsNullOrEmpty(strOrderby) && paging != null)
                throw new JingetException("In order to use paging, you should specify at least one order by expression.", 4000);

            return $"{strOrderby} {(paging == null ? "" : paging.ToString())}";
        }
    }
}
