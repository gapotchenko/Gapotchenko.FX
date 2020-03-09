using System;
using System.Numerics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// A unifying interface for all possible Crockford Base 32 encoding implementations.
    /// </summary>
    public interface ICrockfordBase32 : IBase32, INumericTextDataEncoding
    {
    }
}
