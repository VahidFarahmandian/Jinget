using Jinget.Core.ExpressionToSql.Internal;
using System;
using System.Linq.Expressions;

namespace Jinget.Core.ExpressionToSql
{
    public static class Sql
    {
        public static Select<T, R> Select<T, R>(Expression<Func<T, R>> selector, string tableName) => Create(selector, null, tableName);

        public static Select<T, R> Select<T, R>(Expression<Func<T, R>> selector, Table table) => Create(selector, null, table);

        public static Select<T, R> Top<T, R>(Expression<Func<T, R>> selector, int take, string tableName) => Create(selector, take, tableName);

        public static Select<T, R> Top<T, R>(Expression<Func<T, R>> selector, int take, Table table) => Create(selector, take, table);

        private static Select<T, R> Create<T, R>(Expression<Func<T, R>> selector, int? take, string tableName) => Create(selector, take, new Table { Name = tableName });

        private static Select<T, R> Create<T, R>(Expression<Func<T, R>> selector, int? take, Table table) => new(selector, take, table);
    }
}
