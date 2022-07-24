namespace Gapotchenko.FX;

/// <summary>
/// A special type that allows only one value (<c>null</c>), and thus can hold no information.
/// </summary>
public sealed class Unit
{
    Unit() => throw new InvalidOperationException("Unit type cannot be instantiated.");
}
