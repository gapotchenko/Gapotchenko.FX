using System;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Customizable Jochaim Henke's Base91 (basE91) encoding.
    /// </summary>
    public class CustomHenkeBase91 : GenericHenkeBase91
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CustomHenkeBase91"/> class with the specified case-sensitive alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        public CustomHenkeBase91(string alphabet) :
            this(new TextDataEncodingAlphabet(alphabet ?? throw new ArgumentNullException(nameof(alphabet)), true))
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CustomHenkeBase91"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        public CustomHenkeBase91(TextDataEncodingAlphabet alphabet) :
            base(alphabet)
        {
        }
    }
}
