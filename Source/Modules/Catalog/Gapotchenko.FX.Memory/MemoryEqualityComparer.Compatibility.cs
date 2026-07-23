#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable CS3008 // Identifier is not CLS-compliant

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Memory;

/// <summary>
/// Infrastructure.
/// Should not be used in user code.
/// </summary>
[Obsolete("Should not be used in user code.")]
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class _CompatibleMemoryEqualityComparer
{
    private protected _CompatibleMemoryEqualityComparer()
    {
    }

#if BINARY_COMPATIBILITY

    /// <summary>
    /// Determines whether two memory regions are equal.
    /// </summary>
    /// <typeparam name="T">The type of elements in the memory regions.</typeparam>
    /// <param name="x">The first memory region to compare.</param>
    /// <param name="y">The second memory region to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the two memory regions are equal; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [Obsolete("Use a matching method of MemoryEqualityComparer class instead.")] // 2026
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(-1000)]
    public static bool Equals<T>(in ReadOnlyMemory<T> x, in ReadOnlyMemory<T> y) => MemoryEqualityComparer.Equals(x, y);

    /// <summary>
    /// Returns a hash code for specified memory region.
    /// </summary>
    /// <typeparam name="T">The type of elements in the memory region.</typeparam>
    /// <param name="memory">The memory region to compute a hash code for.</param>
    /// <returns>A hash code representing the content of the specified memory region.</returns>
    [Obsolete("Use a matching method of MemoryEqualityComparer class instead.")] // 2026
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(-1000)]
    public static int GetHashCode<T>(in ReadOnlyMemory<T> memory) => MemoryEqualityComparer.GetHashCode(memory);

#endif
}
