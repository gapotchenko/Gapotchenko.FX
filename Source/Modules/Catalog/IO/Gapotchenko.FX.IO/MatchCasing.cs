#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_IO_MATCHCASING
#endif

#if !TFF_IO_MATCHCASING

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.IO;

/// <summary>
/// <para>
/// Specifies the type of character casing to match.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
public enum MatchCasing
{
    /// <summary>
    /// Matches using the default casing for the given platform.
    /// </summary>
    PlatformDefault,

    /// <summary>
    /// Matches respecting character casing.
    /// </summary>
    CaseSensitive,

    /// <summary>
    /// Matches ignoring character casing.
    /// </summary>
    CaseInsensitive
}

#else

using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(MatchCasing))]

#endif
