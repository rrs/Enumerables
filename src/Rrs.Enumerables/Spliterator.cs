using System;
using System.Collections.Generic;

namespace Rrs.Enumerables
{
    public class Spliterator<T>
    {
        public delegate TOut SplitIndexedSelector<TIn, TOut>(TIn value, int linearIndex, int splitIndex);
        public delegate TOut SplitSelector<TIn, TOut>(TIn value);

        private readonly IEnumerable<T> _enumerable;
        private readonly Predicate<T> _predicate;
        internal Spliterator(IEnumerable<T> enumerable, Predicate<T> predicate) => (_enumerable, _predicate) = (enumerable, predicate);

        public (IEnumerable<TOut> included, IEnumerable<TOut> excluded) Select<TOut>(SplitSelector<T, TOut> selector) => Select(selector, selector);
        public (IEnumerable<TOut> included, IEnumerable<TOut> excluded) Select<TOut>(SplitIndexedSelector<T, TOut> selector) => Select(selector, selector);

        public (IEnumerable<TOutIncluded> included, IEnumerable<TOutExcluded> excluded) Select<TOutIncluded, TOutExcluded>(SplitSelector<T, TOutIncluded> includeSelector, SplitSelector<T, TOutExcluded> excludeSelector)
            => Select((e, i1, i2) => includeSelector(e), (e, i1, i2) => excludeSelector(e));

        public (IEnumerable<TOutIncluded> included, IEnumerable<TOutExcluded> excluded) Select<TOutIncluded, TOutExcluded>(SplitIndexedSelector<T, TOutIncluded> includeSelector, SplitIndexedSelector<T, TOutExcluded> excludeSelector)
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

        public (IEnumerable<T> included, IEnumerable<T> excluded) ToLists() => Select(o => o);

        public Spliterator<T> ForEach(Action<T> includedAction, Action<T> excludedAction)
        {
            foreach (var e in _enumerable)
            {
                if (_predicate(e))
                    includedAction(e);
                else
                    excludedAction(e);
            }

            return this;
        }


    }
}
