using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;

namespace Rrs.Enumerables
{
    public delegate bool TryFunc<in TSource, TResult>(TSource arg, out TResult result);

    public static class EnumerableExtensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
        {
            return new HashSet<T>(enumerable);
        }

        public static HashSet<TOut> ToHashSet<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, TOut> func)
        {
            return new HashSet<TOut>(enumerable.ForEach(o => func(o)));
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> a)
        {
            foreach(var e in enumerable)
            {
                a(e);
                yield return e;
            }
        }

        public static IEnumerable<TOut> ForEach<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, TOut> func)
        {
            foreach (var e in enumerable)
            {
                yield return func(e);
            }
        }

        public static IEnumerable<IDictionary<string, object>> ToDictionary(this IEnumerable<DataRow> rows)
        {
            return rows.Select(row => row.ToDictionary());
        }

        public static IDictionary<string, object> ToDictionary(this DataRow row)
        {
            return row.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row.IsNull(col.ColumnName) ? null : row[col.ColumnName]);
        }

        public static IEnumerable<OrderedDictionary> ToOrderedDictionary(this IEnumerable<DataRow> rows)
        {
            return rows.Select(row => row.ToOrderedDictionary());
        }

        public static OrderedDictionary ToOrderedDictionary(this DataRow row)
        {
            var orderedDictionary = new OrderedDictionary();

            var keyValues = row.Table.Columns.Cast<DataColumn>().Select(col => new { key = col.ColumnName, value = row.IsNull(col.ColumnName) ? null : row[col.ColumnName] });

            foreach (var kv in keyValues)
            {
                orderedDictionary.Add(kv.key, kv.value);
            }

            return orderedDictionary;
        }

        public static IEnumerable<IDictionary<string, object>> ToDictionary(this DataTable table)
        {
            return table.Select().ToDictionary();
        }

        public static IEnumerable<OrderedDictionary> ToOrderedDictionary(this DataTable table)
        {
            return table.Select().ToOrderedDictionary();
        }

        public static IEnumerable<TOut> SelectTry<TIn, TOut>(this IEnumerable<TIn> enumerable, TryFunc<TIn, TOut> selector)
        {
            foreach (var v in enumerable)
            {
                if (selector(v, out var r))
                    yield return r;
            }
        }

        public static void Spliterate<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Action<T> included, Action<T> excluded)
        {
            foreach (var e in enumerable)
            {
                if (predicate(e))
                    included(e);
                else
                    excluded(e);
            }
        }

        public static (IEnumerable<TOutIncluded> included, IEnumerable<TOutExcluded> excluded) Spliterate<T, TOutIncluded, TOutExcluded>(this IEnumerable<T> enumerable, Predicate<T> predicate, Func<T, TOutIncluded> includeSelector, Func<T, TOutExcluded> excludeSelector)
        {
            var included = new List<TOutIncluded>();
            var excluded = new List<TOutExcluded>();
            foreach (var e in enumerable)
            {
                if (predicate(e))
                    included.Add(includeSelector(e));
                else
                    excluded.Add(excludeSelector(e));
            }
            return (included, excluded);
        }

        public static (IEnumerable<TOut> included, IEnumerable<TOut> excluded) Spliterate<T, TOut>(this IEnumerable<T> enumerable, Predicate<T> predicate, Func<T, TOut> selector)
        {
            var included = new List<TOut>();
            var excluded = new List<TOut>();
            foreach (var e in enumerable)
            {
                if (predicate(e))
                    included.Add(selector(e));
                else
                    excluded.Add(selector(e));
            }
            return (included, excluded);
        }
    }
}
