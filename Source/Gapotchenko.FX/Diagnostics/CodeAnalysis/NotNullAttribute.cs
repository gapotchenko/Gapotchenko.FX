using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if !TFF_NULLABLE_ATTRIBUTES

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// <para>
    /// Specifies that an output will not be <c>null</c> even if the corresponding type allows it.
    /// Specifies that an input argument was not <c>null</c> when the call returns.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    public sealed class NotNullAttribute : Attribute
    {
    }
}

#else

[assembly: TypeForwardedTo(typeof(NotNullAttribute))]

#endif
