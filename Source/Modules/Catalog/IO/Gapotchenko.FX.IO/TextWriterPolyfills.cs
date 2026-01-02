// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
#define TFF_TEXTWRITER_SPAN
#endif

using System.Buffers;

namespace Gapotchenko.FX.IO;

/// <summary>
/// Provides polyfill extension methods for <see cref="TextWriter"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class TextWriterPolyfills
{
    /// <summary>
    /// Writes the text representation of a character span to the text stream.
    /// </summary>
    /// <param name="writer">The text writer.</param>
    /// <param name="buffer">The char span value to write to the text stream.</param>
#if TFF_TEXTWRITER_SPAN
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static void Write(
#if !TFF_TEXTWRITER_SPAN
        this
#endif
        TextWriter writer, ReadOnlySpan<char> buffer)
    {
        ArgumentNullException.ThrowIfNull(writer);

#if TFF_TEXTWRITER_SPAN
        writer.Write(buffer);
#else
        var pool = ArrayPool<char>.Shared;
        char[] array = pool.Rent(buffer.Length);
        try
        {
            buffer.CopyTo(new Span<char>(array));
            writer.Write(array, 0, buffer.Length);
        }
        finally
        {
            pool.Return(array);
        }
#endif
    }

    /// <summary>
    /// Writes the text representation of a character span to the text stream,
    /// followed by a line terminator.
    /// </summary>
    /// <param name="writer">The text writer.</param>
    /// <param name="buffer">The char span value to write to the text stream.</param>
#if TFF_TEXTWRITER_SPAN
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static void WriteLine(
#if !TFF_TEXTWRITER_SPAN
        this
#endif
        TextWriter writer, ReadOnlySpan<char> buffer)
    {
        ArgumentNullException.ThrowIfNull(writer);

#if TFF_TEXTWRITER_SPAN
        writer.WriteLine(buffer);
#else
        var pool = ArrayPool<char>.Shared;
        char[] array = pool.Rent(buffer.Length);
        try
        {
            buffer.CopyTo(new Span<char>(array));
            writer.WriteLine(array, 0, buffer.Length);
        }
        finally
        {
            pool.Return(array);
        }
#endif
    }
}
