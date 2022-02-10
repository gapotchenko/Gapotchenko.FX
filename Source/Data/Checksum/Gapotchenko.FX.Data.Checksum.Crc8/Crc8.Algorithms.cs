namespace Gapotchenko.FX.Data.Checksum
{
    partial class Crc8
    {
        /// <summary>
        /// <para>
        /// Gets the standard CRC-8 algorithm
        /// which performs checksum computation using x^8 + x^2 + x + 1 polynomial with initial value of 0.
        /// </para>
        /// <para>
        /// Alias: CRC-8/SMBUS.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x07, init=0x00, refin=false, refout=false, xorout=0x00, check=0xf4.
        /// </para>
        /// </remarks>
        public static Crc8 Standard => Impl.Standard.Instance;

        static class Impl
        {
            public sealed class Standard : GenericCrc8
            {
                Standard() : base(0x07, 0, false, false, 0) { }
                public static readonly Standard Instance = new();
            }
        }
    }
}
