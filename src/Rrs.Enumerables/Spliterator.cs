using System;
using System.Collections.Generic;

namespace Rrs.Enumerables
{
    public class Spliterator<T>
    {
        private readonly IEnumerable<T> _enumerable;
        private readonly Predicate<T> _predicate;
        internal Spliterator(IEnumerable<T> enumerable, Predicate<T> predicate) => (_enumerable, _predicate) = (enumerable, predicate);

        public (IEnumerable<TOut> included, IEnumerable<TOut> excluded) Select<TOut>(Func<T, TOut> selector) => Select(selector, selector);
        public (IEnumerable<TOut> included, IEnumerable<TOut> excluded) Select<TOut>(Func<T, int, int, TOut> selector) => Select(selector, selector);

        public (IEnumerable<TOutIncluded> included, IEnumerable<TOutExcluded> excluded) Select<TOutIncluded, TOutExcluded>(Func<T, TOutIncluded> includeSelector, Func<T, TOutExcluded> excludeSelector)
            => Select((e, i1, i2) => includeSelector(e), (e, i1, i2) => excludeSelector(e));


        public (IEnumerable<TOutIncluded> included, IEnumerable<TOutExcluded> excluded) Select<TOutIncluded, TOutExcluded>(Func<T, int, int, TOutIncluded> includeSelector, Func<T, int, int, TOutExcluded> excludeSelector)
        {
            var linearIndex = 0;
            var includedIndex = 0;
            var excludedIndex = 0;

            var included = new List<TOutIncluded>();
            var excluded = new List<TOutExcluded>();
            foreach (var e in _enumerable)
            {
                if (_predicate(e))
                    included.Add(includeSelector(e, linearIndex++, includedIndex++));
                else
                    excluded.Add(excludeSelector(e, linearIndex++, excludedIndex++));
            }
            return (included, excluded);
        }

        public void ForEach(Action<T> includedAction, Action<T> excludedAction)
        {
            foreach (var e in _enumerable)
            {
                if (_predicate(e))
                    includedAction(e);
                else
                    excludedAction(e);
            }
        }
    }
}
