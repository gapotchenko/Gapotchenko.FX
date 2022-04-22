using System.Collections.Generic;

namespace Gapotchenko.FX.Data.Encoding
{
    static class Base64LinguaFranca
    {
        public const string Symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        /// <summary>
        /// Gets a common alphabet for <see cref="Base64"/> and <see cref="Base64Url"/> encoding variants.
        /// </summary>
        public static TextDataEncodingAlphabet Alphabet { get; } =
            new TextDataEncodingAlphabet(
                Symbols,
                true,
                new Dictionary<char, string>
                {
                    ['+'] = "-",
                    ['/'] = "_"
                });
    }
}
