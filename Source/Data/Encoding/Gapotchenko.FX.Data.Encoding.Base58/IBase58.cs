using System;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// A unifying interface for all possible Base58 encoding variants.
    /// </summary>
    [CLSCompliant(false)]
    public interface IBase58 : IRadixTextDataEncoding, INumericTextDataEncoding
    {
    }
}
