using System;
using System.Linq;
using System.Linq.Expressions;
using Jinget.Core.Enumerations;
using Jinget.Core.Exceptions;

namespace Jinget.Core.ExtensionMethods.Collections
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Sort a collection based on a property name. 
        /// This method is similar to 
        /// <seealso cref="Enumerable.OrderBy{TSource, TKey}(System.Collections.Generic.IEnumerable{TSource}, Func{TSource, TKey})"/> and 
        /// <seealso cref="Enumerable.OrderByDescending{TSource, TKey}(System.Collections.Generic.IEnumerable{TSource}, Func{TSource, TKey})"/> 
        /// methods except that this method sort the collection using property name.
        /// </summary>
        /// <remarks>If the given <paramref name="orderByMember"/> not found and also <paramref name="query"/> has 'null' value, then <paramref name="orderByMember"/>'s <see cref="ArgumentNullException"/> will have presedence over <see cref="NullReferenceException"/>.</remarks>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="JingetException"></exception>
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string orderByMember, OrderByDirection direction)
        {
            var queryElementTypeParam = Expression.Parameter(typeof(T));
            Expression memberAccess = queryElementTypeParam;
            foreach (var member in orderByMember.Split('.', StringSplitOptions.RemoveEmptyEntries))
            {
                memberAccess = Expression.PropertyOrField(memberAccess, member);
            }

            if (memberAccess is null)
                throw new JingetException($"Jinget Says: {orderByMember} not found in the given type", new NullReferenceException());

            var keySelector = Expression.Lambda(memberAccess, queryElementTypeParam);

            var orderBy = Expression.Call(
                typeof(Queryable),
                direction == OrderByDirection.Ascending ? "OrderBy" : "OrderByDescending",
                [typeof(T), memberAccess.Type],
                query.Expression,
                Expression.Quote(keySelector));

            return query.Provider.CreateQuery<T>(orderBy);
        }
    }
}