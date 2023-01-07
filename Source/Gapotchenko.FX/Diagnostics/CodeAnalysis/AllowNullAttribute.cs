#if !TFF_NULLABLE_ATTRIBUTES

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// <para>
/// Specifies that <c>null</c> is allowed as an input even if the corresponding type disallows it.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
public sealed class AllowNullAttribute : Attribute
{
}

#else

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(AllowNullAttribute))]

#endif
