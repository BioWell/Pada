using System;
using System.Collections.Generic;
using System.Linq;

namespace Pada.Infrastructure.Utils
{
    public static class EnumerableHelper
    {
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> query, bool condition, Func<T, bool> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }

        public static IEnumerable<T> TakeIf<T, TKey>(this IEnumerable<T> query, Func<T, TKey> orderBy, bool condition,
            int limit, bool orderByDescending = true)
        {
            // It is necessary sort items before it
            query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return condition
                ? query.Take(limit)
                : query;
        }

        /// <summary>Indicates whether the specified enumerable is null or has a length of zero.</summary>
        /// <param name="data">The data to test.</param>
        /// <returns>true if the array parameter is null or has a length of zero; otherwise, false.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> data)
        {
            return data == null || !data.Any();
        }

        public static IEnumerable<IEnumerable<T>> Paginate<T>(this IEnumerable<T> items, int pageSize)
        {
            var page = new List<T>();
            foreach (var item in items)
            {
                page.Add(item);
                if (page.Count >= pageSize)
                {
                    yield return page;
                    page = new List<T>();
                }
            }

            if (page.Count > 0)
            {
                yield return page;
            }
        }

        /// <summary>
        /// Performs the indicated action on each item.
        /// </summary>
        /// <param name="action">The action to be performed.</param>
        /// <remarks>If an exception occurs, the action will not be performed on the remaining items.</remarks>
        public static void Apply<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// Performs the indicated action on each item. Boxing free for <c>List+Enumerator{T}</c>.
        /// </summary>
        /// <param name="action">The action to be performed.</param>
        /// <remarks>If an exception occurs, the action will not be performed on the remaining items.</remarks>
        public static void Apply<T>(this List<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// Performs the indicated action on each key-value pair.
        /// </summary>
        /// <param name="action">The action to be performed.</param>
        /// <remarks>If an exception occurs, the action will not be performed on the remaining items.</remarks>
        public static void Apply(this System.Collections.IDictionary items, Action<object, object> action)
        {
            foreach (var key in items.Keys)
            {
                action(key, items[key]);
            }
        }

        public static int GetOrderIndependentHashCode<T>(this IEnumerable<T> source)
        {
            int hash = 0;
            //Need to force order to get  order independent hash code
            foreach (T element in source.OrderBy(x => x, Comparer<T>.Default))
            {
                hash = hash ^ EqualityComparer<T>.Default.GetHashCode(element);
            }

            return hash;
        }
    }
}