namespace Gapotchenko.FX.Runtime.CompilerServices;

/// <summary>
/// Provides strongly typed access to reflection traits of <typeparamref name="T"/> type.
/// </summary>
/// <typeparam name="T">Type to provide the traits for.</typeparam>
public static class TypeTraits<T>
{
    static TypeTraits()
    {
        var type = typeof(T);

        IsValueType = type.IsValueType;
    }

    /// <summary>
    /// Gets a value indicating whether the type is a value type.
    /// </summary>
    public static readonly bool IsValueType;
}
