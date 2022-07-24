using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if !TFF_NULLABLE_ATTRIBUTES

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// <para>
/// Applied to a method that will never return under any circumstance.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class DoesNotReturnAttribute : Attribute
{
}

#else

[assembly: TypeForwardedTo(typeof(DoesNotReturnAttribute))]

#endif
