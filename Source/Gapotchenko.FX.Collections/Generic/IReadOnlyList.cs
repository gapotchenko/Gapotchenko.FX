using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

#if !TF_READONLY_LIST

namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a read-only collection of elements that can be accessed by index.
    /// This is a polyfill provided by Gapotchenko FX.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the read-only list.
    /// This type parameter is covariant.
    /// That is, you can use either the type you specified or any type that is more derived.
    /// </typeparam>
    public interface IReadOnlyList<T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index in the read-only list.</returns>
        T this[int index] { get; }
    }
}

#else

[assembly: TypeForwardedTo(typeof(IReadOnlyList<>))]

#endif
