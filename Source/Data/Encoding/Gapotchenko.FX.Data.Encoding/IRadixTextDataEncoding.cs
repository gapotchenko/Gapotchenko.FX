namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Defines the interface of a binary-to-text encoding based on positional numeral system.
    /// </summary>
    public interface IRadixTextDataEncoding : ITextDataEncoding
    {
        /// <summary>
        /// Gets the number of unique symbols in positional numeral system of the encoding.
        /// </summary>
        int Radix { get; }
    }
}
