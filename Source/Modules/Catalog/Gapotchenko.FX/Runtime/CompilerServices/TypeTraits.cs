#pragma warning disable CA1000 // Do not declare static members on generic types

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

        bool isValueType = type.IsValueType;
        IsValueType = isValueType;

        IsBitwiseEquatable =
            isValueType &&
            (type.IsEnum ||
            type.IsPrimitive && Type.GetTypeCode(type) is not (TypeCode.Single or TypeCode.Double) ||
            type == typeof(Guid));
    }

    /// <summary>
    /// Gets a value indicating whether <typeparamref name="T"/> type is a value type.
    /// </summary>
    public static readonly bool IsValueType;

    /// <summary>
    /// Gets a value indicating whether <typeparamref name="T"/> type is bitwise equatable.
    /// </summary>
    public static bool IsBitwiseEquatable { get; }
}
