// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NET10_0_OR_GREATER
#define TFF_ENUMERABLE_SHUFFLE
#endif

namespace Gapotchenko.FX.Linq;

partial class EnumerablePolyfills
{
    /// <summary>Shuffles the order of the elements of a sequence.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to shuffle.</param>
    /// <returns>A sequence whose elements correspond to those of the input sequence in randomized order.</returns>
    /// <remarks>Randomization is performed using a non-cryptographically-secure random number generator.</remarks>
#if TFF_ENUMERABLE_SHUFFLE
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static IEnumerable<TSource> Shuffle<TSource>(
#if !TFF_ENUMERABLE_SHUFFLE
        this
#endif
        IEnumerable<TSource> source)
    {
#if TFF_ENUMERABLE_SHUFFLE
        // Redirect to BCL implementation.
        return Enumerable.Shuffle(source);
#else
        // Gapotchenko FX implementation.

        ArgumentNullException.ThrowIfNull(source);

        if (source.TryGetNonEnumeratedCount() == 0)
            return [];

        var array = source.ToArray();
        Random.Shared.Shuffle(array);
        return array;
#endif
    }
}
