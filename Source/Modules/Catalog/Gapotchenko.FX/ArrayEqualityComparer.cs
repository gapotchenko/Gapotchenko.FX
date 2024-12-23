﻿using Gapotchenko.FX.Properties;

namespace Gapotchenko.FX;

/// <summary>
/// Optimized and fast equality comparer for one-dimensional arrays.
/// </summary>
public static partial class ArrayEqualityComparer
{
    /// <summary>
    /// Determines whether the specified arrays are equal.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <param name="x">The first array to compare.</param>
    /// <param name="y">The second array to compare.</param>
    /// <returns><see langword="true"/> if the specified arrays are equal; otherwise, <see langword="false"/>.</returns>
    public static bool Equals<T>(T[]? x, T[]? y) => ArrayEqualityComparer<T>.Default.Equals(x, y);

    /// <summary>
    /// Returns a hash code for the specified array.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    /// <param name="array">The array.</param>
    /// <returns>A hash code for the specified array.</returns>
    public static int GetHashCode<T>(T[]? array) =>
        array is null ?
            0 :
            ArrayEqualityComparer<T>.Default.GetHashCode(array);

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

        if (arrayX == null && arrayY == null)
            throw new ArgumentException(Resources.Argument_InvalidComparison);

        if (arrayX == null || arrayY == null)
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
            if (!elementEqualityComparer.Equals(arrayX.GetValue(i), arrayY.GetValue(i)))
                return false;

        return true;

        static bool TypedEquals<T>(T[] x, object y) => Equals(x, y as T[]);
    }

    /// <summary>
    /// Retrieves an equality comparer for one-dimensional array with a specified comparer for elements.
    /// </summary>
    /// <typeparam name="T">The type of array elements.</typeparam>
    /// <param name="elementComparer">The equality comparer for array elements.</param>
    /// <returns>The equality comparer for one-dimensional array with elements of type <typeparamref name="T"/>.</returns>
    public static ArrayEqualityComparer<T> Create<T>(IEqualityComparer<T>? elementComparer) =>
        Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Byte when IsDefaultComparer(elementComparer) => (ArrayEqualityComparer<T>)(object)ByteRank1Comparer.Instance,
            _ => new DefaultArrayEqualityComparer<T>(elementComparer),
        };

    static bool IsDefaultComparer<T>(IEqualityComparer<T>? comparer) => Empty.Nullify(comparer) == null;
}
