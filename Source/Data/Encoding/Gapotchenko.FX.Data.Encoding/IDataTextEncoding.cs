using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Defines interface of a binary-to-text encoding.
    /// </summary>
    public interface IDataTextEncoding : IDataEncoding
    {
        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation of the contents of <paramref name="data"/>.</returns>
        string GetString(ReadOnlySpan<byte> data);

        /// <summary>
        /// Decodes the specified string to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        byte[] GetBytes(string s);
    }
}
