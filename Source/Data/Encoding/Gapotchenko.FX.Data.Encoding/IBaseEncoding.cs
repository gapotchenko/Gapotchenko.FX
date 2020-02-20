using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Defines the interface of a binary-to-text encoding based on positional numeral system.
    /// </summary>
    public interface IBaseEncoding : IDataTextEncoding
    {
        /// <summary>
        /// Gets the number of unique symbols in positional numeral system of the encoding.
        /// </summary>
        int Radix { get; }
    }
}
