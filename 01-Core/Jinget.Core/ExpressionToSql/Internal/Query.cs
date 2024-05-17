namespace Jinget.Core.ExpressionToSql.Internal;

public abstract class Query
{
    public (QueryBuilder query, Dictionary<string, object?>? parameters) ToSql() => ToSql(new StringBuilder());

    private (QueryBuilder query, Dictionary<string, object?>? parameters) ToSql(StringBuilder sb)
    {
        var (query, parameters) = ToSql(new QueryBuilder(sb));
        return (query, parameters);
    }

    internal abstract (QueryBuilder query, Dictionary<string, object?>? parameters) ToSql(QueryBuilder query);
}