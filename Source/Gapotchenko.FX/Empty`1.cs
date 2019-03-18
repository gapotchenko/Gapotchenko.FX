using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Provides typed constructions related to a functional notion of emptiness.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public static class Empty<T>
    {
#if !TF_ARRAY_EMPTY
        static class ArrayFactory
        {
            public static readonly T[] Array = new T[0];
        }
#endif

        /// <summary>
        /// Gets an empty array instance.
        /// </summary>
        public static T[] Array =>
#if !TF_ARRAY_EMPTY
            ArrayFactory.Array;
#else
            System.Array.Empty<T>();
#endif
    }
}
