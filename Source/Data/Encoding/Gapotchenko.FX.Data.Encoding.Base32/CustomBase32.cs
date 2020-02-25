using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Customizable Base32 encoding.
    /// </summary>
    public class CustomBase32 : GenericBase32
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CustomBase32"/> class with the specified alphabet and options.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        /// <param name="caseSensitive">Indicates whether alphabet is case sensitive.</param>
        /// <param name="synonyms">The optional synonyms of alphabet symbols.</param>
        public CustomBase32(string alphabet, bool caseSensitive = false, IReadOnlyDictionary<char, string> synonyms = null) :
            base(
                new TextDataEncodingAlphabet(
                    alphabet ?? throw new ArgumentNullException(nameof(alphabet)),
                    caseSensitive,
                    synonyms))
        {
        }

        /// <inheritdoc/>
        public override string Name => "Custom Base32";
    }
}
