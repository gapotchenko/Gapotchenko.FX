using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a customizable implementation of Base32 encoding.
    /// </summary>
    public class CustomBase64 : GenericBase64
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CustomBase64"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        /// <param name="synonyms">The optional synonyms of alphabet symbols.</param>
        protected CustomBase64(string alphabet, IReadOnlyDictionary<char, string> synonyms = null) :
            base(
                new TextDataEncodingAlphabet(
                    alphabet ?? throw new ArgumentNullException(nameof(alphabet)),
                    true,
                    synonyms))
        {
        }

        /// <inheritdoc/>
        public override string Name => "Custom Base64";
    }
}
