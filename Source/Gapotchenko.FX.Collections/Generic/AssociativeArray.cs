﻿using Gapotchenko.FX.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

#if NETCOREAPP3_0 || NETFRAMEWORK || NETSTANDARD2_0
#pragma warning disable CS8714
#endif

namespace Gapotchenko.FX.Collections.Generic
{
    /// <summary>
    /// Represents a collection of keys and values with <see langword="null"/> key support.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the associative array.</typeparam>
    /// <typeparam name="TValue">The type of the values in the associative array.</typeparam>
    [DebuggerTypeProxy(typeof(AssociativeArrayDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    public partial class AssociativeArray<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
    {
#pragma warning disable CS8714
        Dictionary<TKey, TValue> m_Dictionary;
#if !NETCOREAPP3_0 && !NETFRAMEWORK && !NETSTANDARD2_0
#pragma warning restore CS8714
#endif

        TValue m_NullValue;
        bool m_HasNullValue;

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
            m_Dictionary = new(capacity, comparer!);
            m_NullValue = default!;
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

            m_Dictionary = new(associativeArray.m_Dictionary, comparer!);
            m_NullValue = associativeArray.m_NullValue;
            m_HasNullValue = associativeArray.m_HasNullValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociativeArray{TKey, TValue}"/> class that contains 
        /// elements copied from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the default 
        /// equality comparer for the key type.
        /// </summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the new 
        /// <see cref="AssociativeArray{TKey, TValue}"/>.
        /// </param>
        public AssociativeArray(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociativeArray{TKey, TValue}"/> class that contains 
        /// elements copied from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the specified 
        /// <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the new 
        /// <see cref="AssociativeArray{TKey, TValue}"/>.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default 
        /// <see cref="IEqualityComparer{T}"/> for the type of the key.
        /// </param>
        public AssociativeArray(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            m_Dictionary = new(dictionary, comparer!);
            m_NullValue = default!;
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
                    if (m_HasNullValue)
                        return m_NullValue;
                    ThrowHelper.ThrowKeyNotFoundException(key);
                    return default!;
                }

                return m_Dictionary[key];
            }
            set
            {
                if (key is null)
                {
                    m_HasNullValue = true;
                    m_NullValue = value;
                }
                else
                {
                    m_Dictionary[key] = value;
                }
            }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get => this[key];
            set => this[key] = value;
        }

        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] =>
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
            new KeyValueCollection<TKey>(this, KeyValueCollection<TKey>.CollectionKind.Keys, m_Dictionary.Keys, () => default!);

        /// <summary>
        /// Gets a collection containing the values in the <see cref="AssociativeArray{TKey, TValue}"/>.
        /// </summary>
        public ICollection<TValue> Values =>
            new KeyValueCollection<TValue>(this, KeyValueCollection<TValue>.CollectionKind.Values, m_Dictionary.Values, () => m_NullValue);

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="AssociativeArray{TKey, TValue}"/>.
        /// </summary>
        public int Count =>
            m_HasNullValue ? m_Dictionary.Count + 1 : m_Dictionary.Count;

        ICollection IDictionary.Keys => (ICollection)Keys;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        ICollection IDictionary.Values => (ICollection)Values;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        bool IDictionary.IsReadOnly => false;
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;
        bool IDictionary.IsFixedSize => false;

        int ICollection.Count => Count;
        int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count => Count;

        object ICollection.SyncRoot => m_Dictionary;

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
                if (!m_HasNullValue)
                {
                    m_HasNullValue = true;
                    m_NullValue = value;
                }
                else
                {
                    ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);
                }
            }
            else
            {
                m_Dictionary.Add(key, value);
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

                if (!m_HasNullValue)
                {
                    m_HasNullValue = true;
                    m_NullValue = (TValue)value!;
                }
                else
                {
                    ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(default);
                }
            }
            else
            {
                ((IDictionary)m_Dictionary).Add(key, value);
            }
        }

        /// <summary>
        /// Attempts to add the specified key and value to the associative array.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <returns><see langword="true"/> if the key/value pair was added to the associative array successfully; otherwise, <see langword="false"/>.</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            if (key is null)
            {
                if (m_HasNullValue)
                {
                    return false;
                }
                else
                {
                    m_HasNullValue = true;
                    m_NullValue = value;
                    return true;
                }
            }
            else
            {
                return m_Dictionary.TryAdd(key, value);
            }
        }

        /// <summary>
        /// Removes all keys and values from the <see cref="AssociativeArray{TKey, TValue}"/>.
        /// </summary>
        public void Clear()
        {
            m_Dictionary.Clear();
            m_HasNullValue = false;
            m_NullValue = default!;
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
        /// <see langword="true"/> if the <see cref="AssociativeArray{TKey, TValue}"/> contains an element with
        /// the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            if (key is null)
                return m_HasNullValue;
            return m_Dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the <see cref="AssociativeArray{TKey, TValue}"/> contains the specified value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="AssociativeArray{TKey, TValue}"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="AssociativeArray{TKey, TValue}"/> contains an element with
        /// the specified value; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsValue(TValue value)
        {
            if (m_HasNullValue)
            {
                if (value is null && m_NullValue is null ||
                    EqualityComparer<TValue>.Default.Equals(value, m_NullValue))
                {
                    return true;
                }
            }

            return m_Dictionary.ContainsValue(value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).CopyTo(array, arrayIndex + (m_HasNullValue ? 1 : 0));

            if (m_HasNullValue)
            {
                array[arrayIndex] = new KeyValuePair<TKey, TValue>(default!, m_NullValue);
            }
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="AssociativeArray{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><see langword="true"/> if the element is successfully found and removed; otherwise, <see langword="false"/>.</returns>
        public bool Remove(TKey key)
        {
            if (key is null)
            {
                if (m_HasNullValue)
                {
                    m_HasNullValue = false;
                    m_NullValue = default!;
                    return true;
                }

                return false;
            }

            return m_Dictionary.Remove(key);
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="AssociativeArray{TKey, TValue}"/>,
        /// and copies the element to the <paramref name="value"/> parameter.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The removed element.</param>
        /// <returns><see langword="true"/> if the element is successfully found and removed; otherwise, <see langword="false"/>.</returns>
        public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (key is null)
            {
                if (m_HasNullValue)
                {
                    value = m_NullValue;
                    m_HasNullValue = false;
                    m_NullValue = default!;
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }

            return m_Dictionary.Remove(key, out value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            if (keyValuePair.Key is null)
            {
                if (m_HasNullValue && EqualityComparer<TValue>.Default.Equals(m_NullValue, keyValuePair.Value))
                {
                    m_HasNullValue = false;
                    m_NullValue = default!;
                    return true;
                }

                return false;
            }

            return ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).Remove(keyValuePair);
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
                if (m_HasNullValue)
                {
                    value = m_NullValue;
                    return true;
                }
                else
                {
                    value = default!;
                    return false;
                }
            }

            return m_Dictionary.TryGetValue(key, out value!);
        }

        bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(
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

        bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key) =>
            ContainsKey(key);

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            ((ICollection)m_Dictionary).CopyTo(array, arrayIndex + (m_HasNullValue ? 1 : 0));

            if (m_HasNullValue)
            {
                if (array is KeyValuePair<TKey, TValue>[] pairs)
                {
                    pairs[arrayIndex] = new KeyValuePair<TKey, TValue>(default!, m_NullValue!);
                }
                else if (array is DictionaryEntry[] dictEntryArray)
                {
                    dictEntryArray[arrayIndex] = new DictionaryEntry(default!, m_NullValue!);
                }
                else if (array is object[] objArray)
                {
                    objArray[arrayIndex] = new KeyValuePair<TKey, TValue>(default!, m_NullValue!);
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
            if (m_HasNullValue)
                return m_Dictionary
                    .Prepend(new KeyValuePair<TKey, TValue>(default!, m_NullValue!))
                    .GetEnumerator();
            else
                return m_Dictionary.GetEnumerator();
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
            m_Dictionary.EnsureCapacity(capacity);

        /// <summary>
        /// Sets the capacity of this associative array to what it would be if it had been originally
        /// initialized with all its entries.
        /// </summary>
        public void TrimExcess() =>
            m_Dictionary.TrimExcess();

        /// <summary>
        /// Sets the capacity of this associative array to hold up a specified number of entries
        /// without any further expansion of its backing storage.
        /// </summary>
        /// <param name="capacity">The new capacity.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than <see cref="AssociativeArray{TKey, TValue}"/>.</exception>
        public void TrimExcess(int capacity) =>
            m_Dictionary.TrimExcess(capacity);

#endif

        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}"/> that is used to determine
        /// equality of keys for the associative array.
        /// </summary>
        public IEqualityComparer<TKey> Comparer =>
            m_Dictionary.Comparer;

        struct DictionaryEnumerator : IDictionaryEnumerator
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly IEnumerator<KeyValuePair<TKey, TValue>> m_Enumerator;

            public DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
            {
                m_Enumerator = enumerator;
            }

            public DictionaryEntry Entry
            {
                get
                {
                    var entry = m_Enumerator.Current;
                    return new DictionaryEntry(entry.Key!, entry.Value);
                }
            }

            public object Key => m_Enumerator.Current.Key!;
            public object? Value => m_Enumerator.Current.Value;
            public object Current => Entry;
            public bool MoveNext() => m_Enumerator.MoveNext();
            public void Reset() => m_Enumerator.Reset();
        }

        [DebuggerTypeProxy(typeof(AssociativeArrayKeyValueCollectionDebugView<,,>))]
        [DebuggerDisplay("Count = {Count}")]
        sealed class KeyValueCollection<T> : ICollection<T>, ICollection
        {
            public enum CollectionKind { Keys, Values };

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public CollectionKind Kind { get; }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly AssociativeArray<TKey, TValue> m_Parent;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly ICollection<T> m_Collection;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly Func<T> m_NullValueGetter;

            public KeyValueCollection(
                AssociativeArray<TKey, TValue> parent,
                CollectionKind kind,
                ICollection<T> collection,
                Func<T> nullValueGetter)
            {
                Kind = kind;
                m_Parent = parent;
                m_Collection = collection;
                m_NullValueGetter = nullValueGetter;
            }

            public int Count => m_Parent.Count;

            public bool IsReadOnly => true;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => m_Parent.m_Dictionary;

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
                            return m_Parent.m_HasNullValue;
                        break;

                    case CollectionKind.Values:
                        if (m_Parent.m_HasNullValue &&
                            (item is null && m_Parent.m_NullValue is null ||
                                EqualityComparer<T>.Default.Equals(item, m_NullValueGetter())))
                        {
                            return true;
                        }
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                return m_Collection.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                m_Collection.CopyTo(array, arrayIndex + (m_Parent.m_HasNullValue ? 1 : 0));

                if (m_Parent.m_HasNullValue)
                {
                    array[arrayIndex] = m_NullValueGetter()!;
                }
            }

            public void CopyTo(Array array, int arrayIndex)
            {
                ((ICollection)m_Collection).CopyTo(array, arrayIndex + (m_Parent.m_HasNullValue ? 1 : 0));

                if (m_Parent.m_HasNullValue)
                {
                    if (array is T[] tArray)
                    {
                        tArray[arrayIndex] = m_NullValueGetter()!;
                    }
                    else if (array is object[] objArray)
                    {
                        objArray[arrayIndex] = m_NullValueGetter()!;
                    }
                    else
                    {
                        ThrowHelper.ThrowArgumentException_Argument_InvalidArrayType();
                    }
                }
            }

            public IEnumerator<T> GetEnumerator()
            {
                if (m_Parent.m_HasNullValue)
                    return m_Collection.Prepend(m_NullValueGetter()!).GetEnumerator();
                else
                    return m_Collection.GetEnumerator();
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

            [DoesNotReturn]
            public static void ThrowWrongValueTypeArgumentException<T>(T value, Type targetType)
            {
                throw new ArgumentException(string.Format(WrongArgumentTypeMessage, value, targetType), nameof(value));
            }

            [DoesNotReturn]
            public static void ThrowWrongKeyTypeArgumentException<T>(T key, Type targetType)
            {
                throw new ArgumentException(string.Format(WrongArgumentTypeMessage, key, targetType), nameof(key));
            }

            [DoesNotReturn]
            public static void ThrowKeyNotFoundException(TKey? key)
            {
                throw new KeyNotFoundException($"The given key '{key}' was not present in the associative array.");
            }

            [DoesNotReturn]
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

            [DoesNotReturn]
            public static void ThrowArgumentException_Argument_InvalidArrayType()
            {
                throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
            }

            [DoesNotReturn]
            public static void ThrowAddingDuplicateWithKeyArgumentException(TKey? key)
            {
                throw new ArgumentException($"An item with the same key has already been added. Key: '{key}'.");
            }
        }
    }
}
