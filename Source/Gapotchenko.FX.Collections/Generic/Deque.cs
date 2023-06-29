using Gapotchenko.FX.Collections.Utils;
using Gapotchenko.FX.Threading;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Collections.Generic;

/// <summary>
/// Represents a linear collection that supports element insertion and removal at both ends with O(1) algorithmic complexity.
/// </summary>
/// <remarks>
/// The name "deque" is acronym for "<u>d</u>ouble <u>e</u>nded <u>que</u>ue" and is usually pronounced "deck".
/// </remarks>
public class Deque<T> : IList<T>, IReadOnlyList<T>, IList
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Deque{T}"/> class that is empty
    /// and has the default initial capacity.
    /// </summary>
    public Deque()
    {
        m_Array = Array.Empty<T>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Deque{T}"/> class that is empty
    /// and has the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the <see cref="Deque{T}"/> can contain.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
    public Deque(int capacity)
    {
        ExceptionHelpers.ThrowIfArgumentIsNegative(capacity);

        m_Array = new T[capacity];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Deque{T}"/> class
    /// that contains elements copied from the specified collection
    /// and has sufficient capacity to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection to copy elements from.</param>
    public Deque(IEnumerable<T> collection)
    {
        ExceptionHelpers.ThrowIfArgumentIsNull(collection);

        m_Array = EnumerableHelpers.ToArray(collection);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    T[] m_Array = Array.Empty<T>();

    /// <summary>
    /// The index of the first collection element in the <see cref="m_Array"/>.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_Offset;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_Count;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_Version;

    const int DefaultCapacity = 4;

    /// <summary>
    /// Gets the number of elements contained in the <see cref="Deque{T}"/>.
    /// </summary>
    public int Count => m_Count;

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is greater than or equal to <see cref="Count"/>.</exception>
    public T this[int index]
    {
        get
        {
            ExceptionHelpers.ValidateIndexArgumentRange(index, m_Count);

            return GetItemCore(index);
        }
        set
        {
            ExceptionHelpers.ValidateIndexArgumentRange(index, m_Count);

            m_Array[GetArrayIndex(index)] = value;
            UpdateVersion();
        }
    }

    /// <summary>
    /// Removes all items from the <see cref="Deque{T}"/>.
    /// </summary>
    public void Clear()
    {
        UpdateVersion();

        if (m_Count != 0)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
            {
                if (IsSplit)
                {
                    int n = Capacity - m_Offset;
                    Array.Clear(m_Array, m_Offset, n);
                    Array.Clear(m_Array, 0, m_Count - n);
                }
                else
                {
                    Array.Clear(m_Array, m_Offset, m_Count);
                }
            }

            m_Count = 0;
        }

        m_Offset = 0;
    }

    /// <summary>
    /// Determines whether the <see cref="Deque{T}"/> contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="Deque{T}"/>.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="item"/> is found in the <see cref="Deque{T}"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool Contains(T item) => m_Count != 0 && IndexOf(item) != -1;

    /// <summary>
    /// Copies the elements of the <see cref="Deque{T}"/> to an <see cref="Array"/>,
    /// starting at a particular <see cref="Array"/> index.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="Deque{T}"/>.
    /// The <see cref="Array"/> must have zero-based indexing.
    /// </param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
    /// <exception cref="ArgumentException">
    /// The number of elements in the source <see cref="Deque{T}"/>
    /// is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.
    /// </exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ExceptionHelpers.ThrowIfArgumentIsNull(array);

        CopyToArrayCore(array, arrayIndex);
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence
    /// within the entire <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="Deque{T}"/>.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of <paramref name="item"/> within the entire <see cref="Deque{T}"/>, if found;
    /// otherwise, -1.
    /// </returns>
    public int IndexOf(T item)
    {
        if (IsSplit)
        {
            int n = Capacity - m_Offset;

            int index = Array.IndexOf(m_Array, item, m_Offset, n);
            if (index != -1)
                return index - m_Offset;

            index = Array.IndexOf(m_Array, item, 0, m_Count - n);
            if (index != -1)
                return index + n;
        }
        else
        {
            int index = Array.IndexOf(m_Array, item, m_Offset, m_Count);
            if (index != -1)
                return index - m_Offset;
        }

        return -1;
    }

    /// <summary>
    /// Inserts an object at the beginning of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="item">The object to be inserted at the beginning of the <see cref="Deque{T}"/>.</param>
    public void PushToFront(T item)
    {
        EnsureCapacityForOneElement();
        PushToFrontCore(item);
    }

    /// <summary>
    /// Adds an object to the end of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="item">The object to be added to the end of the <see cref="Deque{T}"/>.</param>
    public void PushToBack(T item)
    {
        EnsureCapacityForOneElement();
        PushToBackCore(item);
    }

    /// <summary>
    /// Removes and returns the object at the beginning of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <returns>The object removed from the beginning of the <see cref="Deque{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="Deque{T}"/> is empty.</exception>
    public T PopFromFront()
    {
        if (TryPopFromFront(out var value))
            return value;
        else
            throw ExceptionHelpers.CreateEmptyCollectionException();
    }

    /// <summary>
    /// Removes and returns the object at the end of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <returns>The object removed from the end of the <see cref="Deque{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="Deque{T}"/> is empty.</exception>
    public T PopFromBack()
    {
        if (TryPopFromBack(out var value))
            return value;
        else
            throw ExceptionHelpers.CreateEmptyCollectionException();
    }

    /// <summary>
    /// Returns a value that indicates whether there is an object at the beginning of the <see cref="Deque{T}"/>,
    /// and if one is present, copies it to the result parameter, and removes it from the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="result">
    /// If present, the object at the beginning of the <see cref="Deque{T}"/>;
    /// otherwise, the default value of <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if there is an object at the beginning of <see cref="Deque{T}"/>;
    /// <see langword="false"/> if the <see cref="Deque{T}"/> is empty.
    /// </returns>
    public bool TryPopFromFront([MaybeNullWhen(false)] out T result)
    {
        int count = m_Count;
        if (count == 0)
        {
            result = default;
            return false;
        }
        else
        {
            result = m_Array[PostIncrementOffset(1)];
            m_Count = count - 1;
            UpdateVersion();
            return true;
        }
    }

    /// <summary>
    /// Returns a value that indicates whether there is an object at the end of the <see cref="Deque{T}"/>,
    /// and if one is present, copies it to the result parameter, and removes it from the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="result">
    /// If present, the object at the end of the <see cref="Deque{T}"/>;
    /// otherwise, the default value of <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if there is an object at the end of <see cref="Deque{T}"/>;
    /// <see langword="false"/> if the <see cref="Deque{T}"/> is empty.
    /// </returns>
    public bool TryPopFromBack([MaybeNullWhen(false)] out T result)
    {
        int count = m_Count;
        if (count == 0)
        {
            result = default;
            return false;
        }
        else
        {
            result = m_Array[GetArrayIndex(m_Count = count - 1)];
            UpdateVersion();
            return true;
        }
    }

    /// <summary>
    /// Returns the object at the beginning of the <see cref="Deque{T}"/> 
    /// without removing it.
    /// </summary>
    /// <returns>The object at the beginning of the <see cref="Deque{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="Deque{T}"/> is empty.</exception>
    public T PeekFromFront()
    {
        if (TryPeekFromFront(out var value))
            return value;
        else
            throw ExceptionHelpers.CreateEmptyCollectionException();
    }

    /// <summary>
    /// Returns the object at the end of the <see cref="Deque{T}"/> 
    /// without removing it.
    /// </summary>
    /// <returns>The object at the end of the <see cref="Deque{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="Deque{T}"/> is empty.</exception>
    public T PeekFromBack()
    {
        if (TryPeekFromBack(out var value))
            return value;
        else
            throw ExceptionHelpers.CreateEmptyCollectionException();
    }

    /// <summary>
    /// Returns a value that indicates whether there is an object at the beginning of the <see cref="Deque{T}"/>,
    /// and if one is present, copies it to the <paramref name="result"/> parameter.
    /// The object is not removed from the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="result">
    /// If present, the object at the beginning of the <see cref="Deque{T}"/>;
    /// otherwise, the default value of <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if there is an object at the beginning of <see cref="Deque{T}"/>;
    /// <see langword="false"/> if the <see cref="Deque{T}"/> is empty.
    /// </returns>
    public bool TryPeekFromFront([MaybeNullWhen(false)] out T result)
    {
        if (m_Count == 0)
        {
            result = default;
            return false;
        }
        else
        {
            result = m_Array[m_Offset];
            return true;
        }
    }

    /// <summary>
    /// Returns a value that indicates whether there is an object at the end of the <see cref="Deque{T}"/>,
    /// and if one is present, copies it to the <paramref name="result"/> parameter.
    /// The object is not removed from the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="result">
    /// If present, the object at the end of the <see cref="Deque{T}"/>;
    /// otherwise, the default value of <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if there is an object at the end of <see cref="Deque{T}"/>;
    /// <see langword="false"/> if the <see cref="Deque{T}"/> is empty.
    /// </returns>
    public bool TryPeekFromBack([MaybeNullWhen(false)] out T result)
    {
        int count = m_Count;
        if (count == 0)
        {
            result = default;
            return false;
        }
        else
        {
            result = GetItemCore(count - 1);
            return true;
        }
    }

    /// <summary>
    /// Inserts an element into the <see cref="Deque{T}"/>
    /// at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert. The value can be null for reference types.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is greater than <see cref="Count"/>.</exception>
    public void Insert(int index, T item)
    {
        int count = m_Count;
        ExceptionHelpers.ValidateIndexArgumentBounds(index, count);

        EnsureCapacityForOneElement();

        if (index == 0)
        {
            PushToFrontCore(item);
        }
        else if (index == count)
        {
            PushToBackCore(item);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="item">The object to remove from the <see cref="Deque{T}"/>.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="item"/> is successfully removed;
    /// otherwise, <see langword="false"/>.
    /// This method also returns <see langword="false"/> if item was not found in the <see cref="Deque{T}"/>.
    /// </returns>
    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///  Removes the element at the specified index of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is greater than or equal to <see cref="Count"/>.</exception>
    public void RemoveAt(int index)
    {
        ExceptionHelpers.ValidateIndexArgumentRange(index, m_Count);

        throw new NotImplementedException();
    }

    /// <summary>
    /// Ensures that the capacity of the <see cref="Deque{T}"/> is greater than or equal to the specified <paramref name="capacity"/>.
    /// Otherwise, the <see cref="Deque{T}"/> capacity will be doubled until it reaches or exceeds the specified value.
    /// </summary>
    /// <param name="capacity">The minimum capacity to provision.</param>
    /// <returns>The new capacity of the <see cref="Deque{T}"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
    public int EnsureCapacity(int capacity)
    {
        ExceptionHelpers.ThrowIfArgumentIsNegative(capacity);

        if (Capacity < capacity)
            Grow(capacity);

        return Capacity;
    }

    /// <summary>
    /// Sets the capacity to the actual number of elements in the <see cref="Deque{T}"/>,
    /// if that number is less than a threshold value.
    /// </summary>
    public void TrimExcess()
    {
        CollectionHelpers.TrimExcess(ref m_Array, m_Count);
    }

    /// <summary>
    /// Copies the <see cref="Deque{T}"/> to a new array.
    /// </summary>
    /// <returns>A new array containing copies of the elements of the <see cref="Deque{T}"/>.</returns>
    public T[] ToArray()
    {
        int count = m_Count;
        if (count == 0)
        {
            return Array.Empty<T>();
        }
        else
        {
            var result = new T[count];
            CopyToArrayCore(result, 0);
            return result;
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="Deque{T}"/>.
    /// </summary>
    /// <returns>A <see cref="Enumerator"/> for the <see cref="Deque{T}"/>.</returns>
    public Enumerator GetEnumerator() => new(this);

    // ----------------------------------------------------------------------

    T GetItemCore(int index) => m_Array[GetArrayIndex(index)];

    /// <summary>
    /// Gets an array index by the index of a collection element.
    /// </summary>
    /// <param name="elementIndex">The collection element index.</param>
    /// <returns>The array index.</returns>
    int GetArrayIndex(int elementIndex) => (m_Offset + elementIndex) % Capacity;

    void CopyToArrayCore(Array array, int arrayIndex)
    {
        ExceptionHelpers.ThrowIfArgumentIsNegative(arrayIndex);

        if (IsSplit)
        {
            int n = Capacity - m_Offset;
            Array.Copy(m_Array, m_Offset, array, arrayIndex, n);
            Array.Copy(m_Array, 0, array, arrayIndex + n, m_Count - n);
        }
        else
        {
            Array.Copy(m_Array, m_Offset, array, arrayIndex, m_Count);
        }
    }

    void UpdateVersion() => ++m_Version;

    int PostIncrementOffset(int addend)
    {
        int offset = m_Offset;
        m_Offset = (offset + addend) % Capacity;
        return offset;
    }

    int PreDecrementOffset(int subtrahend)
    {
        int offset = m_Offset - subtrahend;
        if (offset < 0)
            offset += Capacity;
        return m_Offset = offset;
    }

    void PushToFrontCore(T value)
    {
        m_Array[PreDecrementOffset(1)] = value;
        ++m_Count;
        UpdateVersion();
    }

    void PushToBackCore(T value)
    {
        int count = m_Count;
        m_Array[GetArrayIndex(count)] = value;
        m_Count = count + 1;
        UpdateVersion();
    }

    int Capacity => m_Array.Length;

    void Grow(int capacity)
    {
        CollectionHelpers.GrowCapacity(ref m_Array, capacity, DefaultCapacity);
    }

    void EnsureCapacityForOneElement()
    {
        int count = m_Count;
        if (count == Capacity)
            Grow(count + 1);
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="m_Array"/> is split in two parts
    /// meaning that the beginning of the collection is stored at a later array index than the end.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IsSplit => m_Offset > Capacity - m_Count; // overflow-safe equivalent of "m_Offset + m_Count > Capacity"

    /// <summary>
    /// Enumerates the elements of a <see cref="Deque{T}"/>.
    /// </summary>
    public struct Enumerator : IEnumerator<T>
    {
        internal Enumerator(Deque<T> deque)
        {
            m_Deque = deque;
            m_Version = deque.m_Version;
        }

        readonly Deque<T> m_Deque;
        readonly int m_Version;
        int m_Index;
        T? m_Current;

        /// <summary>
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        /// <value>
        /// The element in the <see cref="Deque{T}"/> at the current position of the enumerator.
        /// </value>
        public readonly T Current => m_Current!;

        readonly object? IEnumerator.Current
        {
            get
            {
                if (m_Index == 0 || m_Index > m_Deque.m_Count)
                    throw new InvalidOperationException("Enumeration has either not started or has already finished.");

                return Current;
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="Enumerator"/>.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Advances the enumerator to the next element of the <see cref="Deque{T}"/>.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator was successfully advanced to the next element;
        /// <see langword="false"/> if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        public bool MoveNext()
        {
            var deque = m_Deque;

            if (m_Version == deque.m_Version && (uint)m_Index < deque.m_Count)
            {
                m_Current = deque.GetItemCore(m_Index++);
                return true;
            }
            else
            {
                return MoveNextRare();
            }
        }

        bool MoveNextRare()
        {
            ValidateVersion();

            m_Index = m_Deque.m_Count + 1;
            m_Current = default;
            return false;
        }

        void IEnumerator.Reset()
        {
            ValidateVersion();

            m_Index = 0;
            m_Current = default;
        }

        void ValidateVersion()
        {
            if (m_Version != m_Deque.m_Version)
                throw ExceptionHelpers.CreateEnumeratedCollectionWasModifiedException();
        }
    }

    #region Compatibility

    #region ICollection<T>

    void ICollection<T>.Add(T item) => PushToBack(item);

    bool ICollection<T>.IsReadOnly => false;

    #endregion

    #region IList

    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = CollectionHelpers.GetCompatibleValue<T>(value);
    }

    int IList.Add(object? value)
    {
        PushToBack(CollectionHelpers.GetCompatibleValue<T>(value));
        return m_Count - 1;
    }

    bool IList.Contains(object? value) =>
        CollectionHelpers.TryGetCompatibleValue<T>(value, out var compatibleValue) &&
        Contains(compatibleValue);

    int IList.IndexOf(object? value)
    {
        if (CollectionHelpers.TryGetCompatibleValue<T>(value, out var compatibleValue))
            return IndexOf(compatibleValue);
        else
            return -1;
    }

    void IList.Insert(int index, object? value) =>
        Insert(index, CollectionHelpers.GetCompatibleValue<T>(value));

    void IList.Remove(object? value)
    {
        if (CollectionHelpers.TryGetCompatibleValue<T>(value, out var compatibleValue))
            Remove(compatibleValue);
    }

    bool IList.IsReadOnly => false;

    bool IList.IsFixedSize => false;

    #endregion

    #region ICollection

    void ICollection.CopyTo(Array array, int index)
    {
        ExceptionHelpers.ThrowIfArgumentIsNull(array);
        ExceptionHelpers.ThrowIfArrayArgumentIsMultiDimensional(array);

        try
        {
            CopyToArrayCore(array, index);
        }
        catch (ArrayTypeMismatchException)
        {
            throw ExceptionHelpers.CreateIncompatibleArrayTypeException();
        }
    }

    bool ICollection.IsSynchronized => false;

    static object? m_SyncRoot;

    object ICollection.SyncRoot => LazyInitializerEx.EnsureInitialized(ref m_SyncRoot);

    #endregion

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}
