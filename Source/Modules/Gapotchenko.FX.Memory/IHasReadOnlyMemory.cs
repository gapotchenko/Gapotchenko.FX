namespace Gapotchenko.FX.Memory;

/// <summary>
/// Represents an abstract <see cref="ReadOnlyMemory{T}"/> provider.
/// </summary>
/// <typeparam name="T">The type of items in the <see cref="ReadOnlyMemory{T}"/>.</typeparam>
public interface IHasReadOnlyMemory<T>
{
    /// <summary>
    /// Gets the <see cref="ReadOnlyMemory{T}"/> associated with this instance.
    /// </summary>
    /// <returns>The <see cref="ReadOnlyMemory{T}"/> associated with this instance.</returns>
    ReadOnlyMemory<T> GetMemory();

    /// <summary>
    /// Tries to get the <see cref="ReadOnlyMemory{T}"/> associated with this instance.
    /// </summary>
    /// <param name="memory">The <see cref="ReadOnlyMemory{T}"/> associated with this instance.</param>
    /// <returns><see langword="true"/> if the <see cref="ReadOnlyMemory{T}"/> is available; otherwise, <see langword="false"/>.</returns>
    bool TryGetMemory(out ReadOnlyMemory<T> memory);
}
