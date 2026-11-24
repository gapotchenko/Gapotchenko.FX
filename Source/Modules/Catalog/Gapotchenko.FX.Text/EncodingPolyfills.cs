// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_TEXT_ENCODING_SPAN
#endif

using Gapotchenko.FX.Runtime.InteropServices;
using System.Buffers;
using System.Text;

namespace Gapotchenko.FX.Text;

/// <summary>
/// Provides polyfill members for <see cref="Encoding"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class EncodingPolyfills
{
    /// <inheritdoc cref="EncodingPolyfills"/>
    extension(Encoding encoding)
    {
        /// <summary>
        /// Gets a value indicating whether the current encoding represents a Unicode character encoding scheme.
        /// </summary>
        public bool IsUnicodeScheme
        {
            get
            {
                ArgumentNullException.ThrowIfNull(encoding);

                return
                    encoding.CodePage switch
                    {
                        1200 or // UTF-16LE
                        1201 or // UTF-16BE
                        12000 or // UTF-32LE
                        12001 or // UTF-32BE
                        65000 or // UTF-7
                        65001 => true, // UTF-8
                        _ => false,
                    };
            }
        }
    }

    /// <summary>
    /// Decodes all the bytes in the specified byte span into a string.
    /// </summary>
    /// <param name="encoding">The encoding.</param>
    /// <param name="bytes">A read-only byte span to decode to a string.</param>
    /// <returns>
    /// A string that contains the decoded bytes from the provided read-only span.
    /// </returns>
    public static string GetString(
#if !TFF_TEXT_ENCODING_SPAN
        this
#endif
        Encoding encoding,
        ReadOnlySpan<byte> bytes)
    {
        ArgumentNullException.ThrowIfNull(encoding);

#if TFF_TEXT_ENCODING_SPAN
        return encoding.GetString(bytes);
#else
        if (bytes.IsEmpty)
            return encoding.GetString([]);

        if (CodeSafetyStrategy.UnsafeCodeAllowed)
        {
            unsafe
            {
                fixed (byte* ptr = bytes)
                    return encoding.GetString(ptr, bytes.Length);
            }
        }
        else
        {
            var pool = ArrayPool<byte>.Shared;
            int length = bytes.Length;
            byte[] buffer = pool.Rent(length);
            try
            {
                bytes.CopyTo(buffer);
                return encoding.GetString(buffer, 0, length);
            }
            finally
            {
                pool.Return(buffer);
            }
        }
#endif
    }
}
