#if !TFF_ISEXTERNALINIT

using System.ComponentModel;

namespace System.Runtime.CompilerServices;

/// <summary>
/// <para>
/// Reserved to be used by the compiler for tracking metadata.
/// This class should not be used by developers in source code.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class IsExternalInit
{
}

#else

using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(IsExternalInit))]

#endif
