using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if !TFF_NULLABLE_ATTRIBUTES

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// <para>
/// Specifies that an output may be <c>null</c> even if the corresponding type disallows it.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
public sealed class MaybeNullAttribute : Attribute
{
}

#else

[assembly: TypeForwardedTo(typeof(MaybeNullAttribute))]

#endif
