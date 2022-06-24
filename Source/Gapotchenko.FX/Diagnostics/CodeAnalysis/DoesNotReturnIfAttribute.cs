using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if !TFF_NULLABLE_ATTRIBUTES

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// <para>
/// Specifies that the method will not return if the associated <see cref="Boolean"/> parameter is passed the specified value.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class DoesNotReturnIfAttribute : Attribute
{
    /// <summary>
    /// Initializes the attribute with the specified parameter value.
    /// </summary>
    /// <param name="parameterValue">
    /// The condition parameter value.
    /// Code after the method will be considered unreachable by diagnostics
    /// if the argument to the associated parameter matches this value.
    /// </param>
    public DoesNotReturnIfAttribute(bool parameterValue) => ParameterValue = parameterValue;

    /// <summary>
    /// Gets the condition parameter value.
    /// </summary>
    public bool ParameterValue { get; }
}

#else

[assembly: TypeForwardedTo(typeof(DoesNotReturnIfAttribute))]

#endif
