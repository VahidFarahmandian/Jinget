using Jinget.Core.Exceptions;

namespace Jinget.Core.ExtensionMethods.ExpressionToSql;

public static class PagingExtensions
{
    public static string GetPaging(this Paging paging, List<OrderBy>? lstOrderBy)
    {
        lstOrderBy ??= [];

        string strOrderby = lstOrderBy.GetSorting();

        if (string.IsNullOrEmpty(strOrderby) && paging != null)
            throw new JingetException("In order to use paging, you should specify at least one order by expression.", 4000);

        return $"{strOrderby} {(paging is null ? "" : paging.ToString())}";
    }
}
