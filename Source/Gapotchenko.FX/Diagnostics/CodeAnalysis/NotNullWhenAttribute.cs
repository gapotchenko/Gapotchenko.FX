#if !TFF_NULLABLE_ATTRIBUTES

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// <para>
/// Specifies that when a method returns <see cref="ReturnValue"/>,
/// the parameter will not be null even if the corresponding type allows it.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class NotNullWhenAttribute : Attribute
{
    /// <summary>
    /// Initializes the attribute with the specified return value condition.
    /// </summary>
    /// <param name="returnValue">
    /// The return value condition.
    /// If the method returns this value, the associated parameter will not be <c>null</c>.
    /// </param>
    public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

    /// <summary>
    /// Gets the return value condition.
    /// </summary>
    public bool ReturnValue { get; }
}

#else

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(NotNullWhenAttribute))]

#endif
