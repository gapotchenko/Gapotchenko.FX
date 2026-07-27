// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Properties;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX;

/// <summary>
/// Optimized and fast equality comparer for one-dimensional arrays.
/// </summary>
public static partial class ArrayEqualityComparer
{
    #region Equals

    /// <summary>
    /// Determines whether two arrays are equal
    /// by comparing the elements using <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the arrays.</typeparam>
    /// <param name="x">The first array to compare.</param>
    /// <param name="y">The second array to compare.</param>
    /// <param name="comparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the two arrays are equal;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Equals<T>(T[]? x, T[]? y, IEqualityComparer<T>? comparer = null)
    {
        if (comparer is null)
        {
            // This path provides accelerated comparison to cover the cases
            // that would be normally covered by "Equals<T>(T[]? x, T[]? y) where T : IEquatable<T>?"
            // overload.
            return ArrayEqualityComparer<T>.Default.Equals(x, y);
        }
        else
        {
            return EqualsCore(x, y, comparer);
        }
    }

    /// <summary>
    /// Determines whether two arrays are equal.
    /// </summary>
    /// <typeparam name="T">The type of elements in the arrays.</typeparam>
    /// <param name="x">The first array to compare.</param>
    /// <param name="y">The second array to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the two arrays are equal; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
#if BINARY_COMPATIBILITY // 2026
    // Ideally, this method should have been introduced with T : IEquatable<T>? constraint from the get go.
    // It cannot be changed now as it would break the binary API which was in use during 2019-2026.
    // Deprioritizing the method in favor of a more common overload for now
    // so that the API might have a chance to be revisited in the future.
    [OverloadResolutionPriority(-10)]
#endif
    public static bool Equals<T>(T[]? x, T[]? y)
#if !BINARY_COMPATIBILITY // 2026
        where T : IEquatable<T>?
#endif
    {
        return ArrayEqualityComparer<T>.Default.Equals(x, y);
    }

    /// <summary>
    /// Determines whether the specified arrays are equal.
    /// </summary>
    /// <remarks>This method overshadows <see cref="object.Equals(object, object)"/> to avoid a comparison by reference pitfall.</remarks>
    /// <param name="x">The first array to compare.</param>
    /// <param name="y">The second array to compare.</param>
    /// <returns><see langword="true"/> if the specified arrays are equal; otherwise, <see langword="false"/>.</returns>
    public static new bool Equals(object? x, object? y)
    {
        if (x is null && y is null)
            return true;

        var arrayX = x as Array;
        var arrayY = y as Array;

        if (arrayX is null && arrayY is null)
            throw new ArgumentException(Resources.Argument_InvalidComparison);

        if (arrayX is null || arrayY is null)
            return false;

        if (arrayX.Rank != 1 && arrayY.Rank != 1)
            throw new ArgumentException(Resources.Argument_InvalidComparison);

        if (arrayX == arrayY)
            return true;

        if (arrayX.Rank != arrayY.Rank)
            return false;

        int n = arrayX.Length;
        if (arrayY.Length != n)
            return false;

        var elementType = arrayX.GetType().GetElementType();
        if (arrayY.GetType().GetElementType() != elementType)
            return false;

        if (n == 0)
            return true;

        // Signed element types are covered by implicit cast to an array of matched unsigned types.
        //
        // E.g. the following is possible:
        //     short[] x = new { 1, 2, 3 };
        //     var arrayX = (Array)x;
        //     var tx = (ushort[])arrayX; // implicit cast

        switch (arrayX)
        {
            case bool[] tx:
                return TypedEquals(tx, arrayY);
            case char[] tx:
                return TypedEquals(tx, arrayY);
            case byte[] tx:
                return TypedEquals(tx, arrayY);
            case ushort[] tx:
                return TypedEquals(tx, arrayY);
            case uint[] tx:
                return TypedEquals(tx, arrayY);
            case ulong[] tx:
                return TypedEquals(tx, arrayY);
            case float[] tx:
                return TypedEquals(tx, arrayY);
            case double[] tx:
                return TypedEquals(tx, arrayY);
            case decimal[] tx:
                return TypedEquals(tx, arrayY);
            case DateTime[] tx:
                return TypedEquals(tx, arrayY);
            case string[] tx:
                return TypedEquals(tx, arrayY);
        }

        var elementEqualityComparer = EqualityComparer<object>.Default;
        for (int i = 0; i != n; ++i)
        {
            if (!elementEqualityComparer.Equals(arrayX.GetValue(i), arrayY.GetValue(i)))
                return false;
        }

        return true;

        static bool TypedEquals<T>(T[] x, object y) => EqualsCore(x, y as T[]);
    }

