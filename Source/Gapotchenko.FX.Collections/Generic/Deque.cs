// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Stephen Cleary
// Portions © Masashi Mizuno
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Utils;
using Gapotchenko.FX.Linq;
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
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
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
        ExceptionHelper.ThrowIfArgumentIsNegative(capacity);

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
        ExceptionHelper.ThrowIfArgumentIsNull(collection);

        var array = EnumerableHelper.ToArray(collection);
        m_Array = array;
        m_Size = array.Length;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    T[] m_Array;

    /// <summary>
    /// The index of the first collection element in the <see cref="m_Array"/>.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_Offset;

    /// <summary>
    /// The number of elements contained in the <see cref="Deque{T}"/>.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_Size;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_Version;

    const int DefaultCapacity = 4;

    /// <summary>
    /// Gets the number of elements contained in the <see cref="Deque{T}"/>.
    /// </summary>
    public int Count => m_Size;

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
            ExceptionHelper.ValidateIndexArgumentRange(index, m_Size);

            return GetElement(index);
        }
        set
        {
            ExceptionHelper.ValidateIndexArgumentRange(index, m_Size);

            UpdateVersion();
            SetElement(index, value);
        }
    }

    /// <summary>
    /// Removes all items from the <see cref="Deque{T}"/>.
    /// </summary>
    public void Clear()
    {
        UpdateVersion();

        if (m_Size != 0)
        {
#if TFF_RUNTIMEHELPERS_ISREFERENCEORCONTAINSREFERENCES
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
            {
                // Allow garbage collected memory to be freed.
                if (IsSplit)
                {
                    int n = Capacity - m_Offset;
                    Array.Clear(m_Array, m_Offset, n);
                    Array.Clear(m_Array, 0, m_Size - n);
                }
                else
                {
                    Array.Clear(m_Array, m_Offset, m_Size);
                }
            }

            m_Size = 0;
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
    public bool Contains(T item) => m_Size != 0 && IndexOf(item) != -1;

    /// <summary>
    /// Copies the elements of the <see cref="Deque{T}"/> to an <see cref="Array"/>.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="Deque{T}"/>.
    /// The <see cref="Array"/> must have zero-based indexing.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// The number of elements in the source <see cref="Deque{T}"/>
    /// is greater than the available space in the destination array.
    /// </exception>
    public void CopyTo(T[] array)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(array);
        ExceptionHelper.ValidateIndexAndCountArgumentsRange(
            0, m_Size, array.Length,
            indexParameterName: null, countParameterName: null);

        CopyToCore(array, 0);
    }

    /// <summary>
    /// Copies the elements of the <see cref="Deque{T}"/> to an <see cref="Array"/>,
    /// starting at the specified <see cref="Array"/> index.
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
        ExceptionHelper.ThrowIfArgumentIsNull(array);
        ExceptionHelper.ValidateIndexAndCountArgumentsRange(
            arrayIndex, m_Size, array.Length,
            countParameterName: null);

        CopyToCore(array, arrayIndex);
    }

    /// <summary>
    /// Copies the specified number of elements of the <see cref="Deque{T}"/> to an <see cref="Array"/>,
    /// starting at the specified <see cref="Array"/> index.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="Deque{T}"/>.
    /// The <see cref="Array"/> must have zero-based indexing.
    /// </param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <param name="count">The number of elements to copy.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
    /// <exception cref="ArgumentException">
    /// The number of elements in the source <see cref="Deque{T}"/>
    /// is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.
    /// </exception>
    public void CopyTo(T[] array, int arrayIndex, int count)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(array);
        ExceptionHelper.ValidateIndexAndCountArgumentsRange(arrayIndex, count, array.Length);

        CopyToCore(0, array, arrayIndex, count);
    }

    /// <summary>
    /// Copies a range of elements from the <see cref="Deque{T}"/> to an <see cref="Array"/>,
    /// starting at the specified <see cref="Array"/> index.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="Deque{T}"/>.
    /// The <see cref="Array"/> must have zero-based indexing.
    /// </param>
    /// <param name="index">The zero-based index in the source <see cref="Deque{T}"/> at which copying begins.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <param name="count">The number of elements to copy.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="index"/> and <paramref name="count"/>
    /// do not denote a valid range of elements in the <see cref="Deque{T}"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The number of elements in the source <see cref="Deque{T}"/>
    /// is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.
    /// </exception>
    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
        ExceptionHelper.ValidateIndexAndCountArgumentsRange(index, count, m_Size);
        ExceptionHelper.ThrowIfArgumentIsNull(array);
        ExceptionHelper.ValidateIndexAndCountArgumentsRange(arrayIndex, count, array.Length);

        CopyToCore(index, array, arrayIndex, count);
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

            index = Array.IndexOf(m_Array, item, 0, m_Size - n);
            if (index != -1)
                return index + n;
        }
        else
        {
            int index = Array.IndexOf(m_Array, item, m_Offset, m_Size);
            if (index != -1)
                return index - m_Offset;
        }

        return -1;
    }

    /// <summary>
    /// Inserts an object at the beginning of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="item">The object to be inserted at the beginning of the <see cref="Deque{T}"/>.</param>
    public void PushFront(T item)
    {
        UpdateVersion();
        EnsureCapacityForOneElement();
        PushFrontCore(item);
    }

    /// <summary>
    /// Adds an object to the end of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="item">The object to be added to the end of the <see cref="Deque{T}"/>.</param>
    public void PushBack(T item)
    {
        UpdateVersion();
        EnsureCapacityForOneElement();
        PushBackCore(item);
    }

    /// <summary>
    /// Inserts the elements of the specified collection at the beginning of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="collection">The collection whole elements should be inserted at the beginning of the <see cref="Deque{T}"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
    public void PushFrontRange(IEnumerable<T> collection) => InsertRange(0, collection);

    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="collection">The collection whole elements should be added to the end of the <see cref="Deque{T}"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
    public void PushBackRange(IEnumerable<T> collection) => InsertRange(m_Size, collection);

    /// <summary>
    /// Removes and returns the object at the beginning of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <returns>The object removed from the beginning of the <see cref="Deque{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="Deque{T}"/> is empty.</exception>
    public T PopFront()
    {
        if (TryPopFront(out var value))
            return value;
        else
            throw ExceptionHelper.CreateEmptyCollectionException();
    }

    /// <summary>
    /// Removes and returns the object at the end of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <returns>The object removed from the end of the <see cref="Deque{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="Deque{T}"/> is empty.</exception>
    public T PopBack()
    {
        if (TryPopBack(out var value))
            return value;
        else
            throw ExceptionHelper.CreateEmptyCollectionException();
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
    public bool TryPopFront([MaybeNullWhen(false)] out T result)
    {
        int size = m_Size;
        if (size == 0)
        {
            result = default;
            return false;
        }
        else
        {
            UpdateVersion();
            m_Size = size - 1;
            int index = PostIncrementOffset(1);
            result = m_Array[index];
            ClearArrayAt(index);
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
    public bool TryPopBack([MaybeNullWhen(false)] out T result)
    {
        int size = m_Size;
        if (size == 0)
        {
            result = default;
            return false;
        }
        else
        {
            UpdateVersion();
            int index = GetArrayIndex(m_Size = size - 1);
            result = m_Array[index];
            ClearArrayAt(index);
            return true;
        }
    }

    /// <summary>
    /// Returns the object at the beginning of the <see cref="Deque{T}"/> 
    /// without removing it.
    /// </summary>
    /// <returns>The object at the beginning of the <see cref="Deque{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="Deque{T}"/> is empty.</exception>
    public T PeekFront()
    {
        if (TryPeekFront(out var value))
            return value;
        else
            throw ExceptionHelper.CreateEmptyCollectionException();
    }

    /// <summary>
    /// Returns the object at the end of the <see cref="Deque{T}"/> 
    /// without removing it.
    /// </summary>
    /// <returns>The object at the end of the <see cref="Deque{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="Deque{T}"/> is empty.</exception>
    public T PeekBack()
    {
        if (TryPeekBack(out var value))
            return value;
        else
            throw ExceptionHelper.CreateEmptyCollectionException();
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
    public bool TryPeekFront([MaybeNullWhen(false)] out T result)
    {
        if (m_Size == 0)
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
    public bool TryPeekBack([MaybeNullWhen(false)] out T result)
    {
        int size = m_Size;
        if (size == 0)
        {
            result = default;
            return false;
        }
        else
        {
            result = GetElement(size - 1);
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
        ExceptionHelper.ValidateIndexArgumentBounds(index, m_Size);

        UpdateVersion();
        InsertCore(index, item);
    }

    /// <summary>
    /// Inserts the elements of a collection into the <see cref="Deque{T}"/>
    /// at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
    /// <param name="collection">The collection whose elements should be inserted into the <see cref="Deque{T}"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is greater than <see cref="Count"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
    public void InsertRange(int index, IEnumerable<T> collection)
    {
        int size = m_Size;
        ExceptionHelper.ValidateIndexArgumentBounds(index, size);
        ExceptionHelper.ThrowIfArgumentIsNull(collection);

        if (collection.TryReifyNonEnumeratedCollection(out var reifiedCollection))
        {
            int count = reifiedCollection.Count;
            if (count != 0)
            {
                UpdateVersion();
                EnsureCapacityCore(size + count);
                InsertRangePlaceholder(index, count);

                int i = index;
                foreach (var item in collection)
                    SetElement(i++, item);
            }
        }
        else
        {
            int i = index;
            foreach (var item in collection)
                InsertCore(i++, item);

            if (i != index)
                UpdateVersion();
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
        int index = IndexOf(item);
        if (index != -1)
        {
            RemoveAtCore(index);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    ///  Removes the element at the specified index of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is greater than or equal to <see cref="Count"/>.</exception>
    public void RemoveAt(int index)
    {
        ExceptionHelper.ValidateIndexArgumentRange(index, m_Size);

        RemoveAtCore(index);
    }

    /// <summary>
    /// Removes a range of elements from the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
    /// <param name="count">The number of elements to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than 0.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="index"/> and <paramref name="count"/>
    /// do not denote a valid range of elements in the <see cref="Deque{T}"/>.
    /// </exception>
    public void RemoveRange(int index, int count)
    {
        ExceptionHelper.ValidateIndexAndCountArgumentsRange(index, count, m_Size);

        if (count == 0)
            return;

        int size = m_Size;

        if (index == 0)
        {
            // Remove from the beginning.
            ClearRange(0, count);
            UpdateVersion();
            m_Size = size - count;
            PostIncrementOffset(count);
        }
        else if (index == size - count)
        {
            // Remove from the end.
            ClearRange(index, count);
            UpdateVersion();
            m_Size = index;
        }
        else
        {
            // Remove in the middle.
            RemoveMiddleRange(index, count);
        }
    }

    /// <summary>
    /// Removes all elements that match the conditions defined by the specified predicate from the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to remove.</param>
    /// <returns>The number of elements that were removed from the <see cref="Deque{T}"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
    public int RemoveWhere(Predicate<T> match)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(match);

        var size = m_Size;

        int i;
        for (i = 0; i < size; ++i)
            if (match(GetElement(i)))
                break;

        if (i == size)
            return 0;

        for (var j = i + 1; ; ++i, ++j)
        {
            for (; j < size; ++j)
                if (!match(GetElement(j)))
                    break;

            if (j == size)
                break;

            CopyElement(j, i);
        }

        int count = size - i;
        ClearRange(i, count);

        m_Size = i;
        UpdateVersion();

        return count;
    }

    /// <summary>
    /// Reverses the order of the elements in the entire <see cref="Deque{T}"/>.
    /// </summary>
    public void Reverse() => ReverseCore(0, m_Size);

    /// <summary>
    /// Reverses the order of the elements in the specified range.
    /// </summary>
    /// <param name="index">The zero-based starting index of the range to reverse.</param>
    /// <param name="count">The number of elements in the range to reverse.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than 0.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="index"/> and <paramref name="count"/>
    /// do not denote a valid range of elements in the <see cref="Deque{T}"/>.
    /// </exception>
    public void Reverse(int index, int count)
    {
        ExceptionHelper.ValidateIndexAndCountArgumentsRange(index, count, m_Size);

        ReverseCore(index, count);
    }

    /// <summary>
    /// Sorts the elements in the entire <see cref="Deque{T}"/>
    /// using the default comparer.
    /// </summary>
    public void Sort() => Sort((IComparer<T>?)null);

    /// <summary>
    /// Sorts the elements in the entire <see cref="Deque{T}"/>
    /// using the specified comparer.
    /// </summary>
    /// <param name="comparer">
    /// The <see cref="IComparer{T}"/> implementation to use when comparing elements,
    /// or <see langword="null"/> to use the default comparer <see cref="Comparer{T}.Default"/>.
    /// </param>
    public void Sort(IComparer<T>? comparer) => SortCore(0, m_Size, comparer);

    /// <summary>
    /// Sorts the elements in a range of elements in <see cref="Deque{T}"/>
    /// using the specified comparer.
    /// </summary>
    /// <param name="index">The zero-based starting index of the range to sort.</param>
    /// <param name="count">The length of the range to sort.</param>
    /// <param name="comparer">
    /// The <see cref="IComparer{T}"/> implementation to use when comparing elements,
    /// or <see langword="null"/> to use the default comparer <see cref="Comparer{T}.Default"/>.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than 0.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="index"/> and <paramref name="count"/>
    /// do not denote a valid range of elements in the <see cref="Deque{T}"/>.
    /// </exception>
    public void Sort(int index, int count, IComparer<T>? comparer)
    {
        ExceptionHelper.ValidateIndexAndCountArgumentsRange(index, count, m_Size);

        SortCore(index, count, comparer);
    }

    /// <summary>
    /// Sorts the elements in the entire <see cref="Deque{T}"/>
    /// using the specified <see cref="Comparison{T}"/>.
    /// </summary>
    /// <param name="comparison">The method to use when comparing elements.</param>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    public void Sort(Comparison<T> comparison)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(comparison);

        SortCore(0, m_Size, comparison);
    }

    /// <summary>
    /// Sorts the elements in a range of elements in <see cref="Deque{T}"/>
    /// using the specified <see cref="Comparison{T}"/>.
    /// </summary>
    /// <param name="index">The zero-based starting index of the range to sort.</param>
    /// <param name="count">The length of the range to sort.</param>
    /// <param name="comparison">The method to use when comparing elements.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than 0.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="index"/> and <paramref name="count"/>
    /// do not denote a valid range of elements in the <see cref="Deque{T}"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is <see langword="null"/>.</exception>
    public void Sort(int index, int count, Comparison<T> comparison)
    {
        ExceptionHelper.ValidateIndexAndCountArgumentsRange(index, count, m_Size);
        ExceptionHelper.ThrowIfArgumentIsNull(comparison);

        SortCore(index, count, comparison);
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
        ExceptionHelper.ThrowIfArgumentIsNegative(capacity);

        // Always update the version to catch concurrent enumeration errors better.
        UpdateVersion();
        EnsureCapacityCore(capacity);
        return Capacity;
    }

    /// <summary>
    /// Sets the capacity to the actual number of elements in the <see cref="Deque{T}"/>,
    /// if that number is less than a threshold value.
    /// </summary>
    public void TrimExcess()
    {
        // Always update the version to catch concurrent enumeration errors better.
        UpdateVersion();
        SetCapacity(CollectionHelper.TrimExcess(Capacity, m_Size));
    }

    /// <summary>
    /// Copies the <see cref="Deque{T}"/> to a new array.
    /// </summary>
    /// <returns>A new array containing copies of the elements of the <see cref="Deque{T}"/>.</returns>
    public T[] ToArray()
    {
        int size = m_Size;
        if (size == 0)
        {
            return Array.Empty<T>();
        }
        else
        {
            var result = new T[size];
            CopyToCore(result, 0);
            return result;
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="Deque{T}"/>.
    /// </summary>
    /// <returns>A <see cref="Enumerator"/> for the <see cref="Deque{T}"/>.</returns>
    public Enumerator GetEnumerator() => new(this);

    // ----------------------------------------------------------------------

    /// <summary>
    /// Gets a collection element by the specified index.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    T GetElement(int index)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(index < m_Size);

        return m_Array[GetArrayIndex(index)];
    }

    /// <summary>
    /// Sets a collection element by the specified index.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void SetElement(int index, T value)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(index < Capacity);

        m_Array[GetArrayIndex(index)] = value;
    }

    /// <summary>
    /// Gets an array index by the index of a collection element.
    /// </summary>
    /// <param name="elementIndex">The collection element index.</param>
    /// <returns>The array index.</returns>
    int GetArrayIndex(int elementIndex)
    {
        Debug.Assert(elementIndex >= 0);
        Debug.Assert(elementIndex < Capacity);

        return (m_Offset + elementIndex) % Capacity;
    }

    /// <summary>
    /// Gets an array index by the index of a collection element in a contiguous range.
    /// </summary>
    /// <param name="elementIndex">The collection element index in a contiguous range.</param>
    /// <returns>The array index.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int GetContiguousArrayIndex(int elementIndex)
    {
        Debug.Assert(elementIndex >= 0);
        Debug.Assert(elementIndex < Capacity - m_Offset);

        return m_Offset + elementIndex;
    }

    void CopyToCore(Array array, int arrayIndex)
    {
        Debug.Assert(array != null);
        Debug.Assert(arrayIndex >= 0);
        Debug.Assert(arrayIndex <= array.Length - m_Size);

        if (IsSplit)
        {
            int n = Capacity - m_Offset;
            Array.Copy(m_Array, m_Offset, array, arrayIndex, n);
            Array.Copy(m_Array, 0, array, arrayIndex + n, m_Size - n);
        }
        else
        {
            Array.Copy(m_Array, m_Offset, array, arrayIndex, m_Size);
        }
    }

    void CopyToCore(int index, Array array, int arrayIndex, int count)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(array != null);
        Debug.Assert(arrayIndex >= 0);
        Debug.Assert(count >= 0);
        Debug.Assert(index <= m_Size - count);
        Debug.Assert(arrayIndex <= array.Length - count);

        if (IsContiguousRange(index, count))
        {
            Array.Copy(m_Array, GetContiguousArrayIndex(index), array, arrayIndex, count);
        }
        else
        {
            var n = Math.Max(Capacity - m_Offset - index, count);
            if (n >= 0)
            {
                Array.Copy(m_Array, m_Offset + index, array, arrayIndex, n);
                if (count > n)
                    Array.Copy(m_Array, 0, array, arrayIndex + n, count - n);
            }
            else
            {
                Array.Copy(m_Array, -n, array, arrayIndex, count);
            }
        }
    }

    /// <summary>
    /// Increases version to invalidate concurrent enumerations.
    /// </summary>
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

    void PushFrontCore(T value)
    {
        ++m_Size;
        m_Array[PreDecrementOffset(1)] = value;
    }

    void PushBackCore(T value)
    {
        int size = m_Size;
        m_Size = size + 1;
        SetElement(size, value);
    }

    void InsertCore(int index, T item)
    {
        EnsureCapacityForOneElement();

        if (index == 0)
        {
            PushFrontCore(item);
        }
        else if (index == m_Size)
        {
            PushBackCore(item);
        }
        else
        {
            InsertRangePlaceholder(index, 1);
            SetElement(index, item);
        }
    }

    void InsertRangePlaceholder(int index, int count)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(count >= 0);
        Debug.Assert(index <= m_Size - count);

        int size = m_Size;

        if (IsFirstHalf(index))
        {
            CopyRange(0, Capacity - count, index);
            PreDecrementOffset(count);
        }
        else
        {
            CopyRange(index, index + count, size - index);
        }

        m_Size = size + count;
    }

    void RemoveAtCore(int index)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(index <= m_Size - 1);

        int size = m_Size - 1;

        if (index == 0)
        {
            // Remove from the beginning.
            UpdateVersion();
            m_Size = size;
            ClearArrayAt(PostIncrementOffset(1));
        }
        else if (index == size)
        {
            // Remove from the end.
            UpdateVersion();
            ClearAt(m_Size = size);
        }
        else
        {
            // Remove in the middle.
            RemoveMiddleRange(index, 1);
        }
    }

    void RemoveMiddleRange(int index, int count)
    {
        Debug.Assert(index > 0);
        Debug.Assert(count > 0);
        Debug.Assert(index < m_Size - count);

        int size = m_Size;
        int rangeEnd = checked(index + count);

        if (index < size - rangeEnd)
        {
            // Removing from the first half of the collection.
            CopyRange(0, count, index);
            ClearRange(0, count);
            PostIncrementOffset(count);
        }
        else
        {
            // Removing from the second half of the collection.
            CopyRange(rangeEnd, index, size - rangeEnd);
            ClearRange(size - count, count);
        }

        m_Size = size - count;
        UpdateVersion();
    }

    void ReverseCore(int index, int count)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(count >= 0);
        Debug.Assert(index <= m_Size - count);

        UpdateVersion();
        EnsureContiguous(index, count);

        Array.Reverse(m_Array, GetContiguousArrayIndex(index), count);
    }

    void SortCore(int index, int count, IComparer<T>? comparer)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(count >= 0);
        Debug.Assert(index <= m_Size - count);

        UpdateVersion();
        EnsureContiguous(index, count);

        Array.Sort(m_Array, GetContiguousArrayIndex(index), count, comparer);
    }

    void SortCore(int index, int count, Comparison<T> comparison)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(count >= 0);
        Debug.Assert(index <= m_Size - count);

        UpdateVersion();
        EnsureContiguous(index, count);

#if NET5_0_OR_GREATER
        new Span<T>(m_Array, GetContiguousArrayIndex(index), count).Sort(comparison);
#else
        Array.Sort(m_Array, GetContiguousArrayIndex(index), count, Comparer<T>.Create(comparison));
#endif
    }

    void ClearAt(int index)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(index < Capacity);

#if TFF_RUNTIMEHELPERS_ISREFERENCEORCONTAINSREFERENCES
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
        {
            // Allow garbage collected memory to be freed.
            m_Array[GetArrayIndex(index)] = default!;
        }
    }

    void ClearArrayAt(int arrayIndex)
    {
        Debug.Assert(arrayIndex >= 0);
        Debug.Assert(arrayIndex < Capacity);

#if TFF_RUNTIMEHELPERS_ISREFERENCEORCONTAINSREFERENCES
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
        {
            // Allow garbage collected memory to be freed.
            m_Array[arrayIndex] = default!;
        }
    }

    void ClearRange(int index, int count)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(count >= 0);
        Debug.Assert(index <= m_Size - count);

