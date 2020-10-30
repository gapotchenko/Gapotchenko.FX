using System;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Customizable Base58 encoding.
    /// </summary>
    public class CustomBase58 : GenericBase58
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CustomBase58"/> class with the specified case-sensitive alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        public CustomBase58(string alphabet) :
            this(new TextDataEncodingAlphabet(alphabet ?? throw new ArgumentNullException(nameof(alphabet))))
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CustomBase58"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        public CustomBase58(TextDataEncodingAlphabet alphabet) :
            base(alphabet)
        {
        }
    }
}
