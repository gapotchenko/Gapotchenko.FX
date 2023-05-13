using Gapotchenko.FX.Linq;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Collections.Generic;

/// <summary>
/// <para>
/// Represents a collection of keys and values.
/// </para>
/// <para>
/// In contrast to <see cref="Dictionary{TKey, TValue}"/>, <see cref="AssociativeArray{TKey, TValue}"/> supports keys with <see langword="null"/> values.
/// </para>
/// </summary>
/// <typeparam name="TKey">The type of the keys in the associative array.</typeparam>
/// <typeparam name="TValue">The type of the values in the associative array.</typeparam>
[DebuggerTypeProxy(typeof(AssociativeArrayDebugView<,>))]
[DebuggerDisplay("Count = {Count}")]
public partial class AssociativeArray<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
{
#pragma warning disable CS8714
    Dictionary<TKey, TValue> m_Dictionary;
#pragma warning restore CS8714

    Optional<TValue> m_NullSlot;

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
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
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
    /// Initializes a new instance of the <see cref="AssociativeArray{TKey, TValue}"/> class that is empty, 
    /// has the specified initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the <see cref="AssociativeArray{TKey, TValue}"/> can contain.</param>
    /// <param name="comparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default 
    /// <see cref="IEqualityComparer{T}"/> for the type of the key.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
    public AssociativeArray(int capacity, IEqualityComparer<TKey>? comparer)
    {
        m_Dictionary = new(capacity, comparer!);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssociativeArray{TKey, TValue}"/> class that contains 
    /// elements copied from the specified <see cref="IReadOnlyDictionary{TKey, TValue}"/> and uses the default 
    /// equality comparer for the key type.
    /// </summary>
    /// <param name="dictionary">
    /// The <see cref="IReadOnlyDictionary{TKey, TValue}"/> whose elements are copied to the new 
    /// <see cref="AssociativeArray{TKey, TValue}"/>.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="dictionary"/> contains one or more duplicated keys.</exception>
    public AssociativeArray(IReadOnlyDictionary<TKey, TValue> dictionary)
        : this(dictionary, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssociativeArray{TKey, TValue}"/> class that contains 
    /// elements copied from the specified <see cref="IReadOnlyDictionary{TKey, TValue}"/> and uses the specified 
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <param name="dictionary">
    /// The <see cref="IReadOnlyDictionary{TKey, TValue}"/> whose elements are copied to the new 
    /// <see cref="AssociativeArray{TKey, TValue}"/>.
    /// </param>
    /// <param name="comparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default 
    /// <see cref="IEqualityComparer{T}"/> for the type of the key.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="dictionary"/> contains one or more duplicated keys.</exception>
    public AssociativeArray(IReadOnlyDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer)
    {
        if (dictionary == null)
            throw new ArgumentNullException(nameof(dictionary));

        AddRange(dictionary, comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssociativeArray{TKey, TValue}"/> class that contains 
    /// elements copied from the specified <see cref="IEnumerable{T}"/> and uses the default 
    /// equality comparer for the key type.
    /// </summary>
    /// <param name="collection">
    /// The <see cref="IEnumerable{T}"/> whose elements are copied to the new <see cref="AssociativeArray{TKey, TValue}"/>.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="collection"/> contains one or more duplicated keys.</exception>
    public AssociativeArray(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        : this(collection, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssociativeArray{TKey, TValue}"/> class that contains 
    /// elements copied from the specified <see cref="IEnumerable{T}"/> and uses the specified 
    /// <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <param name="collection">
    /// The <see cref="IEnumerable{T}"/> whose elements are copied to the new <see cref="AssociativeArray{TKey, TValue}"/>.
    /// </param>
    /// <param name="comparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default 
    /// <see cref="IEqualityComparer{T}"/> for the type of the key.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="collection"/> contains one or more duplicated keys.</exception>
    public AssociativeArray(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        AddRange(collection, comparer);
    }

    [MemberNotNull(nameof(m_Dictionary))]
    void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer)
    {
        switch (collection)
        {
            case AssociativeArray<TKey, TValue> associativeArray:
                m_Dictionary = new(associativeArray.m_Dictionary, comparer);
                m_NullSlot = associativeArray.m_NullSlot;
                break;

#pragma warning disable CS8714
            case Dictionary<TKey, TValue> dictionary:
                m_Dictionary = new(dictionary, comparer);
                break;
#pragma warning restore CS8714

            default:
                m_Dictionary = new(collection.TryGetNonEnumeratedCount() ?? 0, comparer);
                foreach (var pair in collection)
                    Add(pair.Key, pair.Value);
                break;
        }
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
                if (m_NullSlot.HasValue)
                {
                    return m_NullSlot.Value;
                }
                else
                {
                    ThrowHelper.ThrowKeyNotFoundException(key);
                    return default!;
                }
            }
            else
            {
                return m_Dictionary[key];
            }
        }
        set
        {
            if (key is null)
                m_NullSlot = value;
            else
                m_Dictionary[key] = value;
        }
    }

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get => this[key];
        set => this[key] = value;
    }

    TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] => this[key]!;

    object? IDictionary.this[object key]
    {
        get
        {
            if (key is TKey tKey)
                return this[tKey];
            else
                return null;
        }
        set
        {
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, nameof(value));

            try
            {
                var tempKey = (TKey)key;
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
    public ICollection<TKey> Keys => new KeyCollection(this, m_Dictionary.Keys);

    /// <summary>
    /// Gets a collection containing the values in the <see cref="AssociativeArray{TKey, TValue}"/>.
    /// </summary>
    public ICollection<TValue> Values => new ValueCollection(this, m_Dictionary.Values);

    /// <summary>
    /// Gets the number of key/value pairs contained in the <see cref="AssociativeArray{TKey, TValue}"/>.
    /// </summary>
    public int Count => m_NullSlot.HasValue ? m_Dictionary.Count + 1 : m_Dictionary.Count;

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
            if (!m_NullSlot.HasValue)
                m_NullSlot = value;
            else
                ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);
        }
        else
        {
            m_Dictionary.Add(key, value);
        }
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

    void IDictionary.Add(object key, object? value)
    {
        if (key is null)
        {
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, nameof(value));

            if (value is not null && value is not TValue)
                ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));

