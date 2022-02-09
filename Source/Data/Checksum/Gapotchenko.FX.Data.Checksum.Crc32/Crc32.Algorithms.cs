namespace Gapotchenko.FX.Data.Checksum
{
    partial class Crc32
    {
        /// <summary>
        /// <para>
        /// Gets the standard CRC-32 algorithm
        /// which performs checksum computation using x^32 + x^26 + x^23 + x^22 + x^16 + x^12 + x^11 + x^10 + x^8 + x^7 + x^5 + x^4 + x^2 + x + 1 polynomial with initial value of 0xFFFFFFFF.
        /// </para>
        /// <para>
        /// Aliases: CRC-32, CRC-32/ISO-HDLC, CRC-32/ADCCP, CRC-32/V-42, CRC-32/XZ, PKZIP.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x04c11db7, init=0xffffffff, refin=true, refout=true, xorout=0xffffffff, check=0xcbf43926.
        /// </para>
        /// </remarks>
        public static Crc32 Standard => Impl.Standard.Instance;

        /// <summary>
        /// Attested CRC-32 algorithms.
        /// </summary>
        public static class Attested
        {
            /// <summary>
            /// <para>
            /// Gets CRC-32C algorithm
            /// which performs checksum computation using x^32 + x^28 + x^27 + x^26 + x^25 + x^23 + x^22 + x^20 + x^19 + x^18 + x^14 + x^13 + x^11 + x^10 + x^9 + x^8 + x^6 + 1 polynomial with initial value of 0xFFFFFFFF.
            /// </para>
            /// <para>
            /// Aliases: CRC-32/BASE91-C, CRC-32/CASTAGNOLI, CRC-32/INTERLAKEN, CRC-32/ISCSI.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x1edc6f41, init=0xffffffff, refin=true, refout=true, xorout=0xffffffff, check=0xe3069283.
            /// </para>
            /// </remarks>
            public static Crc32 C => Impl.C.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-32D algorithm
            /// which performs checksum computation using x^32 + x^31 + x^29 + x^27 + x^21 + x^20 + x^17 + x^16 + x^15 + x^12 + x^11 + x^5 + x^3 + x + 1 polynomial with initial value of 0xFFFFFFFF.
            /// </para>
            /// <para>
            /// Alias: CRC-32/BASE91-D.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0xa833982b, init=0xffffffff, refin=true, refout=true, xorout=0xffffffff, check=0x87315576.
            /// </para>
            /// </remarks>
            public static Crc32 D => Impl.D.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-32Q algorithm
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

            /// <summary>
            /// <para>
            /// Gets CRC-32/POSIX algorithm
            /// which performs checksum computation using x^32 + x^26 + x^23 + x^22 + x^16 + x^12 + x^11 + x^10 + x^8 + x^7 + x^5 + x^4 + x^2 + x + 1 polynomial with initial value of 0.
            /// </para>
            /// <para>
            /// Aliases: CRC-32/CKSUM, CKSUM.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x04c11db7, init=0x00000000, refin=false, refout=false, xorout=0xffffffff, check=0x765e7680.
            /// </para>
            /// </remarks>
            public static Crc32 Posix => Impl.Posix.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-32/DECT-B algorithm
            /// which performs checksum computation using x^32 + x^26 + x^23 + x^22 + x^16 + x^12 + x^11 + x^10 + x^8 + x^7 + x^5 + x^4 + x^2 + x + 1 polynomial with initial value of 0xFFFFFFFF.
            /// </para>
            /// <para>
            /// Aliases: CRC-32/BZIP2, CRC-32/AAL5, B-CRC-32.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x04c11db7, init=0xffffffff, refin=false, refout=false, xorout=0xffffffff, check=0xfc891918.
            /// </para>
            /// </remarks>
            public static Crc32 DectB => Impl.DectB.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-32/MEF algorithm
            /// which performs checksum computation using x^32 + x^30 + x^29 + x^28 + x^26 + x^20 + x^19 + x^17 + x^16 + x^15 + x^11 + x^10 + x^7 + x^6 + x^4 + x^2 + x + 1 polynomial with initial value of 0xFFFFFFFF.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x741b8cd7, init=0xffffffff, refin=true, refout=true, xorout=0x00000000, check=0xd2c22f51.
            /// </para>
            /// </remarks>
            public static Crc32 Mef => Impl.Mef.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-32/MPEG-2 algorithm
            /// which performs checksum computation using x^32 + x^26 + x^23 + x^22 + x^16 + x^12 + x^11 + x^10 + x^8 + x^7 + x^5 + x^4 + x^2 + x + 1 polynomial with initial value of 0xFFFFFFFF.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x04c11db7, init=0xffffffff, refin=false, refout=false, xorout=0x00000000, check=0x0376e6e7.
            /// </para>
            /// </remarks>
            public static Crc32 Mpeg2 => Impl.Mpeg2.Instance;
        }

        static class Impl
        {
            public sealed class Standard : GenericCrc32
            {
                Standard() : base(0x04c11db7, 0xffffffff, true, true, 0xffffffff) { }
                public static readonly Standard Instance = new();
            }

            public sealed class C : GenericCrc32
            {
                C() : base(0x1edc6f41, 0xffffffff, true, true, 0xffffffff) { }
                public static readonly C Instance = new();
            }

            public sealed class D : GenericCrc32
            {
                D() : base(0xa833982b, 0xffffffff, true, true, 0xffffffff) { }
                public static readonly D Instance = new();
            }

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

            public sealed class Posix : GenericCrc32
            {
                Posix() : base(0x04c11db7, 0, false, false, 0xffffffff) { }
                public static readonly Posix Instance = new();
            }

            public sealed class DectB : GenericCrc32
            {
                DectB() : base(0x04c11db7, 0xffffffff, false, false, 0xffffffff) { }
                public static readonly DectB Instance = new();
            }

            public sealed class Mef : GenericCrc32
            {
                Mef() : base(0x741b8cd7, 0xffffffff, true, true, 0) { }
                public static readonly Mef Instance = new();
            }

            public sealed class Mpeg2 : GenericCrc32
            {
                Mpeg2() : base(0x04c11db7, 0xffffffff, false, false, 0) { }
                public static readonly Mpeg2 Instance = new();
            }
        }
    }
}
