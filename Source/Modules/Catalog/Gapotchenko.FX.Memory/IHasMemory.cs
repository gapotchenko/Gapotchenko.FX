namespace Gapotchenko.FX.Memory;

/// <summary>
/// Represents an abstract <see cref="Memory{T}"/> provider.
/// </summary>
/// <typeparam name="T">The type of items in the <see cref="Memory{T}"/>.</typeparam>
public interface IHasMemory<T>
{
    /// <summary>
    /// Gets the <see cref="Memory{T}"/> associated with this instance.
    /// </summary>
    /// <returns>The <see cref="Memory{T}"/> associated with this instance.</returns>
    Memory<T> GetMemory();

    /// <summary>
    /// Tries to get the <see cref="Memory{T}"/> associated with this instance.
    /// </summary>
    /// <param name="memory">The <see cref="Memory{T}"/> associated with this instance.</param>
    /// <returns><see langword="true"/> if the <see cref="Memory{T}"/> is available; otherwise, <see langword="false"/>.</returns>
    bool TryGetMemory(out Memory<T> memory);
}
