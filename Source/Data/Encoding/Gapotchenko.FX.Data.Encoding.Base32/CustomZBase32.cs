using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Customizable z-base-32 encoding.
    /// </summary>
    public class CustomZBase32 : GenericZBase32
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CustomZBase32"/> class with the specified alphabet and options.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        /// <param name="caseSensitive">Indicates whether alphabet is case sensitive.</param>
        /// <param name="synonyms">The optional synonyms of alphabet symbols.</param>
        public CustomZBase32(string alphabet, bool caseSensitive = false, IReadOnlyDictionary<char, string> synonyms = null) :
            base(
                new TextDataEncodingAlphabet(
                    alphabet ?? throw new ArgumentNullException(nameof(alphabet)),
                    caseSensitive,
                    synonyms))
        {
            m_CaseSensitive = caseSensitive;
        }

        /// <inheritdoc/>
        public override string Name => "Custom ZBase32";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly bool m_CaseSensitive;

        /// <inheritdoc/>
        public override bool IsCaseSensitive => m_CaseSensitive;
    }
}
