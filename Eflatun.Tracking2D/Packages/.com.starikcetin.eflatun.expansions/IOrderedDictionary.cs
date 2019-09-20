using System.Collections.Generic;

namespace starikcetin.Eflatun.Expansions
{
    /// <summary>
    /// A dictionary that remembers the order that keys were first inserted. <para/>
    /// If a new entry overwrites an existing entry, the original insertion position is left unchanged. <para/>
    /// Deleting an entry and reinserting it will move it to the end.
    /// </summary>
    /// <typeparam name="TKey">The type of keys. Cannot be <see cref="int"/></typeparam>
    /// <typeparam name="TValue">The type of values.</typeparam>
    public interface IOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        //
        // I took this code somewhere on the internet and edited it, but I can't remember the original source.
        // That being said, here are two of the possible candidates:
        // https://gist.github.com/hickford/5137384
        // https://stackoverflow.com/a/9844528/6301627
        //

        /// <summary>
        /// The value of the element at the given index.
        /// </summary>
        TValue this[int index] { get; set; }

        /// <summary>
        /// Find the position of an element by key. Returns -1 if the dictionary does not contain an element with the given key.
        /// </summary>
        int IndexOf(TKey key);

        /// <summary>
        /// Insert an element at the given index.
        /// </summary>
        void Insert(int index, TKey key, TValue value);

        /// <summary>
        /// Remove the element at the given index.
        /// </summary>
        void RemoveAt(int index);
    }
}