            if (!m_NullSlot.HasValue)
            {
                m_NullSlot = (TValue)value!;
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
            if (m_NullSlot.HasValue)
            {
                return false;
            }
            else
            {
                m_NullSlot = value;
                return true;
            }
        }
        else
        {
#pragma warning disable CS8714
            return m_Dictionary.TryAdd(key, value);
#pragma warning restore CS8714
        }
    }

    /// <summary>
    /// Removes all keys and values from the <see cref="AssociativeArray{TKey, TValue}"/>.
    /// </summary>
    public void Clear()
    {
        m_Dictionary.Clear();
        m_NullSlot = default;
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) =>
        TryGetValue(item.Key, out var value) &&
        EqualityComparer<TValue>.Default.Equals(value, item.Value);

    /// <summary>
    /// Determines whether the <see cref="AssociativeArray{TKey, TValue}"/> contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the <see cref="AssociativeArray{TKey, TValue}"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="AssociativeArray{TKey, TValue}"/> contains an element with
    /// the specified key; otherwise, <see langword="false"/>.
    /// </returns>
    public bool ContainsKey(TKey key) => key is null ? m_NullSlot.HasValue : m_Dictionary.ContainsKey(key);

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
        if (m_NullSlot.HasValue &&
            EqualityComparer<TValue>.Default.Equals(value, m_NullSlot.Value))
        {
            return true;
        }
        else
        {
            return m_Dictionary.ContainsValue(value);
        }
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if ((uint)arrayIndex > (uint)array.Length)
            ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count)
            ThrowHelper.ThrowArgumentException_ArrayPlusOffTooSmall();

        ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).CopyTo(array, arrayIndex + (m_NullSlot.HasValue ? 1 : 0));

        if (m_NullSlot.HasValue)
            array[arrayIndex] = new(default!, m_NullSlot.Value);
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
            if (m_NullSlot.HasValue)
            {
                m_NullSlot = default;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return m_Dictionary.Remove(key);
        }
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
            if (m_NullSlot.HasValue)
            {
                value = m_NullSlot.Value;
                m_NullSlot = default;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        else
        {
#pragma warning disable CS8714
            return m_Dictionary.Remove(key, out value);
#pragma warning restore CS8714
        }
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
    {
        if (keyValuePair.Key is null)
        {
            if (m_NullSlot.HasValue &&
                EqualityComparer<TValue>.Default.Equals(m_NullSlot.Value, keyValuePair.Value))
            {
                m_NullSlot = default;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).Remove(keyValuePair);
        }
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
            if (m_NullSlot.HasValue)
            {
                value = m_NullSlot.Value;
                return true;
            }
            else
            {
                value = default!;
                return false;
            }
        }
        else
        {
            return m_Dictionary.TryGetValue(key, out value!);
        }
    }

    bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(
        TKey key,
#if NETCOREAPP3_1_OR_GREATER
        [MaybeNullWhen(false)]
#endif
        out TValue value) =>
        TryGetValue(key, out value);

    void IDictionary.Clear() => Clear();

    bool IDictionary.Contains(object key) => key is TKey tKey && ContainsKey(tKey);

    bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key) => ContainsKey(key);

    void ICollection.CopyTo(Array array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if ((uint)arrayIndex > (uint)array.Length)
            ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count)
            ThrowHelper.ThrowArgumentException_ArrayPlusOffTooSmall();

        ((ICollection)m_Dictionary).CopyTo(array, arrayIndex + (m_NullSlot.HasValue ? 1 : 0));

        if (m_NullSlot.HasValue)
        {
            if (array is KeyValuePair<TKey, TValue>[] pairs)
                pairs[arrayIndex] = new(default!, m_NullSlot.Value);
            else if (array is DictionaryEntry[] dictEntryArray)
                dictEntryArray[arrayIndex] = new(default!, m_NullSlot.Value);
            else if (array is object[] objArray)
                objArray[arrayIndex] = new KeyValuePair<TKey, TValue>(default!, m_NullSlot.Value);
            else
                ThrowHelper.ThrowArgumentException_Argument_InvalidArrayType();
        }
    }

    void IDictionary.Remove(object key)
    {
        if (key is TKey tKey)
            Remove(tKey);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="AssociativeArray{TKey, TValue}"/>.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{T}"/> for the <see cref="AssociativeArray{TKey, TValue}"/>.</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
        new PrependEnumerator<KeyValuePair<TKey, TValue>>(
            () => m_NullSlot.HasValue ? new Optional<KeyValuePair<TKey, TValue>>(new(default!, m_NullSlot.Value)) : default,
            m_Dictionary);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IDictionaryEnumerator IDictionary.GetEnumerator() => new DictionaryEnumerator(GetEnumerator());

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
    public IEqualityComparer<TKey> Comparer => m_Dictionary.Comparer;

    sealed class DictionaryEnumerator : IDictionaryEnumerator
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
                return new(entry.Key!, entry.Value);
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
    abstract class KeyValueCollection<T> : ICollection<T>, ICollection
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected AssociativeArray<TKey, TValue> Parent { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly ICollection<T> m_Collection;

        public KeyValueCollection(AssociativeArray<TKey, TValue> parent, ICollection<T> collection)
        {
            Parent = parent;
            m_Collection = collection;
        }

        public int Count => Parent.Count;

        public bool IsReadOnly => true;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => Parent.m_Dictionary;

        public void Add(T item) => ThrowMutationNotAllowed();

        public bool Remove(T item)
        {
            ThrowMutationNotAllowed();
            return default;
        }

        public void Clear() => ThrowMutationNotAllowed();

        public virtual bool Contains(T item) => m_Collection.Contains(item);

        protected abstract Optional<T> NullSlot { get; }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if ((uint)arrayIndex > (uint)array.Length)
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count)
                ThrowHelper.ThrowArgumentException_ArrayPlusOffTooSmall();

            var nullSlot = NullSlot;

            m_Collection.CopyTo(array, arrayIndex + (nullSlot.HasValue ? 1 : 0));

            if (nullSlot.HasValue)
                array[arrayIndex] = nullSlot.Value;
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if ((uint)arrayIndex > (uint)array.Length)
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count)
                ThrowHelper.ThrowArgumentException_ArrayPlusOffTooSmall();

            var nullSlot = NullSlot;

            ((ICollection)m_Collection).CopyTo(array, arrayIndex + (nullSlot.HasValue ? 1 : 0));

            if (nullSlot.HasValue)
            {
                if (array is T[] tArray)
                    tArray[arrayIndex] = nullSlot.Value;
                else if (array is object[] objArray)
                    objArray[arrayIndex] = nullSlot.Value!;
                else
                    ThrowHelper.ThrowArgumentException_Argument_InvalidArrayType();
            }
        }

        public IEnumerator<T> GetEnumerator() => new PrependEnumerator<T>(() => NullSlot, m_Collection);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [DoesNotReturn]
        protected abstract void ThrowMutationNotAllowed();
    }

    sealed class KeyCollection : KeyValueCollection<TKey>
    {
        public KeyCollection(AssociativeArray<TKey, TValue> parent, ICollection<TKey> collection) :
            base(parent, collection)
        {
        }

        protected override Optional<TKey> NullSlot =>
            Parent.m_NullSlot.HasValue ?
                Optional.Some(default(TKey)!) :
                default;

        public override bool Contains(TKey item)
        {
            if (item is null)
                return Parent.m_NullSlot.HasValue;
            else
                return base.Contains(item);
        }

        [DoesNotReturn]
        protected override void ThrowMutationNotAllowed() =>
            throw new NotSupportedException($"Mutating a key collection retrieved from an associative array is not allowed.");
    }

    sealed class ValueCollection : KeyValueCollection<TValue>
    {
        public ValueCollection(AssociativeArray<TKey, TValue> parent, ICollection<TValue> collection) :
            base(parent, collection)
        {
        }

        protected override Optional<TValue> NullSlot => Parent.m_NullSlot;

        public override bool Contains(TValue item)
        {
            var nullSlot = NullSlot;
            if (nullSlot.HasValue &&
                EqualityComparer<TValue>.Default.Equals(item, nullSlot.Value))
            {
                return true;
            }
            else
            {
                return base.Contains(item);
            }
        }

        [DoesNotReturn]
        protected override void ThrowMutationNotAllowed() =>
            throw new NotSupportedException($"Mutating a value collection retrieved from an associative array is not allowed.");
    }

    sealed class PrependEnumerator<T> : IEnumerator<T>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int m_Index;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool m_EndOfData;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Func<Optional<T>> m_ElementGetter;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IEnumerable<T> m_Source;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IEnumerator<T>? m_SourceEnumerator;

        public PrependEnumerator(
            Func<Optional<T>> elementGetter,
            IEnumerable<T> source)
        {
            m_ElementGetter = elementGetter;
            m_Source = source;
            Current = default!;
        }

        public T Current { get; private set; }

        object IEnumerator.Current
        {
            get
            {
                if (m_Index == 0 || m_EndOfData)
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                return Current!;
            }
        }

        public bool MoveNext()
        {
            if (m_Index == 0)
            {
                var element = m_ElementGetter();
                if (element.HasValue)
                {
                    Current = element.Value;
                    m_Index++;
                    return true;
                }
            }

            var enumerator = m_SourceEnumerator ??= m_Source.GetEnumerator();

            var moveNext = enumerator.MoveNext();
            if (!moveNext)
            {
                m_EndOfData = true;
            }
            else
            {
                Current = enumerator.Current;
                m_Index++;
            }

            return moveNext;
        }

        public void Reset()
        {
            m_Index = 0;
            m_EndOfData = false;
            m_SourceEnumerator?.Reset();
        }

        public void Dispose() { }
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
        public static void ThrowWrongValueTypeArgumentException<T>(T value, Type targetType) =>
            throw new ArgumentException(string.Format(WrongArgumentTypeMessage, value, targetType), nameof(value));

        [DoesNotReturn]
        public static void ThrowWrongKeyTypeArgumentException<T>(T key, Type targetType) =>
            throw new ArgumentException(string.Format(WrongArgumentTypeMessage, key, targetType), nameof(key));

        [DoesNotReturn]
        public static void ThrowKeyNotFoundException(TKey? key) =>
            throw new KeyNotFoundException($"The given key '{key}' was not present in the associative array.");

        [DoesNotReturn]
        public static void ThrowArgumentException_Argument_InvalidArrayType() =>
            throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");

        [DoesNotReturn]
        public static void ThrowAddingDuplicateWithKeyArgumentException(TKey? key) =>
            throw new ArgumentException($"An item with the same key has already been added. Key: '{key}'.");

        [DoesNotReturn]
        public static void ThrowIndexArgumentOutOfRange_NeedNonNegNumException(string argName) =>
            throw new ArgumentOutOfRangeException(argName, "Non-negative number required.");

        [DoesNotReturn]
        public static void ThrowArgumentException_ArrayPlusOffTooSmall() =>
            throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

        [DoesNotReturn]
        public static void ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen() =>
            throw new InvalidOperationException("Enumeration has either not started or has already finished.");
    }
}
