using System;
using System.Collections.Generic;

namespace starikcetin.Eflatun.Expansions
{
    /// <summary>
    /// A dictionary type which allows bi-directional lookups. <para />
    /// This type allows multiple first-second matches (many-to-many relation); thus lookup results are lists containing all found matches.
    /// </summary>
    /// <seealso cref="StrictBiDictionary{TFirst, TSecond}"/>
    public class SafeBiDictionary<TFirst, TSecond> : IEnumerable<KeyValuePair<TFirst, IList<TSecond>>>
    {
        //
        // Originally written by Jon Skeet on https://stackoverflow.com/a/255638/6301627
        // I edited a little bit to separate 1-to-1 and many-to-many implementations
        //

        private static readonly IList<TFirst> EmptyFirstList = new TFirst[0];
        private static readonly IList<TSecond> EmptySecondList = new TSecond[0];

        private readonly IDictionary<TFirst, IList<TSecond>> _firstToSecond;
        private readonly IDictionary<TSecond, IList<TFirst>> _secondToFirst;

        public IDictionary<TFirst, IList<TSecond>> FirstToSecond
        {
            get { return _firstToSecond; }
        }

        public ICollection<TFirst> Firsts
        {
            get { return _firstToSecond.Keys; }
        }

        public IDictionary<TSecond, IList<TFirst>> SecondToFirst
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
        /// Initializes a new instance of the <see cref="SafeBiDictionary{TFirst, TSecond}"/> class.
        /// </summary>
        public SafeBiDictionary()
        {
            EnsureTypesDifferent();

            _firstToSecond = new Dictionary<TFirst, IList<TSecond>>();
            _secondToFirst = new Dictionary<TSecond, IList<TFirst>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeBiDictionary{TFirst, TSecond}"/> class 
        /// with a custom equality comparer for <typeparamref name="TFirst"/>.
        /// </summary>
        /// <param name="firstComparer">Custom equality comparer for TFirst.</param>
        public SafeBiDictionary(IEqualityComparer<TFirst> firstComparer)
        {
            EnsureTypesDifferent();

            _firstToSecond = new Dictionary<TFirst, IList<TSecond>>(firstComparer);
            _secondToFirst = new Dictionary<TSecond, IList<TFirst>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeBiDictionary{TFirst, TSecond}"/> class
        /// with a custom equality comparer for <typeparamref name="TSecond"/>.
        /// </summary>
        /// <param name="secondComparer">Custom equality comparer for TSecond.</param>
        public SafeBiDictionary(IEqualityComparer<TSecond> secondComparer)
        {
            EnsureTypesDifferent();

            _firstToSecond = new Dictionary<TFirst, IList<TSecond>>();
            _secondToFirst = new Dictionary<TSecond, IList<TFirst>>(secondComparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeBiDictionary{TFirst, TSecond}"/> class
        /// with custom equality comparers for both <typeparamref name="TFirst"/> and <typeparamref name="TSecond"/>.
        /// </summary>
        /// <param name="firstComparer">Custom equality comparer for TFirst.</param>
        /// <param name="secondComparer">Custom equality comparer for TSecond.</param>
        public SafeBiDictionary(IEqualityComparer<TFirst> firstComparer, IEqualityComparer<TSecond> secondComparer)
        {
            EnsureTypesDifferent();

            _firstToSecond = new Dictionary<TFirst, IList<TSecond>>(firstComparer);
            _secondToFirst = new Dictionary<TSecond, IList<TFirst>>(secondComparer);
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
        public IList<TSecond> this[TFirst first]
        {
            get { return GetByFirst(first); }
        }

        public IList<TFirst> this[TSecond second]
        {
            get { return GetBySecond(second); }
        }

        public void Add(TFirst first, TSecond second)
        {
            IList<TFirst> firsts;
            IList<TSecond> seconds;
            if (!_firstToSecond.TryGetValue(first, out seconds))
            {
                seconds = new List<TSecond>();
                _firstToSecond[first] = seconds;
            }

            if (!_secondToFirst.TryGetValue(second, out firsts))
            {
                firsts = new List<TFirst>();
                _secondToFirst[second] = firsts;
            }

            seconds.Add(second);
            firsts.Add(first);
        }

        public IList<TSecond> GetByFirst(TFirst first)
        {
            IList<TSecond> list;
            if (!_firstToSecond.TryGetValue(first, out list))
            {
                return EmptySecondList;
            }

            return new List<TSecond>(list); // Create a copy for sanity
        }

        public IList<TFirst> GetBySecond(TSecond second)
        {
            IList<TFirst> list;
            if (!_secondToFirst.TryGetValue(second, out list))
            {
                return EmptyFirstList;
            }

            return new List<TFirst>(list); // Create a copy for sanity
        }

        public bool ContainsFirst(TFirst first)
        {
            return _firstToSecond.ContainsKey(first);
        }

        public bool ContainsSecond(TSecond second)
        {
            return _secondToFirst.ContainsKey(second);
        }

        public bool TryGetByFirst(TFirst first, out IList<TSecond> seconds)
        {
            return _firstToSecond.TryGetValue(first, out seconds);
        }

        public bool TryGetBySecond(TSecond second, out IList<TFirst> firsts)
        {
            return _secondToFirst.TryGetValue(second, out firsts);
        }

        public bool RemoveByFirst(TFirst first)
        {
            foreach (var second in _firstToSecond[first])
            {
                _secondToFirst[second].Remove(first);
            }

            return _firstToSecond.Remove(first);
        }

        public bool RemoveBySecond(TSecond second)
        {
            foreach (var first in _secondToFirst[second])
            {
                _firstToSecond[first].Remove(second);
            }

            return _secondToFirst.Remove(second);
        }

        public void Clear()
        {
            _firstToSecond.Clear();
            _secondToFirst.Clear();
        }

        #region IEnumerable implementation

        public IEnumerator<KeyValuePair<TFirst, IList<TSecond>>> GetEnumerator()
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
