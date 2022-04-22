using System;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Customizable z-base-32 encoding.
    /// </summary>
    public class CustomZBase32 : GenericZBase32
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CustomZBase32"/> class with the specified case-insensitive alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        public CustomZBase32(string alphabet) :
            this(new TextDataEncodingAlphabet(alphabet ?? throw new ArgumentNullException(nameof(alphabet)), false))
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CustomZBase32"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        /// <param name="paddingChar">The padding character.</param>
        public CustomZBase32(TextDataEncodingAlphabet alphabet, char paddingChar = '=') :
            base(alphabet, paddingChar)
        {
        }
    }
}
