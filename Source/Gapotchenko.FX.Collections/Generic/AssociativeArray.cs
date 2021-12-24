using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Collections.Generic
{
    /// <summary>
    /// Represents a collection of keys and values with <c>null</c> key support.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the associative array.</typeparam>
    /// <typeparam name="TValue">The type of the values in the associative array.</typeparam>
    [DebuggerTypeProxy(typeof(AssociativeArrayDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
#pragma warning disable CS8714
    public partial class AssociativeArray<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
#pragma warning restore CS8714
    {
#pragma warning disable CS8714
        Dictionary<TKey, TValue> _dictionary;
#pragma warning restore CS8714
        TValue _nullValue;
        bool _hasNullValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociativeArray{TKey, TValue}"/> class that is empty, 
        /// has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        public AssociativeArray()
            : this(0, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociativeArray{TKey, TValue}"/> class that is empty, 
        /// has the specified initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="AssociativeArray{TKey, TValue}"/> can contain.</param>
        public AssociativeArray(int capacity)
            : this(capacity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociativeArray{TKey, TValue}"/> class that is empty, 
        /// has the default initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default 
        /// <see cref="IEqualityComparer{T}"/> for the type of the key.
        /// </param>
        public AssociativeArray(IEqualityComparer<TKey>? comparer)
            : this(0, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociativeArray{TKey, TValue}"/> class that contains 
        /// elements copied from the specified <see cref="AssociativeArray{TKey, TValue}"/> and uses the default 
        /// equality comparer for the key type.
        /// </summary>
        /// <param name="associativeArray">
        /// The <see cref="AssociativeArray{TKey, TValue}"/> whose elements are copied to the new 
        /// <see cref="AssociativeArray{TKey, TValue}"/>.
        /// </param>
        public AssociativeArray(AssociativeArray<TKey, TValue> associativeArray)
            : this(associativeArray, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociativeArray{TKey, TValue}"/> class that is empty, 
        /// has the specified initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="AssociativeArray{TKey, TValue}"/> can contain.</param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default 
        /// <see cref="IEqualityComparer{T}"/> for the type of the key.
        /// </param>
        public AssociativeArray(int capacity, IEqualityComparer<TKey>? comparer)
        {
            _dictionary = new(capacity, comparer!);
            _nullValue = default!;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociativeArray{TKey, TValue}"/> class that contains 
        /// elements copied from the specified <see cref="AssociativeArray{TKey, TValue}"/> and uses the specified 
        /// <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="associativeArray">
        /// The <see cref="AssociativeArray{TKey, TValue}"/> whose elements are copied to the new 
        /// <see cref="AssociativeArray{TKey, TValue}"/>.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default 
        /// <see cref="IEqualityComparer{T}"/> for the type of the key.
        /// </param>
        public AssociativeArray(AssociativeArray<TKey, TValue> associativeArray, IEqualityComparer<TKey>? comparer)
        {
            if (associativeArray == null)
                throw new ArgumentNullException(nameof(associativeArray));

            _dictionary = new(associativeArray._dictionary, comparer!);
            _nullValue = associativeArray._nullValue;
            _hasNullValue = associativeArray._hasNullValue;
        }


        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found,
        /// a get operation throws a <see cref="KeyNotFoundException"/>, and
        /// a set operation creates a new element with the specified key.
        /// </returns>
        /// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
        public TValue this[TKey key]
        {
            get
            {
                if (key is null)
                {
                    if (_hasNullValue)
                        return _nullValue;
                    ThrowHelper.ThrowKeyNotFoundException(key);
                    return default!;
                }

                return _dictionary[key];
            }
            set
            {
                if (key is null)
                {
                    _hasNullValue = true;
                    _nullValue = value;
                }
                else
                {
                    _dictionary[key] = value;
                }
            }
        }

#pragma warning disable CS8714
        TValue IDictionary<TKey, TValue>.this[TKey key]
#pragma warning restore CS8714
        {
            get => this[key];
            set => this[key] = value;
        }

#pragma warning disable CS8714
        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] =>
#pragma warning restore CS8714
            this[key]!;

        object? IDictionary.this[object key]
        {
            get
            {
                if (key is TKey tKey)
                {
                    return this[tKey];
                }

                return null;
            }
            set
            {
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, nameof(value));

                try
                {
                    TKey tempKey = (TKey)key;
                    try
                    {
                        this[tempKey] = (TValue)value!;
                    }
                    catch (InvalidCastException)
                    {
                        ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                    }
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
                }
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="AssociativeArray{TKey, TValue}"/>.
        /// </summary>
        public ICollection<TKey> Keys =>
            new KeyValueCollection<TKey>(this, KeyValueCollection<TKey>.CollectionKind.Keys, _dictionary.Keys, () => default!);

        /// <summary>
        /// Gets a collection containing the values in the <see cref="AssociativeArray{TKey, TValue}"/>.
        /// </summary>
        public ICollection<TValue> Values =>
            new KeyValueCollection<TValue>(this, KeyValueCollection<TValue>.CollectionKind.Values, _dictionary.Values, () => _nullValue);

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="AssociativeArray{TKey, TValue}"/>.
        /// </summary>
        public int Count =>
            _hasNullValue ? _dictionary.Count + 1 : _dictionary.Count;

        ICollection IDictionary.Keys => (ICollection)Keys;
#pragma warning disable CS8714
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
#pragma warning restore CS8714

        ICollection IDictionary.Values => (ICollection)Values;
#pragma warning disable CS8714
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
#pragma warning restore CS8714

        bool IDictionary.IsReadOnly => false;
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;
        bool IDictionary.IsFixedSize => false;

        int ICollection.Count => Count;
        int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count => Count;

        object ICollection.SyncRoot => _dictionary;

        bool ICollection.IsSynchronized => false;

        /// <summary>
        /// Adds the specified key and value to the associative array.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="AssociativeArray{TKey, TValue}"/>.</exception>
        public void Add(TKey key, TValue value)
        {
            if (key is null)
            {
                if (!_hasNullValue)
                {
                    _hasNullValue = true;
                    _nullValue = value;
                }
                else
                {
                    ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);
                }
            }
            else
            {
                _dictionary.Add(key, value);
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) =>
            Add(item.Key, item.Value);

        void IDictionary.Add(object key, object? value)
        {
            if (key is null)
            {
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, nameof(value));

                if (value is not null && value is not TValue)
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));

                if (!_hasNullValue)
                {
                    _hasNullValue = true;
                    _nullValue = (TValue)value!;
                }
                else
                {
                    ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(default);
                }
            }
            else
            {
                ((IDictionary)_dictionary).Add(key, value);
            }
        }

        /// <summary>
        /// Attempts to add the specified key and value to the associative array.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns><c>true</c> if the key/value pair was added to the associative array successfully; otherwise, <c>false</c>.</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            if (key is null)
            {
                if (_hasNullValue)
                {
                    return false;
                }
                else
                {
                    _hasNullValue = true;
                    _nullValue = value;
                    return true;
                }
            }
            else
            {

#pragma warning disable CS8714
                return _dictionary.TryAdd(key, value);
#pragma warning restore CS8714
            }
        }

        /// <summary>
        /// Removes all keys and values from the <see cref="AssociativeArray{TKey, TValue}"/>.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
            _hasNullValue = false;
            _nullValue = default!;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            if (TryGetValue(item.Key, out var value) &&
                EqualityComparer<TValue>.Default.Equals(value, item.Value))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the <see cref="AssociativeArray{TKey, TValue}"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="AssociativeArray{TKey, TValue}"/>.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="AssociativeArray{TKey, TValue}"/> contains an element with
        /// the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            if (key is null)
                return _hasNullValue;
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the <see cref="AssociativeArray{TKey, TValue}"/> contains the specified value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="AssociativeArray{TKey, TValue}"/>.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="AssociativeArray{TKey, TValue}"/> contains an element with
        /// the specified value; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsValue(TValue value)
        {
            if (_hasNullValue)
            {
                if (value is null && _nullValue is null ||
                    EqualityComparer<TValue>.Default.Equals(value, _nullValue))
                {
                    return true;
                }
            }

            return _dictionary.ContainsValue(value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex + (_hasNullValue ? 1 : 0));

            if (_hasNullValue)
            {
                array[arrayIndex] = new KeyValuePair<TKey, TValue>(default!, _nullValue);
            }
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="AssociativeArray{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.</returns>
        public bool Remove(TKey key)
        {
            if (key is null)
            {
                if (_hasNullValue)
                {
                    _hasNullValue = false;
                    _nullValue = default!;
                    return true;
                }

                return false;
            }

            return _dictionary.Remove(key);
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="AssociativeArray{TKey, TValue}"/>,
        /// and copies the element to the <paramref name="value"/> parameter.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The removed element.</param>
        /// <returns><c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.</returns>
        public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (key is null)
            {
                if (_hasNullValue)
                {
                    value = _nullValue;
                    _hasNullValue = false;
                    _nullValue = default!;
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }

#pragma warning disable CS8714
            return _dictionary.Remove(key, out value);
#pragma warning restore CS8714
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            if (keyValuePair.Key is null)
            {
                if (_hasNullValue && EqualityComparer<TValue>.Default.Equals(_nullValue, keyValuePair.Value))
                {
                    _hasNullValue = false;
                    _nullValue = default!;
                    return true;
                }

                return false;
            }

            return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(keyValuePair);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns></returns>
        public bool TryGetValue(
            TKey key,
#if NETCOREAPP3_1_OR_GREATER
            [MaybeNullWhen(false)]
#endif
            out TValue value)
        {
            if (key is null)
            {
                if (_hasNullValue)
                {
                    value = _nullValue;
                    return true;
                }
                else
                {
                    value = default!;
                    return false;
                }
            }

            return _dictionary.TryGetValue(key, out value!);
        }

#pragma warning disable CS8714
        bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(
#pragma warning restore CS8714
            TKey key,
#if NETCOREAPP3_1_OR_GREATER
            [MaybeNullWhen(false)]
#endif
            out TValue value)
        {
            return TryGetValue(key, out value);
        }

        void IDictionary.Clear() =>
            Clear();

        bool IDictionary.Contains(object key)
        {
            if (key is TKey tKey)
                return ContainsKey(tKey);
            return false;
        }

#pragma warning disable CS8714
        bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key) =>
#pragma warning restore CS8714
            ContainsKey(key);

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            ((ICollection)_dictionary).CopyTo(array, arrayIndex + (_hasNullValue ? 1 : 0));

            if (_hasNullValue)
            {
                if (array is KeyValuePair<TKey, TValue>[] pairs)
                {
                    pairs[arrayIndex] = new KeyValuePair<TKey, TValue>(default!, _nullValue!);
                }
                else if (array is DictionaryEntry[] dictEntryArray)
                {
                    dictEntryArray[arrayIndex] = new DictionaryEntry(default!, _nullValue!);
                }
                else if (array is object[] objArray)
                {
                    objArray[arrayIndex] = new KeyValuePair<TKey, TValue>(default!, _nullValue!);
                }
                else
                {
                    ThrowHelper.ThrowArgumentException_Argument_InvalidArrayType();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        IDictionaryEnumerator IDictionary.GetEnumerator() =>
            new DictionaryEnumerator(GetEnumerator());

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="AssociativeArray{TKey, TValue}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> for the <see cref="AssociativeArray{TKey, TValue}"/>.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (_hasNullValue)
                yield return new KeyValuePair<TKey, TValue>(default!, _nullValue!);
            foreach (var item in _dictionary)
                yield return item!;
        }


        void IDictionary.Remove(object key)
        {
            if (key is TKey tKey)
                Remove(tKey);
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER

        /// <summary>
        /// Ensures that the associative array can hold up to a specified number of entries without
        /// any further expansion of its backing storage.
        /// </summary>
        /// <param name="capacity">The number of entries.</param>
        /// <returns>The current capacity of the <see cref="AssociativeArray{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        public int EnsureCapacity(int capacity) =>
            _dictionary.EnsureCapacity(capacity);

        /// <summary>
        /// Sets the capacity of this associative array to what it would be if it had been originally
        /// initialized with all its entries.
        /// </summary>
        public void TrimExcess() =>
            _dictionary.TrimExcess();

        /// <summary>
        /// Sets the capacity of this associative array to hold up a specified number of entries
        /// without any further expansion of its backing storage.
        /// </summary>
        /// <param name="capacity">The new capacity.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than <see cref="AssociativeArray{TKey, TValue}"/>.</exception>
        public void TrimExcess(int capacity) =>
            _dictionary.TrimExcess(capacity);

#endif

        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}"/> that is used to determine
        /// equality of keys for the associative array.
        /// </summary>
        public IEqualityComparer<TKey> Comparer =>
            _dictionary.Comparer;

        struct DictionaryEnumerator : IDictionaryEnumerator
        {
            readonly IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;

            public DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
            {
                _enumerator = enumerator;
            }

            public DictionaryEntry Entry
            {
                get
                {
                    var entry = _enumerator.Current;
                    return new DictionaryEntry(entry.Key!, entry.Value);
                }
            }

            public object Key => _enumerator.Current.Key!;
            public object? Value => _enumerator.Current.Value;
            public object Current => Entry;
            public bool MoveNext() => _enumerator.MoveNext();
            public void Reset() => _enumerator.Reset();
        }

        [DebuggerTypeProxy(typeof(AssociativeArrayKeyValueCollectionDebugView<>))]
        [DebuggerDisplay("Count = {Count}")]
        sealed class KeyValueCollection<T> : ICollection<T>, ICollection
        {
            public enum CollectionKind { Keys, Values };

            public CollectionKind Kind { get; }

            readonly AssociativeArray<TKey, TValue> _parent;
            readonly ICollection<T> _collection;
            readonly Func<T> _nullValueGetter;

            public KeyValueCollection(
                AssociativeArray<TKey, TValue> parent,
                CollectionKind kind,
                ICollection<T> collection,
                Func<T> nullValueGetter)
            {
                Kind = kind;
                _parent = parent;
                _collection = collection;
                _nullValueGetter = nullValueGetter;
            }

            public int Count => _parent.Count;

            public bool IsReadOnly => true;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => _parent._dictionary;

            public void Add(T item) =>
                ThrowHelper.ThrowCollectionMutatingNotAllowed(Kind);

            public void Clear() =>
                ThrowHelper.ThrowCollectionMutatingNotAllowed(Kind);

            public bool Contains(T item)
            {
                switch (Kind)
                {
                    case CollectionKind.Keys:
                        if (item is null)
                            return _parent._hasNullValue;
                        break;

                    case CollectionKind.Values:
                        if (_parent._hasNullValue &&
                            (item is null && _parent._nullValue is null ||
                                EqualityComparer<T>.Default.Equals(item, _nullValueGetter())))
                        {
                            return true;
                        }
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                return _collection.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                _collection.CopyTo(array, arrayIndex + (_parent._hasNullValue ? 1 : 0));

                if (_parent._hasNullValue)
                {
                    array[arrayIndex] = _nullValueGetter()!;
                }
            }

            public void CopyTo(Array array, int arrayIndex)
            {
                ((ICollection)_collection).CopyTo(array, arrayIndex + (_parent._hasNullValue ? 1 : 0));

                if (_parent._hasNullValue)
                {
                    if (array is T[] tArray)
                    {
                        tArray[arrayIndex] = _nullValueGetter()!;
                    }
                    else if (array is object[] objArray)
                    {
                        objArray[arrayIndex] = _nullValueGetter()!;
                    }
                    else
                    {
                        ThrowHelper.ThrowArgumentException_Argument_InvalidArrayType();
                    }
                }
            }

            public IEnumerator<T> GetEnumerator()
            {
                if (_parent._hasNullValue)
                    yield return _nullValueGetter()!;
                foreach (var item in _collection)
                    yield return item!;
            }

            public bool Remove(T item)
            {
                ThrowHelper.ThrowCollectionMutatingNotAllowed(Kind);
                return default;
            }

            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();
        }

        static class ThrowHelper
        {
            // Allow nulls for reference types and Nullable<U>, but not for value types.
            // Aggressively inline so the JIT evaluates the if in place and either drops
            // the call altogether or leaves the body as is.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void IfNullAndNullsAreIllegalThenThrow<T>(object? value, string argName)
            {
                // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
                if (!(default(T) == null) && value == null)
                    throw new ArgumentNullException(argName);
            }

            const string WrongArgumentTypeMessage = "The value '{0}' is not of type '{1}' and cannot be used in this generic collection.";

            public static void ThrowWrongValueTypeArgumentException<T>(T value, Type targetType)
            {
                throw new ArgumentException(string.Format(WrongArgumentTypeMessage, value, targetType), nameof(value));
            }

            public static void ThrowWrongKeyTypeArgumentException<T>(T key, Type targetType)
            {
                throw new ArgumentException(string.Format(WrongArgumentTypeMessage, key, targetType), nameof(key));
            }

            public static void ThrowKeyNotFoundException(TKey? key)
            {
                throw new KeyNotFoundException($"The given key '{key}' was not present in the associative array.");
            }

            public static void ThrowCollectionMutatingNotAllowed<T>(AssociativeArray<TKey, TValue>.KeyValueCollection<T>.CollectionKind kind)
            {
                var kindString = kind switch
                {
                    AssociativeArray<TKey, TValue>.KeyValueCollection<T>.CollectionKind.Keys => "key",
                    AssociativeArray<TKey, TValue>.KeyValueCollection<T>.CollectionKind.Values => "value",
                    _ => throw new ArgumentOutOfRangeException(nameof(kind))
                };

                throw new NotSupportedException($"Mutating a {kindString} collection derived from an associative array is not allowed.");
            }

            public static void ThrowArgumentException_Argument_InvalidArrayType()
            {
                throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
            }

            public static void ThrowAddingDuplicateWithKeyArgumentException(TKey? key)
            {
                throw new ArgumentException($"An item with the same key has already been added. Key: '{key}'.");
            }
        }
    }
}
