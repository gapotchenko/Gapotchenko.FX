namespace Gapotchenko.FX;

/// <summary>
/// Defines the functional notion of emptiness
/// representing an object that can be empty and statically provides an empty value.
/// </summary>
/// <remarks>
/// Types implementing this interface get automatic support of operations provided by <see cref="FX.Empty"/> type.
/// A static empty value is available since .NET 7.0.
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
