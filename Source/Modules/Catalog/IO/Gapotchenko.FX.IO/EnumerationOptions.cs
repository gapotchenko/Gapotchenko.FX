#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_IO_ENUMERATIONOPTIONS
#endif

#if !TFF_IO_ENUMERATIONOPTIONS

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.IO;

/// <summary>
/// <para>
/// Provides file and directory enumeration options.
/// </para>
/// <para>
/// This is a polyfill provided by Gapotchenko.FX.
/// </para>
/// </summary>
public class EnumerationOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumerationOptions"/> class with the recommended default options.
    /// </summary>
    public EnumerationOptions()
    {
    }

    /// <summary>
    /// Gets or sets the attributes to skip.
    /// </summary>
    /// <value>
    /// The attributes to skip.
    /// The default is <c>FileAttributes.Hidden | FileAttributes.System</c>.
    /// </value>
    public FileAttributes AttributesToSkip { get; set; } = FileAttributes.Hidden | FileAttributes.System;

    /// <summary>
    /// Gets or sets the suggested buffer size, in bytes.
    /// </summary>
    /// <value>
    /// The buffer size. The default is 0 (no suggestion).
    /// </value>
    /// <remarks>
    /// <para>
    /// Not all platforms use user allocated buffers, and some require either fixed buffers or a buffer that has enough space to return a full result.
    /// One scenario where this option is useful is with remote share enumeration on Windows.
    /// Having a large buffer may result in better performance as more results can be batched over the wire (for example, over a network share).
    /// A "large" buffer, for example, would be 16K.
    /// Typical is 4K.
    /// </para>
    /// <para>
    /// The suggested buffer size will not be used if it has no meaning for the native APIs on the current platform
    /// or if it would be too small for getting at least a single result.
    /// </para>
    /// </remarks>
    public int BufferSize { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether to skip files or directories when access is denied
    /// (for example, <see cref="UnauthorizedAccessException"/> or <see cref="System.Security.SecurityException"/>).
    /// </summary>
    /// <value>
    /// <see langword="true"/> to skip inaccessible files or directories;
    /// otherwise, <see langword="false"/>.
    /// The default is <see langword="true"/>.
    /// </value>
    public bool IgnoreInaccessible { get; set; } = true;

    /// <summary>
    /// Gets or sets the case-matching behavior.
    /// </summary>
    /// <value>
    /// One of the enumeration values that indicates the case-matching behavior.
    /// The default is <see cref="MatchCasing.PlatformDefault"/>.
    /// </value>
    /// <remarks>
    /// For APIs that allow specifying a match expression, this property allows you to specify the case matching behavior.
    /// The default is to match platform defaults, which are gleaned from the case sensitivity of the temporary folder.
    /// </remarks>
    public MatchCasing MatchCasing { get; set; }

    /// <summary>
    /// Gets or sets the match type.
    /// </summary>
    /// <value>
    /// One of the enumeration values that indicates the match type.
    /// The default is <see cref="MatchType.Simple"/>.
    /// </value>
    /// <remarks>
    /// For APIs that allow specifying a match expression, this property allows you to specify how to interpret the match expression.
    /// The default is simple matching where '*' is always 0 or more characters and '?' is a single character.
    /// </remarks>
    public MatchType MatchType { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the maximum directory depth to recurse while enumerating,
    /// when <see cref="RecurseSubdirectories"/> is set to <see langword="true"/>.
    /// </summary>
    /// <value>
    /// A number that represents the maximum directory depth to recurse while enumerating.
    /// The default is <see cref="int.MaxValue"/>.
    /// </value>
    /// <remarks>
    /// If <see cref="MaxRecursionDepth"/> is set to a negative number, the default value <see cref="int.MaxValue"/> is used.
    /// If <see cref="MaxRecursionDepth"/> is set to zero, enumeration returns the contents of the initial directory.
    /// </remarks>
    public int MaxRecursionDepth { get; set; } = int.MaxValue;

    /// <summary>
    /// Gets or sets a value that indicates whether to recurse into subdirectories while enumerating.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to recurse into subdirectories;
    /// otherwise, <see langword="false"/>.
    /// The default is <see langword="false"/>.
    /// </value>
    public bool RecurseSubdirectories { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether to return the special directory entries "." and "..".
    /// </summary>
    /// <value>
    /// <see langword="true"/> to return the special directory entries "." and "..";
    /// otherwise, <see langword="false"/>.
    /// The default is <see langword="false"/>.
    /// </value>
    public bool ReturnSpecialDirectories { get; set; }
}

#else

using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(EnumerationOptions))]

#endif
