namespace Gapotchenko.FX.Math;

/// <summary>
/// Provides the interface for continuous interval abstraction.
/// </summary>
/// <typeparam name="T">The type of interval value.</typeparam>
public interface IInterval<T> : IIntervalOperations<T>
{
#if false && NET7_0_OR_GREATER
    /// <summary>
    /// Determines whether the specified intervals are equal.
    /// </summary>
    /// <param name="x">The first interval.</param>
    /// <param name="y">The second interval.</param>
    /// <returns><see langword="true"/> if the specified intervals are equal; otherwise, <see langword="false"/>.</returns>
    static abstract bool operator ==(IInterval<T>? x, IInterval<T>? y);

    /// <summary>
    /// Determines whether the specified intervals are not equal.
    /// </summary>
    /// <param name="x">The first interval.</param>
    /// <param name="y">The second interval.</param>
    /// <returns><see langword="true"/> if the specified intervals are not equal; otherwise, <see langword="false"/>.</returns>
    static abstract bool operator !=(IInterval<T>? x, IInterval<T>? y);
#endif
}
