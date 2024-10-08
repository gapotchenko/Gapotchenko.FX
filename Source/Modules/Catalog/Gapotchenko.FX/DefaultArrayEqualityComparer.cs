namespace Gapotchenko.FX;

class DefaultArrayEqualityComparer<T> : ArrayEqualityComparer<T>
{
    internal DefaultArrayEqualityComparer(IEqualityComparer<T>? elementComparer)
    {
        _ElementComparer = elementComparer ?? EqualityComparer<T>.Default;
    }

    readonly IEqualityComparer<T> _ElementComparer;

    /// <summary>
    /// Determines whether the specified arrays are equal.
    /// </summary>
    /// <param name="x">The first array to compare.</param>
    /// <param name="y">The second array to compare.</param>
    /// <returns><see langword="true"/> if the specified arrays are equal; otherwise, <see langword="false"/>.</returns>
    public override bool Equals(T[]? x, T[]? y)
    {
        if (x == y)
            return true;

        if (x is null || y is null)
            return false;

        if (x.Length != y.Length)
            return false;

        for (int i = 0; i < x.Length; i++)
            if (!_ElementComparer.Equals(x[i], y[i]))
                return false;

        return true;
    }

    /// <summary>
    /// Returns a hash code for the specified array.
    /// </summary>
    /// <param name="obj">The array for which a hash code is to be returned.</param>
    /// <returns>A hash code for the specified array.</returns>
    public override int GetHashCode(T[] obj)
    {
        if (obj is null)
            return 0;

        var elementComparer = _ElementComparer;

        // FNV-1a
        uint hash = 2166136261;
        foreach (var i in obj)
            hash = (hash ^ (uint)InternalGetHashCode(i, elementComparer)) * 16777619;
        return (int)hash;
    }

    static int InternalGetHashCode(T value, IEqualityComparer<T> comparer)
    {
        if (value is null)
            return 0;
        return comparer.GetHashCode(value);
    }

    /// <summary>
    /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="ArrayEqualityComparer{T}"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current <see cref="ArrayEqualityComparer{T}"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the specified <see cref="Object"/> is equal to the current <see cref="ArrayEqualityComparer{T}"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals(object? obj) => obj is ArrayEqualityComparer<T>;

    /// <summary>
    /// Returns a hash code for <see cref="ArrayEqualityComparer{T}"/>.
    /// </summary>
    /// <returns>A hash code for <see cref="ArrayEqualityComparer{T}"/>.</returns>
    public override int GetHashCode() => GetType().Name.GetHashCode();
}
