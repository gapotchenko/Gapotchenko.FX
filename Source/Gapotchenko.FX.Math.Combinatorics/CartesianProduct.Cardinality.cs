namespace Gapotchenko.FX.Math.Combinatorics;

partial class CartesianProduct
{
    /// <summary>
    /// Returns a Cartesian product cardinality for the specified sequence lengths.
    /// </summary>
    /// <param name="lengths">The sequence lengths.</param>
    /// <returns>The Cartesian product cardinality.</returns>
    public static int Cardinality(IEnumerable<int> lengths)
    {
        if (lengths == null)
            throw new ArgumentNullException(nameof(lengths));

        bool hasFactor = false;
        int cardinality = 1;

        foreach (var length in lengths)
        {
            if (length == 0)
                return 0;

            cardinality *= length;
            hasFactor = true;
        }

        if (!hasFactor)
            return 0;

        return cardinality;
    }

    /// <summary>
    /// Returns a Cartesian product cardinality for the specified sequence lengths.
    /// </summary>
    /// <param name="lengths">The sequence lengths.</param>
    /// <returns>The Cartesian product cardinality.</returns>
    public static int Cardinality(params int[] lengths) => Cardinality((IEnumerable<int>)lengths);

    /// <summary>
    /// Returns a Cartesian product cardinality for the specified sequence lengths.
    /// </summary>
    /// <param name="lengths">The sequence lengths.</param>
    /// <returns>The Cartesian product cardinality.</returns>
    public static long Cardinality(IEnumerable<long> lengths)
    {
        if (lengths == null)
            throw new ArgumentNullException(nameof(lengths));

        bool hasFactor = false;
        long cardinality = 1;

        foreach (var length in lengths)
        {
            if (length == 0)
                return 0;

            cardinality *= length;
            hasFactor = true;
        }

        if (!hasFactor)
            return 0;

        return cardinality;
    }

    /// <summary>
    /// Returns a Cartesian product cardinality for the specified sequence lengths.
    /// </summary>
    /// <param name="lengths">The sequence lengths.</param>
    /// <returns>The Cartesian product cardinality.</returns>
    public static long Cardinality(params long[] lengths) => Cardinality((IEnumerable<long>)lengths);
}
