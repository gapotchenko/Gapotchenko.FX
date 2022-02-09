using System;

namespace Gapotchenko.FX.Data.Checksum
{
    /// <summary>
    /// Customizable CRC-32 checksum algorithm.
    /// </summary>
    [CLSCompliant(false)]
    public class CustomCrc32 : GenericCrc32
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCrc32"/> class with specified CRC-32 algorithm parameters.
        /// </summary>
        /// <inheritdoc/>
        public CustomCrc32(
            uint polynomial,
            uint initialValue,
            bool reflectedInput,
            bool reflectedOutput,
            uint xorOutput)
            : base(
                  polynomial,
                  initialValue,
                  reflectedInput,
                  reflectedOutput,
                  xorOutput)
        {
        }
    }
}
