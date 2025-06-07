// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

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
}
