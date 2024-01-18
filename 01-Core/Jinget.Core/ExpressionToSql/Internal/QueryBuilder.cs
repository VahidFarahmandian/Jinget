using System;
using System.Text;

namespace Jinget.Core.ExpressionToSql.Internal
{
    public class QueryBuilder
    {
        private readonly StringBuilder _sb;
        public const string AliasName = "a";

        public QueryBuilder(StringBuilder sb)
        {
            if (sb.Length == 0)
            {
                sb.Append("SELECT");
            }
            _sb = sb;
        }

        public override string ToString() => _sb.ToString();

        /// <summary>
        /// create TOP clause
        /// </summary>
        public QueryBuilder Take(int count)
        {
            if (count >= 0)
                _sb.Append(" TOP ").Append(count);
            return this;
        }

        public QueryBuilder AddParameter(string parameterName)
        {
            if (!string.IsNullOrWhiteSpace(parameterName))
                _sb.Append(" @").Append(parameterName);
            return this;
        }

        /// <summary>
        /// Adds aliasname and column names
        /// </summary>
        public QueryBuilder AddAttribute(string attributeName, string aliasName = AliasName)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
                return this;

            _sb.Append(' ');

            if (!string.IsNullOrWhiteSpace(aliasName))
            {
                _sb.Append(aliasName).Append('.');
            }

            AppendEscapedValue(attributeName);
            return this;
        }

        public QueryBuilder AddValue(object? value)
        {
            _sb.Append(' ').Append(value);
            return this;
        }

        /// <summary>
        /// Adds comma separator.
        /// </summary>
        public QueryBuilder AddSeparator()
        {
            _sb.Append(',');
            return this;
        }

        public QueryBuilder Remove(int count = 1)
        {
            if (count > 0)
                _sb.Length -= count;
            return this;
        }

        /// <summary>
        /// Create FROM Table clause
        /// </summary>
        public QueryBuilder AddTable(Table table, string aliasName = AliasName)
        {
            if (table == null)
                return this;

            _sb.Append(" FROM ");

            if (!string.IsNullOrWhiteSpace(table.Schema))
            {
                AppendEscapedValue(table.Schema);
                _sb.Append('.');
            }

            AppendEscapedValue(table.Name);

            if (!string.IsNullOrWhiteSpace(aliasName))
            {
                _sb.Append(" AS ").Append(aliasName);
            }
            return this;
        }

        internal void AppendCondition(string condition)
        {
            if (!string.IsNullOrWhiteSpace(condition))
                _sb.Append(" WHERE ").Append(condition);
        }

        /// <summary>
        /// Add [ and ] to column, table and function names
        /// </summary>
        private void AppendEscapedValue(string attributeName)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
                return;

            if (attributeName.StartsWith("[") && attributeName.EndsWith("]"))
            {
                _sb.Append(attributeName);
                return;
            }

            // Table value function. [ and ] should added to function name only, not to its parameters 
            if (attributeName.Contains('(') && attributeName.Contains(')'))
            {
                string functionName = attributeName[..attributeName.IndexOf("(", StringComparison.Ordinal)];
                string functionParameters = attributeName[attributeName.IndexOf("(", StringComparison.Ordinal)..];
                _sb.Append('[').Append(functionName).Append(']').Append(functionParameters);
            }
            else
                _sb.Append('[').Append(attributeName).Append(']');
        }
    }
}