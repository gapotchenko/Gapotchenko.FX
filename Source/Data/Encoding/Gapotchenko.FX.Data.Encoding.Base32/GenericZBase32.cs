using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a generic implementation of z-base-32 encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class GenericZBase32 : GenericBase32
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericZBase32"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericZBase32(TextDataEncodingAlphabet alphabet) :
            base(alphabet)
        {
        }
    }
}
