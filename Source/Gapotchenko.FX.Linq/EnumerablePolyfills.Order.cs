#if NET7_0_OR_GREATER
#define TFF_ENUMERABLE_ORDER
#endif

namespace Gapotchenko.FX.Linq;

partial class EnumerablePolyfills
{
    /// <summary>
    /// Sorts the elements of a sequence in ascending order.
    /// </summary>
    /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// This method is implemented by using deferred execution. The immediate return value is an object
    /// that stores all the information that is required to perform the action.
    /// The query represented by this method is not executed until the object is enumerated by calling
    /// its <see cref="IEnumerable{T}.GetEnumerator"/> method.
    /// This method compares elements by using the default comparer <see cref="Comparer{T}.Default"/>.
    /// </remarks>
#if TFF_ENUMERABLE_ORDER
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static IOrderedEnumerable<T> Order<T>(
#if !TFF_ENUMERABLE_ORDER
        this
#endif
        IEnumerable<T> source) =>
#if TFF_ENUMERABLE_ORDER
        // Redirect to BCL implementation.
        Enumerable.Order(source);
#else
        // FX implementation.
        Enumerable.OrderBy(source, Fn.Identity);
#endif

    /// <summary>
    /// Sorts the elements of a sequence in ascending order.
    /// </summary>
    /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// This method is implemented by using deferred execution. The immediate return value is an object
    /// that stores all the information that is required to perform the action.
    /// The query represented by this method is not executed until the object is enumerated by calling
    /// its <see cref="IEnumerable{T}.GetEnumerator"/> method.
    /// If comparer is <see langword="null"/>, the default comparer <see cref="Comparer{T}.Default"/> is used to compare elements.
    /// </remarks>
#if TFF_ENUMERABLE_ORDER
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static IOrderedEnumerable<T> Order<T>(
#if !TFF_ENUMERABLE_ORDER
        this
#endif
        IEnumerable<T> source,
        IComparer<T>? comparer) =>
#if TFF_ENUMERABLE_ORDER
        // Redirect to BCL implementation.
        Enumerable.Order(source, comparer);
#else
        // FX implementation.
        Enumerable.OrderBy(source, Fn.Identity, comparer);
#endif

    /// <summary>
    /// Sorts the elements of a sequence in descending order.
    /// </summary>
    /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// This method is implemented by using deferred execution. The immediate return value is an object
    /// that stores all the information that is required to perform the action.
    /// The query represented by this method is not executed until the object is enumerated by calling
    /// its <see cref="IEnumerable{T}.GetEnumerator"/> method.
    /// This method compares elements by using the default comparer <see cref="Comparer{T}.Default"/>.
    /// </remarks>
#if TFF_ENUMERABLE_ORDER
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static IOrderedEnumerable<T> OrderDescending<T>(
#if !TFF_ENUMERABLE_ORDER
        this
#endif
        IEnumerable<T> source) =>
#if TFF_ENUMERABLE_ORDER
        // Redirect to BCL implementation.
        Enumerable.OrderDescending(source);
#else
        // FX implementation.
        Enumerable.OrderByDescending(source, Fn.Identity);
#endif

    /// <summary>
    /// Sorts the elements of a sequence in descending order.
    /// </summary>
    /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="IOrderedEnumerable{TElement}"/> whose elements are sorted.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// This method is implemented by using deferred execution. The immediate return value is an object
    /// that stores all the information that is required to perform the action.
    /// The query represented by this method is not executed until the object is enumerated by calling
    /// its <see cref="IEnumerable{T}.GetEnumerator"/> method.
    ///
    /// If comparer is <see langword="null"/>, the default comparer <see cref="Comparer{T}.Default"/> is used to compare elements.
    /// </remarks>
#if TFF_ENUMERABLE_ORDER
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static IOrderedEnumerable<T> OrderDescending<T>(
#if !TFF_ENUMERABLE_ORDER
        this
#endif
        IEnumerable<T> source,
        IComparer<T>? comparer) =>
#if TFF_ENUMERABLE_ORDER
        // Redirect to BCL implementation.
        Enumerable.OrderDescending(source, comparer);
#else
        // FX implementation.
        Enumerable.OrderByDescending(source, Fn.Identity, comparer);
#endif
}
