#if !TFF_MODULEINITIALIZERATTRIBUTE

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Runtime.CompilerServices;

/// <summary>
/// <para>
/// Used to indicate to the compiler that a method should be called in its containing module's initializer.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class ModuleInitializerAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleInitializerAttribute"/> class.
    /// </summary>
    public ModuleInitializerAttribute()
    {
    }
}

#else

using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(ModuleInitializerAttribute))]

#endif
