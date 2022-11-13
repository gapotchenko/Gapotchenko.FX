namespace Gapotchenko.FX;

/// <summary>
/// A special type that allows only one value and thus can hold no information.
/// </summary>
public sealed class Unit
{
    Unit()
    {
    }

    /// <summary>
    /// Gets the value of <see cref="Unit"/> type.
    /// </summary>
    public static Unit Value { get; } = new();
}
