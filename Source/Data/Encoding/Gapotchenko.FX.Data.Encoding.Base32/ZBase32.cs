using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides z-base-32 encoding implementation.
    /// </summary>
    public sealed class ZBase32 : GenericZBase32
    {
        private ZBase32() :
            base(new TextDataEncodingAlphabet("ybndrfg8ejkmcpqxot1uwisza345h769", false))
        {
        }

        /// <inheritdoc/>
        public override string Name => "z-base-32";
    }
}
