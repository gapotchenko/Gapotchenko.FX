namespace Gapotchenko.FX.Data.Checksum
{
    partial class Crc32
    {
        /// <summary>
        /// Attested CRC-32 algorithms.
        /// </summary>
        public static class Attested
        {
            /// <summary>
            /// <para>
            /// Gets CRC-32/Q algorithm
            /// which performs checksum computation using x^32 + x^31 + x^24 + x^22 + x^16 + x^14 + x^8 + x^7 + x^5 + x^3 + x + 1 polynomial with initial value of 0.
            /// </para>
            /// <para>
            /// Alias: CRC-32/AIXM.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x814141ab, init=0x00000000, refin=false, refout=false, xorout=0x00000000, check=0x3010bf7f.
            /// </para>
            /// </remarks>
            public static Crc32 Q => Impl.Q.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-32/AUTOSAR algorithm
            /// which performs checksum computation using x^32 + x^31 + x^30 + x^29 + x^28 + x^26 + x^23 + x^21 + x^19 + x^18 + x^15 + x^14 + x^13 + x^12 + x^11 + x^9 + x^8 + x^4 + x + 1 polynomial with initial value of 0xFFFFFFFF.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0xf4acfb13, init=0xffffffff, refin=true, refout=true, xorout=0xffffffff, check=0x1697d06a.
            /// </para>
            /// </remarks>
            public static Crc32 Autosar => Impl.Autosar.Instance;
        }

        static class Impl
        {
            public sealed class Q : GenericCrc32
            {
                Q() : base(0x814141ab, 0, false, false, 0) { }
                public static readonly Q Instance = new();
            }

            public sealed class Autosar : GenericCrc32
            {
                Autosar() : base(0xf4acfb13, 0xffffffff, true, true, 0xffffffff) { }
                public static readonly Autosar Instance = new();
            }
        }
    }
}
