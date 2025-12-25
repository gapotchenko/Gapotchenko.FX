#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_IO_MATCHTYPE
#endif

#if !TFF_IO_MATCHTYPE

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.IO;

/// <summary>
/// <para>
/// Specifies the type of wildcard matching to use.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
public enum MatchType
{
    /// <summary>
    /// <para>
    /// Matches using '*' and '?' wildcards.
    /// </para>
    /// <para>
    /// <c>*</c> matches from zero to any amount of characters. 
    /// <c>?</c> matches exactly one character.
    /// <c>*.*</c> matches any name with a period in it (with <see cref="Win32"/>, this would match all items).
    /// </para>
    /// </summary>
    Simple,

    /// <summary>
    /// <para>
    /// Match using Win32 DOS style matching semantics.
    /// </para>
    /// <para>
    /// '*', '?', '&lt;', '&gt;', and '"' are all considered wildcards.
    /// Matches in a traditional DOS / Windows command prompt way.
    /// <c>*.*</c> matches all files.
    /// <c>?</c> matches collapse to periods.
    /// <c>file.??t</c> will match <c>file.t</c>, <c>file.at</c>, and <c>file.txt</c>.
    /// </para>
    /// </summary>
    Win32
}

#else

using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(MatchType))]

#endif