#if TFF_RUNTIMEHELPERS_ISREFERENCEORCONTAINSREFERENCES
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
        {
#if false
            // Canonical Algorithm

            for (int i = 0; i < count; ++i)
                SetElement(index + i, default!);
#else
            // Fast Algorithm

            var array = m_Array;
            var capacity = array.Length;
            var arrayIndex = GetArrayIndex(index);

            if (arrayIndex <= capacity - count)
            {
                Array.Clear(array, arrayIndex, count);
            }
            else
            {
                int n = capacity - arrayIndex;
                Array.Clear(array, arrayIndex, n);
                Array.Clear(array, 0, count - n);
            }
#endif
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void CopyElement(int sourceIndex, int destinationIndex) => SetElement(destinationIndex, GetElement(sourceIndex));

    void CopyRange(int sourceIndex, int destinationIndex, int count)
    {
        Debug.Assert(sourceIndex >= 0);
        Debug.Assert(sourceIndex < Capacity);
        Debug.Assert(destinationIndex >= 0);
        Debug.Assert(destinationIndex < Capacity);
        Debug.Assert(count >= 0);
        Debug.Assert(sourceIndex <= Capacity - count);
        Debug.Assert(destinationIndex <= Capacity - count);

        if (count == 0 || sourceIndex == destinationIndex)
            return;

#if false
        // Canonical Algorithm

        if (sourceIndex > destinationIndex)
        {
            for (int i = 0; i < count; ++i)
                CopyElement(sourceIndex + i, destinationIndex + i);
        }
        else
        {
            for (int i = count - 1; i >= 0; --i)
                CopyElement(sourceIndex + i, destinationIndex + i);
        }
#else
        // Fast Algorithm (by Masashi Mizuno)

        var array = m_Array;
        var capacity = array.Length;
        var srcIndex = GetArrayIndex(sourceIndex);
        var dstIndex = GetArrayIndex(destinationIndex);

        if (srcIndex <= dstIndex)
        {
            var a = Math.Min(capacity - dstIndex, count);
            var c = Math.Max(srcIndex + count - capacity, 0);
            var b = count - (a + c);

            if (srcIndex + count <= dstIndex)
            {
                //  s                               |
                //  +---------------+               |
                //  |       A       |               |
                //  +---------------+               |
                //  |               +---------------+
                //  |               |       A'      |
                //  |               +---------------+
                //  |               d               |

                //  |  s                            |
                //  |  +---------+-----+            |
                //  |  |    A    |  B  |            |
                //  |  +---------+-----+            |
                //  |                     +---------+-----+
                //  |                     |    A'   |  B' |
                //  |                     +---------+-----+
                //  |                     d         |

                // In this case, A' ∩ B = ∅ holds.
                Array.Copy(array, srcIndex, array, dstIndex, a);
                if (b > 0)
                    Array.Copy(array, srcIndex + a, array, 0, b);
            }
            else
            {
                // By (*), d + count ≤ s + Capacity.

                //  |   s                           |   s + Capacity
                //  |   +---------------+           |   +---
                //  |   |       A       |           |   |
                //  |   +---------------+           |   +---
                //  |           +---------------+   |
                //  |           |       A'      |   |
                //  |           +---------------+   |
                //  |           d                   |

                //  |           s                   |           s + Capacity
                //  |           +-----------+---+   |           +---
                //  |           |     A     | B |   |           |
                //  |           +-----------+---+   |           +---
                //  |                   +-----------+---+
                //  |                   |     A'    | B'|
                //  |                   +-----------+---+
                //  |                   d           |

                //  |                   s           |                   s + Capacity
                //  |                   +---+-------+---+               +---
                //  |                   | A |   B   | C |               |
                //  |                   +---+-------+---+               +---
                //  |                           +---+-------+---+
                //  |                           | A'|   B'  | C'|
                //  |                           +---+-------+---+
                //  |                           d   |

                // In this case, C' ∩ (A ∪ B) = ∅ and B' ∩ A = ∅ hold.
                if (c > 0)
                    Array.Copy(array, 0, array, b, c);
                if (b > 0)
                    Array.Copy(array, srcIndex + a, array, 0, b);
                Array.Copy(array, srcIndex, array, dstIndex, a);
            }
        }
        else
        {
            var a = Math.Min(capacity - srcIndex, count);
            var c = Math.Max(dstIndex + count - capacity, 0);
            var b = count - (a + c);

            if (dstIndex + count <= srcIndex)
            {
                //  |               s               |
                //  |               +---------------+
                //  |               |       A       |
                //  |               +---------------+
                //  +---------------+               |
                //  |       A'      |               |
                //  +---------------+               |
                //  d                               |

                //  |                     s         |
                //  |                     +---------+-----+
                //  |                     |    A    |  B  |
                //  |                     +---------+-----+
                //  |  +---------+-----+            |
                //  |  |    A'   |  B' |            |
                //  |  +---------+-----+            |
                //  |  d                            |

                // In this case, B' ∩ A = ∅ holds.
                if (b > 0)
                    Array.Copy(array, 0, array, dstIndex + a, b);
                Array.Copy(array, srcIndex, array, dstIndex, a);
            }
            else
            {
                // By (*), s + count ≤ d + Capacity.

                //  |           s                   |
                //  |           +---------------+   |
                //  |           |       A       |   |
                //  |           +---------------+   |
                //  |   +---------------+           |   +---
                //  |   |       A'      |           |   |
                //  |   +---------------+           |   +---
                //  |   d                           |   d + Capacity

                //  |                   s           |
                //  |                   +-----------+---+
                //  |                   |     A     | B |
                //  |                   +-----------+---+
                //  |           +-----------+---+   |           +---
                //  |           |     A'    | B'|   |           |
                //  |           +-----------+---+   |           +---
                //  |           d                   |           d + Capacity

                //  |                           s   |
                //  |                           +---+-------+---+
                //  |                           | A |   B   | C |
                //  |                           +---+-------+---+
                //  |                   +---+-------+---+               +---
                //  |                   | A'|   B'  | C'|               |
                //  |                   +---+-------+---+               +---
                //  |                   d           |                   d + Capacity

                // In this case, A' ∩ (B ∪ C) = ∅ and B' ∩ C = ∅ hold.
                Array.Copy(array, srcIndex, array, dstIndex, a);
                if (b > 0)
                    Array.Copy(array, 0, array, dstIndex + a, b);
                if (c > 0)
                    Array.Copy(array, b, array, 0, c);
            }
        }
#endif
    }

    int Capacity => m_Array.Length;

    void EnsureCapacityCore(int capacity)
    {
        Debug.Assert(capacity >= 0);

        if (capacity > Capacity)
            Grow(capacity);
    }

    void EnsureCapacityForOneElement()
    {
        int size = m_Size;
        if (size == Capacity)
            Grow(size + 1);
    }

    void Grow(int capacity)
    {
        Debug.Assert(capacity > Capacity);

        SetCapacity(CollectionHelper.GrowCapacity(Capacity, capacity, DefaultCapacity));
    }

    void SetCapacity(int capacity)
    {
        if (capacity != Capacity)
            ReallocateArray(capacity);
    }

    void MakeContiguous()
    {
        Debug.Assert(IsSplit);

        ReallocateArray(Capacity);
    }

    void EnsureContiguous(int index, int count)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(count >= 0);
        Debug.Assert(index <= m_Size - count);

        if (!IsContiguousRange(index, count))
            MakeContiguous();
    }

    /// <summary>
    /// Reallocates <see cref="m_Array"/> for it:
    /// <list type="bullet">
    /// <item>to accommodate the specified number of elements, and</item>
    /// <item>to become contiguous.</item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// This is a non-atomic operation requiring a version update in a calling algorithm.
    /// </remarks>
    /// <param name="capacity">The number of elements <see cref="m_Array"/> should be able to accommodate.</param>
    void ReallocateArray(int capacity)
    {
        Debug.Assert(capacity >= m_Size); // ensure that existing elements stored in array are not capped

        var newArray = new T[capacity];
        CopyToCore(newArray, 0);

        // Changing both array and offset fields is a non-atomic operation.
        m_Array = newArray;
        m_Offset = 0;
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="m_Array"/> is split in two parts
    /// meaning that the beginning of the collection is stored at a later array index than the end.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IsSplit => m_Offset > Capacity - m_Size; // overflow-safe equivalent of "m_Offset + m_Count > Capacity"

    bool IsFirstHalf(int index) => index < m_Size - index;

    bool IsContiguousRange(int index, int count) =>
        count == 0 ||
        GetArrayIndex(index) <= GetArrayIndex(index + count - 1);

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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Deque<T> m_Deque;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly int m_Version;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int m_Index;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        T? m_Current;

        /// <summary>
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        /// <value>
        /// The element in the <see cref="Deque{T}"/> at the current position of the enumerator.
        /// </value>
        public readonly T Current => m_Current!;

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

            if (m_Version == deque.m_Version && (uint)m_Index < deque.m_Size)
            {
                m_Current = deque.GetElement(m_Index++);
                return true;
            }
            else
            {
                return MoveNextRare();
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            ValidateVersion();

            m_Index = 0;
            m_Current = default;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="Enumerator"/>.
        /// </summary>
        public void Dispose()
        {
        }

        // ------------------------------------------------------------------

        bool MoveNextRare()
        {
            ValidateVersion();

            m_Index = m_Deque.m_Size + 1;
            m_Current = default;
            return false;
        }

        readonly void ValidateVersion()
        {
            if (m_Version != m_Deque.m_Version)
                throw ExceptionHelper.CreateEnumeratedCollectionWasModifiedException();
        }

        #region Compatibility

        readonly object? IEnumerator.Current
        {
            get
            {
                if (m_Index == 0 || m_Index > m_Deque.m_Size)
                    throw ExceptionHelper.CreateEnumerationEitherNotStarterOrFinishedException();

                return Current;
            }
        }

        #endregion
    }

    #region Compatibility

    /// <summary>
    /// Do not use this method.
    /// Instead, use <see cref="PushBackRange(IEnumerable{T})"/> method.
    /// </summary>
    /// <inheritdoc cref="PushBackRange(IEnumerable{T})"/>
    [Obsolete("Use PushBackRange method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AddRange(IEnumerable<T> collection) => PushBackRange(collection);

    /// <inheritdoc cref="AddRange(IEnumerable{T})"/>
    [Obsolete("Use PushBackRange method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AddRange(params T[] collection) => PushBackRange(collection);

    #region ICollection<T>

    void ICollection<T>.Add(T item) => PushBack(item);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => false;

    #endregion

    #region IList

    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = CollectionHelper.GetCompatibleValue<T>(value);
    }

    int IList.Add(object? value)
    {
        PushBack(CollectionHelper.GetCompatibleValue<T>(value));
        return m_Size - 1;
    }

    bool IList.Contains(object? value) =>
        CollectionHelper.TryGetCompatibleValue<T>(value, out var compatibleValue) &&
        Contains(compatibleValue);

    int IList.IndexOf(object? value)
    {
        if (CollectionHelper.TryGetCompatibleValue<T>(value, out var compatibleValue))
            return IndexOf(compatibleValue);
        else
            return -1;
    }

    void IList.Insert(int index, object? value) =>
        Insert(index, CollectionHelper.GetCompatibleValue<T>(value));

    void IList.Remove(object? value)
    {
        if (CollectionHelper.TryGetCompatibleValue<T>(value, out var compatibleValue))
            Remove(compatibleValue);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IList.IsReadOnly => false;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IList.IsFixedSize => false;

    #endregion

    #region ICollection

    void ICollection.CopyTo(Array array, int index)
    {
        ExceptionHelper.ThrowIfArgumentIsNull(array);
        ExceptionHelper.ThrowIfArrayArgumentIsMultiDimensional(array);
        ExceptionHelper.ThrowIfArgumentIsNegative(index);

        try
        {
            CopyToCore(array, index);
        }
        catch (ArrayTypeMismatchException)
        {
            throw ExceptionHelper.CreateIncompatibleArrayTypeArgumentException();
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection.IsSynchronized => false;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static object? m_SyncRoot;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object ICollection.SyncRoot => LazyInitializerEx.EnsureInitialized(ref m_SyncRoot);

    #endregion

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}
