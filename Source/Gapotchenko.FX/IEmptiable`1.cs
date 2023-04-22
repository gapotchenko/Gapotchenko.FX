namespace Gapotchenko.FX;

/// <summary>
/// Represents an object that has the functional notion of emptiness, can be empty,
/// and statically provides an empty value (available since .NET 7.0).
/// </summary>
/// <remarks>
/// Types implementing this interface get automatic support of operations
/// provided by <see cref="FX.Empty"/> type.
/// </remarks>
/// <typeparam name="T">The type of the emptiable object.</typeparam>
public interface IEmptiable<out T> : IEmptiable
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Gets the empty value of <typeparamref name="T"/>.
    /// </summary>
    static abstract T Empty { get; }
#endif
}
