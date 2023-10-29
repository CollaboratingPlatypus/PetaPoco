using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace PetaPoco.Core
{
    /// <summary>
    /// A basic, case insensitive, implementation of ExpandObject.
    /// Should be drop-in replacement for ExpandObject with case insensitive member matching by default.
    /// </summary>
    public sealed class ExpandoPoco : DynamicObject, IDictionary<string, object>
    {
        private readonly IDictionary<string, object> _dictionary;

        /// <summary>
        /// Initialize a new <see cref="ExpandoPoco"/> that does not have members.
        /// </summary>
        /// <param name="ignoreCase">Whether the instance must be case insensitive or not. Default is <see langword="true"/></param>
        public ExpandoPoco(bool ignoreCase = true)
        {
            var comparer = ignoreCase ?
                (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase :
                EqualityComparer<string>.Default;
            _dictionary = new Dictionary<string, object>(comparer);
        }

        #region IDictionary<string, object> Implementation

        /// <inheritdoc/>
        public object this[string key] { get => _dictionary[key]; set => _dictionary[key] = value; }
        /// <inheritdoc/>
        public ICollection<string> Keys => _dictionary.Keys;
        /// <inheritdoc/>
        public ICollection<object> Values => _dictionary.Values;
        /// <inheritdoc/>
        public int Count => _dictionary.Count;
        /// <inheritdoc/>
        public bool IsReadOnly => _dictionary.IsReadOnly;
        /// <inheritdoc/>
        public void Add(string key, object value) => _dictionary.Add(key, value);
        /// <inheritdoc/>
        public void Add(KeyValuePair<string, object> item) => _dictionary.Add(item);
        /// <inheritdoc/>
        public void Clear() => _dictionary.Clear();
        /// <inheritdoc/>
        public bool Contains(KeyValuePair<string, object> item) => _dictionary.Contains(item);
        /// <inheritdoc/>
        public bool ContainsKey(string key) => _dictionary.ContainsKey(key);
        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => _dictionary.CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _dictionary.GetEnumerator();
        /// <inheritdoc/>
        public bool Remove(string key) => _dictionary.Remove(key);
        /// <inheritdoc/>
        public bool Remove(KeyValuePair<string, object> item) => _dictionary.Remove(item);
        /// <inheritdoc/>
        public bool TryGetValue(string key, out object value) => _dictionary.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();
        #endregion

        /// <inheritdoc/>
        public override bool TryGetMember(GetMemberBinder binder, out object result) =>
            _dictionary.TryGetValue(binder.Name, out result);

        /// <inheritdoc/>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _dictionary[binder.Name] = value;
            return true;
        }

        /// <summary>
        /// Make <see cref="ExpandoPoco"/> castable to <see cref="ExpandoObject"/>
        /// </summary>
        /// <param name="obj"></param>
        public static implicit operator ExpandoObject(ExpandoPoco obj)
        {
            var expando = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expando;

            foreach (var entry in obj._dictionary)
            {
                expandoDict[entry.Key] = entry.Value;
            }

            return expando;
        }

        /// <summary>
        /// Make <see cref="ExpandoObject"/> castable to <see cref="ExpandoPoco"/>, but case insensitive
        /// </summary>
        /// <param name="obj"></param>
        public static implicit operator ExpandoPoco(ExpandoObject obj)
        {
            var expando = new ExpandoPoco(true);
            var expandoDict = (IDictionary<string, object>)expando;

            foreach (var entry in (IDictionary<string, object>)obj)
            {
                expandoDict[entry.Key] = entry.Value;
            }

            return expando;
        }
    }
}
