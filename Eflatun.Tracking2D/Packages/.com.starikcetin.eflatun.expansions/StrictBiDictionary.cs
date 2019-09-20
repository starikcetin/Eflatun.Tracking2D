using System;
using System.Collections.Generic;

namespace starikcetin.Eflatun.Expansions
{
    /// <summary>
    /// A dictionary type which allows bi-directional lookups. <para />
    /// This type allows only one first-second match (1-to-1 relation), thus lookup results are always single values. <para />
    /// A <see cref="NotSupportedException"/> will be thrown if there is an attempt to add more than one first-second match.
    /// </summary>
    /// <seealso cref="SafeBiDictionary{TFirst, TSecond}"/>
    public class StrictBiDictionary<TFirst, TSecond> : IEnumerable<KeyValuePair<TFirst, TSecond>>
    {
        //
        // Originally written by Jon Skeet on https://stackoverflow.com/a/255638/6301627
        // I edited a little bit to separate 1-to-1 and many-to-many implementations
        //

        private readonly IDictionary<TFirst, TSecond> _firstToSecond;
        private readonly IDictionary<TSecond, TFirst> _secondToFirst;

        public IDictionary<TFirst, TSecond> FirstToSecond
        {
            get { return _firstToSecond; }
        }

        public ICollection<TFirst> Firsts
        {
            get { return _firstToSecond.Keys; }
        }

        public IDictionary<TSecond, TFirst> SecondToFirst
        {
            get { return _secondToFirst; }
        }

        public ICollection<TSecond> Seconds
        {
            get { return _secondToFirst.Keys; }
        }

        public int Count
        {
            get { return _firstToSecond.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StrictBiDictionary{TFirst, TSecond}"/> class.
        /// </summary>
        public StrictBiDictionary()
        {
            EnsureTypesDifferent();

            _firstToSecond = new Dictionary<TFirst, TSecond>();
            _secondToFirst = new Dictionary<TSecond, TFirst>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StrictBiDictionary{TFirst, TSecond}"/> class with a custom equality comparer for TFirst.
        /// </summary>
        /// <param name="firstComparer">Custom equality comparer for TFirst.</param>
        public StrictBiDictionary(IEqualityComparer<TFirst> firstComparer)
        {
            EnsureTypesDifferent();

            _firstToSecond = new Dictionary<TFirst, TSecond>(firstComparer);
            _secondToFirst = new Dictionary<TSecond, TFirst>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StrictBiDictionary{TFirst, TSecond}"/> class with a custom equality comparer for TSecond.
        /// </summary>
        /// <param name="secondComparer">Custom equality comparer for TSecond.</param>
        public StrictBiDictionary(IEqualityComparer<TSecond> secondComparer)
        {
            EnsureTypesDifferent();

            _firstToSecond = new Dictionary<TFirst, TSecond>();
            _secondToFirst = new Dictionary<TSecond, TFirst>(secondComparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StrictBiDictionary{TFirst, TSecond}"/> class with custom equality comparers for both TFirst and TSecond.
        /// </summary>
        /// <param name="firstComparer">Custom equality comparer for TFirst.</param>
        /// <param name="secondComparer">Custom equality comparer for TSecond.</param>
        public StrictBiDictionary(IEqualityComparer<TFirst> firstComparer, IEqualityComparer<TSecond> secondComparer)
        {
            EnsureTypesDifferent();

            _firstToSecond = new Dictionary<TFirst, TSecond>(firstComparer);
            _secondToFirst = new Dictionary<TSecond, TFirst>(secondComparer);
        }

        private static void EnsureTypesDifferent()
        {
            if (typeof(TFirst) == typeof(TSecond))
            {
                throw new NotSupportedException("TFirst and TSecond cannot be the same type.");
            }
        }

        // Note potential ambiguity using indexers (e.g. mapping from int to int)
        // Hence the methods as well...
        public TSecond this[TFirst first]
        {
            get { return GetByFirst(first); }
        }

        public TFirst this[TSecond second]
        {
            get { return GetBySecond(second); }
        }

        public void Add(TFirst first, TSecond second)
        {
            if (_firstToSecond.ContainsKey(first) ||
                _secondToFirst.ContainsKey(second))
            {
                throw new NotSupportedException(
                    "Duplicate firsts or seconds are not supported in StrictBiDictionary type. Consider using SafeBiDictionary if you need multiple first-second matches.");
            }

            _firstToSecond.Add(first, second);
            _secondToFirst.Add(second, first);
        }

        public bool TryAdd(TFirst first, TSecond second)
        {
            if (_firstToSecond.ContainsKey(first) ||
                _secondToFirst.ContainsKey(second))
            {
                return false;
            }

            _firstToSecond.Add(first, second);
            _secondToFirst.Add(second, first);
            return true;
        }

        public TSecond GetByFirst(TFirst first)
        {
            return _firstToSecond[first];
        }

        public TFirst GetBySecond(TSecond second)
        {
            return _secondToFirst[second];
        }

        public bool ContainsFirst(TFirst first)
        {
            return _firstToSecond.ContainsKey(first);
        }

        public bool ContainsSecond(TSecond second)
        {
            return _secondToFirst.ContainsKey(second);
        }

        public bool TryGetByFirst(TFirst first, out TSecond second)
        {
            return _firstToSecond.TryGetValue(first, out second);
        }

        public bool TryGetBySecond(TSecond second, out TFirst first)
        {
            return _secondToFirst.TryGetValue(second, out first);
        }

        public bool RemoveByFirst(TFirst first)
        {
            _secondToFirst.Remove(_firstToSecond[first]);
            return _firstToSecond.Remove(first);
        }

        public bool RemoveBySecond(TSecond second)
        {
            _firstToSecond.Remove(_secondToFirst[second]);
            return _secondToFirst.Remove(second);
        }

        public void Clear()
        {
            _firstToSecond.Clear();
            _secondToFirst.Clear();
        }

        #region IEnumerable implementation

        public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator()
        {
            return _firstToSecond.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
