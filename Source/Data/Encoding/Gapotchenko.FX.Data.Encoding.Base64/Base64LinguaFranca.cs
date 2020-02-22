using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    static class Base64LinguaFranca
    {
        /// <summary>
        /// Gets a common alphabet for <see cref="Base64"/> and <see cref="Base64Url"/> variations.
        /// </summary>
        public static TextDataEncodingAlphabet Alphabet { get; } =
            new TextDataEncodingAlphabet(
                Base64.Symbols,
                true,
                new Dictionary<char, string>
                {
                    ['+'] = "-",
                    ['/'] = "_"
                });
    }
}
