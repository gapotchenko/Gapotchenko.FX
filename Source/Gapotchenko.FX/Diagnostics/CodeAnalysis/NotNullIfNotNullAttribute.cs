using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if !TFF_NULLABLE_ATTRIBUTES

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// <para>
/// Specifies that the output will be non-null if the named parameter is non-null.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
public sealed class NotNullIfNotNullAttribute : Attribute
{
    /// <summary>
    /// Initializes the attribute with the associated parameter name.
    /// </summary>
    /// <param name="parameterName">
    /// The associated parameter name.
    /// The output will be non-null if the argument to the parameter specified is non-null.
    /// </param>
    public NotNullIfNotNullAttribute(string parameterName) => ParameterName = parameterName;

    /// <summary>
    /// Gets the associated parameter name.
    /// </summary>
    public string ParameterName { get; }
}

#else

[assembly: TypeForwardedTo(typeof(NotNullIfNotNullAttribute))]

#endif
