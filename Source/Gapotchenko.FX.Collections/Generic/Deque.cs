using Gapotchenko.FX.Threading;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Collections.Generic;

/// <summary>
/// Represents a linear collection that supports element insertion and removal at both ends with O(1) algorithmic complexity.
/// </summary>
/// <remarks>
/// The name "deque" is acronym for "<u>d</u>ouble <u>e</u>nded <u>que</u>ue" and is usually pronounced "deck".
/// </remarks>
public class Deque<T> : IList, IList<T>, IReadOnlyList<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Deque{T}"/> class that is empty
    /// and has the default initial capacity.
    /// </summary>
    public Deque()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Deque{T}"/> class that is empty
    /// and has the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the <see cref="Deque{T}"/> can contain.</param>
    public Deque(int capacity)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Deque{T}"/> class
    /// that contains elements copied from the specified collection
    /// and has sufficient capacity to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection to copy elements from.</param>
    public Deque(IEnumerable<T> collection)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the number of elements contained in the <see cref="Deque{T}"/>.
    /// </summary>
    public int Count => throw new NotImplementedException();

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is not a valid index in the <see cref="Deque{T}"/>.
    /// </exception>
    public T this[int index]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <summary>
    /// Removes all items from the <see cref="Deque{T}"/>.
    /// </summary>
    public void Clear()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Determines whether the <see cref="Deque{T}"/> contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the <see cref="Deque{T}"/>.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="item"/> is found in the <see cref="Deque{T}"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool Contains(T item)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Copies the elements of the <see cref="Deque{T}"/> to an <see cref="Array"/>,
    /// starting at a particular System.Array index.
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
        throw new NotImplementedException();
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
    /// <exception cref="NotImplementedException"></exception>
    public int IndexOf(T item)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Inserts an object at the beginning of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="item">The object to be inserted at the beginning of the <see cref="Deque{T}"/>.</param>
    public void PushToFront(T item)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Adds an object to the end of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <param name="item">The object to be added to the end of the <see cref="Deque{T}"/>.</param>
    public void PushToBack(T item)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes and returns the object at the beginning of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <returns>The object removed from the beginning of the <see cref="Deque{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="Deque{T}"/> is empty.</exception>
    public T PopFromFront()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes and returns the object at the end of the <see cref="Deque{T}"/>.
    /// </summary>
    /// <returns>The object removed from the end of the <see cref="Deque{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="Deque{T}"/> is empty.</exception>
    public T PopFromBack()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the object at the beginning of the <see cref="Deque{T}"/> 
    /// without removing it.
    /// </summary>
    /// <returns>The object at the beginning of the <see cref="Deque{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="Deque{T}"/> is empty.</exception>
    public T PeekFromFront()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the object at the end of the <see cref="Deque{T}"/> 
    /// without removing it.
    /// </summary>
    /// <returns>The object at the end of the <see cref="Deque{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="Deque{T}"/> is empty.</exception>
    public T PeekFromBack()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is greater than <see cref="Count"/>.</exception>
    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    #region Enumeration

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="Deque{T}"/>.
    /// </summary>
    /// <returns>A <see cref="Enumerator"/> for the <see cref="Deque{T}"/>.</returns>
    public Enumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Enumerates the elements of a <see cref="Deque{T}"/>.
    /// </summary>
    public struct Enumerator : IEnumerator<T>
    {
        /// <summary>
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        /// <value>
        /// The element in the <see cref="Deque{T}"/> at the current position of the enumerator.
        /// </value>
        public readonly T Current => throw new NotImplementedException();

        readonly object IEnumerator.Current => throw new NotImplementedException();

        /// <summary>
        /// Releases all resources used by the <see cref="Enumerator"/>.
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Compatibility

    #region ICollection<T>

    void ICollection<T>.Add(T item) => PushToBack(item);

    bool ICollection<T>.IsReadOnly => false;

    #endregion

    #region IList

    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = CollectionHelper.GetCompatibleValue<T>(value, nameof(value));
    }

    int IList.Add(object? value)
    {
        PushToBack(CollectionHelper.GetCompatibleValue<T>(value, nameof(value)));
        return Count - 1;
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
        Insert(index, CollectionHelper.GetCompatibleValue<T>(value, nameof(value)));

    void IList.Remove(object? value)
    {
        if (CollectionHelper.TryGetCompatibleValue<T>(value, out var compatibleValue))
            Remove(compatibleValue);
    }

    bool IList.IsReadOnly => false;

    bool IList.IsFixedSize => false;

    #endregion

    #region ICollection

    void ICollection.CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }

    bool ICollection.IsSynchronized => false;

    static object? m_SyncRoot;

    object ICollection.SyncRoot => LazyInitializerEx.EnsureInitialized(ref m_SyncRoot);

    #endregion

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}
