using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

#if !TF_READONLY_COLLECTION

namespace System.Collections.Generic
{
    /// <summary>
    /// <para>
    /// Represents a strongly-typed, read-only collection of elements.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the elements.
    /// This type parameter is covariant.
    /// That is, you can use either the type you specified or any type that is more derived.
    /// </typeparam>
    public interface IReadOnlyCollection<out T> : IEnumerable<T>
    {
        /// <summary>
        /// Gets the count of elements in the collection.
        /// </summary>
        int Count { get; }
    }
}

#else

[assembly: TypeForwardedTo(typeof(IReadOnlyCollection<>))]

#endif
