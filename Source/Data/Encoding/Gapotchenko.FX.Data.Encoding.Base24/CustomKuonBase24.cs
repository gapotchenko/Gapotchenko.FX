using System;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Customizable Kuon Base24 encoding.
    /// </summary>
    public class CustomKuonBase24 : GenericKuonBase24
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CustomKuonBase24"/> class with the specified case-insensitive alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        public CustomKuonBase24(string alphabet) :
            this(new TextDataEncodingAlphabet(alphabet ?? throw new ArgumentNullException(nameof(alphabet)), false))
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CustomKuonBase24"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        public CustomKuonBase24(TextDataEncodingAlphabet alphabet) :
            base(alphabet)
        {
        }

        /// <inheritdoc/>
        public override string Name => "Custom Kuon Base24";
    }
}
