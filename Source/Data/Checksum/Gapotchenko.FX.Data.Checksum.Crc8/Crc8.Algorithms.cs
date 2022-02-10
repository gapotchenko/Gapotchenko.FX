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

        /// <summary>
        /// Attested CRC-8 algorithms.
        /// </summary>
        public static class Attested
        {
            /// <summary>
            /// <para>
            /// Gets CRC-8/TECH-3250 algorithm
            /// which performs checksum computation using x^8 + x^4 + x^3 + x^2 + 1 polynomial with initial value of 0xFF.
            /// </para>
            /// <para>
            /// Aliases: CRC-8/AES, CRC-8/EBU.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x1d, init=0xff, refin=true, refout=true, xorout=0x00, check=0x97.
            /// </para>
            /// </remarks>
            public static Crc8 Tech3250 => Impl.Tech3250.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-8/SAE-J1850 algorithm
            /// which performs checksum computation using x^8 + x^4 + x^3 + x^2 + 1 polynomial with initial value of 0xFF.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x1d, init=0xff, refin=false, refout=false, xorout=0xff, check=0x4b.
            /// </para>
            /// </remarks>
            public static Crc8 SaeJ1850 => Impl.SaeJ1850.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-8/OPENSAFETY algorithm
            /// which performs checksum computation using x^8 + x^5 + x^3 + x^2 + x + 1  polynomial with initial value of 0.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x2f, init=0x00, refin=false, refout=false, xorout=0x00, check=0x3e.
            /// </para>
            /// </remarks>
            public static Crc8 OpenSafety => Impl.OpenSafety.Instance;
        }

        static class Impl
        {
            public sealed class Standard : GenericCrc8
            {
                Standard() : base(0x07, 0, false, false, 0) { }
                public static readonly Standard Instance = new();
            }

            public sealed class Tech3250 : GenericCrc8
            {
                Tech3250() : base(0x1d, 0xff, true, true, 0) { }
                public static readonly Tech3250 Instance = new();
            }

            public sealed class SaeJ1850 : GenericCrc8
            {
                SaeJ1850() : base(0x1d, 0xff, false, false, 0xff) { }
                public static readonly SaeJ1850 Instance = new();
            }

            public sealed class OpenSafety : GenericCrc8
            {
                OpenSafety() : base(0x2f, 0, false, false, 0) { }
                public static readonly OpenSafety Instance = new();
            }
        }
    }
}
