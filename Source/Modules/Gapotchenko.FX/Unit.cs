namespace Gapotchenko.FX;

/// <summary>
/// A special type that allows only one value and thus can hold no information.
/// </summary>
public sealed class Unit : IEmptiable<Unit>
{
    Unit()
    {
    }

    /// <summary>
    /// Gets the value of <see cref="Unit"/> type.
    /// </summary>
    public static Unit Value { get; } = new();

    bool IEmptiable.IsEmpty => true;

#if NET7_0_OR_GREATER
    static Unit IEmptiable<Unit>.Empty => Value;
#endif
}
