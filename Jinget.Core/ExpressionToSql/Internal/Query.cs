using System.Collections.Generic;
using System.Text;

namespace Jinget.Core.ExpressionToSql.Internal
{
    public abstract class Query
    {
        public (QueryBuilder query, Dictionary<string, object> parameters) ToSql() => ToSql(new StringBuilder());

        private (QueryBuilder query, Dictionary<string, object> parameters) ToSql(StringBuilder sb)
        {
            var result = ToSql(new QueryBuilder(sb));
            return (result.query, result.parameters);
        }

        internal abstract (QueryBuilder query, Dictionary<string, object> parameters) ToSql(QueryBuilder query);
    }
}