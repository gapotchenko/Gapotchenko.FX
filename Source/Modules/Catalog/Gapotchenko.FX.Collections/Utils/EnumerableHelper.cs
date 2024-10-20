// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Collections;
using System.Diagnostics;

namespace Gapotchenko.FX.Collections.Utils;

static class EnumerableHelper
{
    /// <summary>Calls Reset on an enumerator instance.</summary>
    /// <remarks>Enables Reset to be called without boxing on a struct enumerator that lacks a public Reset.</remarks>
    public static void Reset<T>(ref T enumerator) where T : IEnumerator => enumerator.Reset();

    /// <summary>Gets an enumerator singleton for an empty collection.</summary>
    public static IEnumerator<T> GetEmptyEnumerator<T>() => ((IEnumerable<T>)[]).GetEnumerator();

    public static T[] ToArray<T>(IEnumerable<T> source)
    {
        Debug.Assert(source != null);

        if (source is ICollection<T> collection)
        {
            int count = collection.Count;
            if (count == 0)
            {
                return [];
            }
            else
            {
                var result = new T[count];
                collection.CopyTo(result, 0);
                return result;
            }
        }
        else
        {
            return source.ToArray();
        }
    }
}
