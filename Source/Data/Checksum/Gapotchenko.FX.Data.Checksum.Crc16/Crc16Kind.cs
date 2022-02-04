namespace Gapotchenko.FX.Data.Checksum
{
    /// <summary>
    /// Represents the kind of CRC-16 algorithm.
    /// </summary>
    public enum Crc16Kind
    {
        /// <summary>
        /// <para>
        /// The standard CRC-16 algorithm.
        /// </para>
        /// <para>
        /// Aliases: CRC-16, CRC-16/ARC, CRC-IBM, CRC-16/LHA.
        /// </para>
        /// <para>
        /// Performs checksum calculation using x^16 + x^15 + x^2 + 1 polynomial with an initial value of 0.
        /// </para>
        /// </summary>
        Standard
    }
}
