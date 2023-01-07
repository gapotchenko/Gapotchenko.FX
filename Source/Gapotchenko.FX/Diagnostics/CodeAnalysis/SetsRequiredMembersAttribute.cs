#if !TFF_REQUIREDMEMBERATTRIBUTE

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// <para>
/// Specifies that this constructor sets all required members for the current type,
/// and callers do not need to set any required members themselves.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
public sealed class SetsRequiredMembersAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SetsRequiredMembersAttribute"/> class.
    /// </summary>
    public SetsRequiredMembersAttribute()
    {
    }
}

#else

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(SetsRequiredMembersAttribute))]

#endif
