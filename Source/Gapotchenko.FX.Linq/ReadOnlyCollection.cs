using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Linq
{
#if !TF_READONLY_COLLECTION
    /// <summary>
    /// Generic read-only collection.
    /// A polyfill for .NET 4.0.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    sealed class ReadOnlyCollection<T> : System.Collections.ObjectModel.ReadOnlyCollection<T>, IReadOnlyList<T>
    {
        public ReadOnlyCollection(IList<T> list) :
            base(list)
        {
        }
    }
#endif
}
