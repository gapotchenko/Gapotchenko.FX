using System;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// A unifying interface for all possible Crockford Base 32 encoding implementations.
    /// </summary>
    public interface ICrockfordBase32 : IBase32
    {
        /// <summary>
        /// Encodes an <see cref="Int32"/> value to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="Int32"/> value.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(int value);

        /// <summary>
        /// Encodes an <see cref="Int32"/> value to its equivalent string representation with specified options.
        /// </summary>
        /// <param name="value">The <see cref="Int32"/> value.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(int value, DataEncodingOptions options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int32"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An <see cref="Int32"/> value that is equivalent to <paramref name="s"/>.</returns>
        int GetInt32(ReadOnlySpan<char> s);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int32"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An <see cref="Int32"/> value that is equivalent to <paramref name="s"/>.</returns>
        int GetInt32(ReadOnlySpan<char> s, DataEncodingOptions options);
    }
}
