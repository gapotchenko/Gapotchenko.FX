namespace Gapotchenko.FX.Console;

/// <summary>
/// The tristate atomic data type for a thread-safe representation of a nullable Boolean value in accordance with .NET memory model.
/// </summary>
enum AtomicNullableBool : byte
{
    Null,
    False,
    True
}
