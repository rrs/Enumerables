using System;
using System.Collections.Generic;

namespace Rrs.Enumerables
{
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
    }
}
