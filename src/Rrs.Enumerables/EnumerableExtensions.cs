﻿using System;
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

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> a) => enumerable.ForEach((o, i) => a(o));

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> a)
        {
            foreach(var (e, i) in enumerable.WithIndex())
            {
                a(e, i);
                yield return e;
            }
        }

        public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> enumerable) => enumerable?.Select((o, i) => (o, i)) ?? Enumerable.Empty<(T, int)>();

        public static IEnumerable<TOut> ForEach<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, TOut> func) => enumerable.ForEach((o, i) => func(o));

        public static IEnumerable<TOut> ForEach<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, int, TOut> func)
        {
            foreach (var (e, i) in enumerable.WithIndex())
            {
                yield return func(e, i);
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
            return table.Rows.Cast<DataRow>().ToOrderedDictionary();
        }

        public static IEnumerable<TOut> SelectTry<TIn, TOut>(this IEnumerable<TIn> enumerable, TryFunc<TIn, TOut> selector)
        {
            foreach (var v in enumerable)
            {
                if (selector(v, out var r))
                    yield return r;
            }
        }

        public static Spliterator<T> Spliterate<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
            => new Spliterator<T>(enumerable, predicate);
    }
}
