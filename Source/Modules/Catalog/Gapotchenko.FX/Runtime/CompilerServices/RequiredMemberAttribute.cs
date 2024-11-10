#if !TFF_REQUIREDMEMBERATTRIBUTE

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Runtime.CompilerServices;

/// <summary>
/// <para>
/// Specifies that a type has required members or that a member is required.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class RequiredMemberAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredMemberAttribute"/> class.
    /// </summary>
    public RequiredMemberAttribute()
    {
    }
}

#else

using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(RequiredMemberAttribute))]

#endif