    static bool EqualsCore<T>(T[]? x, T[]? y, IEqualityComparer<T>? comparer = null)
    {
        if (x == y)
            return true;
        if (x is null || y is null)
            return false;

#if NET
        return x.SequenceEqual(y, comparer);
#else
        if (x.Length != y.Length)
            return false;

        comparer ??= EqualityComparer<T>.Default;
        for (int i = 0; i < x.Length; i++)
        {
            if (!comparer.Equals(x[i], y[i]))
                return false;
        }

        return true;
#endif
    }

    static bool EqualsCore<T>(T[]? x, T[]? y) where T : IEquatable<T>?
    {
        if (x == y)
            return true;
        if (x is null || y is null)
            return false;

        return x.SequenceEqual(y);
    }

    #endregion

    /// <summary>
    /// Returns a hash code for the specified array.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <param name="array">The array.</param>
    /// <returns>A hash code for the specified array.</returns>
    public static int GetHashCode<T>(T[]? array)
    {
        return array is null ?
            0 :
            ArrayEqualityComparer<T>.Default.GetHashCode(array);
    }


    /// <summary>
    /// Retrieves an equality comparer for one-dimensional array with a specified comparer for elements.
    /// </summary>
    /// <typeparam name="T">The type of array elements.</typeparam>
    /// <param name="elementComparer">The equality comparer for array elements.</param>
    /// <returns>The equality comparer for one-dimensional array with elements of type <typeparamref name="T"/>.</returns>
    public static ArrayEqualityComparer<T> Create<T>(IEqualityComparer<T>? elementComparer)
    {
        elementComparer = Empty.Nullify(elementComparer);
        if (elementComparer is null)
        {
            return
                Type.GetTypeCode(typeof(T)) switch
                {
                    TypeCode.Byte => (ArrayEqualityComparer<T>)(object)ByteArrayComparer.Instance,
                    TypeCode.SByte => (ArrayEqualityComparer<T>)(object)BitwiseArrayComparer<sbyte>.Instance,
                    TypeCode.Int16 => (ArrayEqualityComparer<T>)(object)BitwiseArrayComparer<short>.Instance,
                    TypeCode.UInt16 => (ArrayEqualityComparer<T>)(object)BitwiseArrayComparer<ushort>.Instance,
                    TypeCode.Int32 => (ArrayEqualityComparer<T>)(object)BitwiseArrayComparer<int>.Instance,
                    TypeCode.UInt32 => (ArrayEqualityComparer<T>)(object)BitwiseArrayComparer<uint>.Instance,
                    TypeCode.Int64 => (ArrayEqualityComparer<T>)(object)BitwiseArrayComparer<long>.Instance,
                    TypeCode.UInt64 => (ArrayEqualityComparer<T>)(object)BitwiseArrayComparer<ulong>.Instance,
                    TypeCode.Boolean => (ArrayEqualityComparer<T>)(object)BitwiseArrayComparer<bool>.Instance,
                    TypeCode.Char => (ArrayEqualityComparer<T>)(object)BitwiseArrayComparer<char>.Instance,
                    _ => DefaultArrayComparer<T>.Instance
                };
        }
        else
        {
            return new CustomArrayComparer<T>(elementComparer);
        }
    }
}
