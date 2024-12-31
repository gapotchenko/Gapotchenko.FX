#if !NET9_0_OR_GREATER

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Runtime.CompilerServices;

/// <summary>
/// <para>
/// Specifies the priority of a member in overload resolution.
/// When unspecified, the default priority is <c>0</c>.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class OverloadResolutionPriorityAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OverloadResolutionPriorityAttribute"/> class.
    /// </summary>
    /// <param name="priority">
    /// The priority of the attributed member.
    /// Higher numbers are prioritized, lower numbers are deprioritized.
    /// <c>0</c> is the default if no attribute is present.
    /// </param>
    public OverloadResolutionPriorityAttribute(int priority)
    {
        Priority = priority;
    }

    /// <summary>
    /// The priority of the member.
    /// </summary>
    public int Priority { get; }
}

#else

using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(OverloadResolutionPriorityAttribute))]

#endif
