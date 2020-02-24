using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Customizable Base16 encoding.
    /// </summary>
    public class CustomBase16 : GenericBase16
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CustomBase16"/> class with the specified alphabet and options.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        /// <param name="caseSensitive">Indicates whether alphabet is case sensitive.</param>
        /// <param name="synonyms">The optional synonyms of alphabet symbols.</param>
        protected CustomBase16(string alphabet, bool caseSensitive = false, IReadOnlyDictionary<char, string> synonyms = null) :
            base(
                new TextDataEncodingAlphabet(
                    alphabet ?? throw new ArgumentNullException(nameof(alphabet)),
                    caseSensitive,
                    synonyms))
        {
            m_CaseSensitive = caseSensitive;
        }

        /// <inheritdoc/>
        public override string Name => "Custom Base16";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly bool m_CaseSensitive;

        /// <inheritdoc/>
        public override bool IsCaseSensitive => m_CaseSensitive;
    }
}
