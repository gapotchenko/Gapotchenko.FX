using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if !TFF_NULLABLE_ATTRIBUTES

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// <para>
/// Specifies that <c>null</c> is disallowed as an input even if the corresponding type allows it.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
public sealed class DisallowNullAttribute : Attribute
{
}

#else

[assembly: TypeForwardedTo(typeof(DisallowNullAttribute))]

#endif
