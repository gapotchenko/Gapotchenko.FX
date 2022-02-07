namespace Gapotchenko.FX.Data.Checksum
{
    partial class Crc16
    {
        /// <summary>
        /// <para>
        /// Gets the standard CRC-16 algorithm
        /// which performs checksum computation using x^16 + x^15 + x^2 + 1 polynomial with initial value of 0.
        /// </para>
        /// <para>
        /// Aliases: CRC-16, CRC-16/ARC, CRC-IBM, CRC-16/LHA.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x8005, init=0x0000, refin=true, refout=true, xorout=0x0000, check=0xbb3d.
        /// </para>
        /// </remarks>
        public static Crc16 Standard => Impl.Standard.Instance;

        /// <summary>
        /// <para>
        /// Gets CRC-16/CCITT algorithm
        /// which performs checksum computation using x^16 + x^12 + x^8 + 1 polynomial with initial value of 0.
        /// </para>
        /// <para>
        /// Aliases: CRC-16/KERMIT, CRC-16/CCITT-TRUE, CRC-16/V-41-LSB, CRC-CCITT, KERMIT.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x1021, init=0x0000, refin=true, refout=true, xorout=0x0000, check=0x2189.
        /// </para>
        /// </remarks>
        public static Crc16 Ccitt => Impl.Ccitt.Instance;

        /// <summary>
        /// <para>
        /// Gets CRC-16/ISO-IEC-14443-3-A algorithm
        /// which performs checksum computation using x^16 + x^12 + x^8 + 1 polynomial with initial value of 0xC6C6.
        /// </para>
        /// <para>
        /// Aliases: CRC-A.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x1021, init=0xc6c6, refin=true, refout=true, xorout=0x0000, check=0xbf05
        /// </para>
        /// </remarks>
        public static Crc16 IsoIec14443_3_A => Impl.IsoIec14443_3_A.Instance;

        /// <summary>
        /// <para>
        /// Gets CRC-16/ISO-IEC-14443-3-B algorithm
        /// which performs checksum computation using x^16 + x^12 + x^8 + 1 polynomial with initial value of 0xFFFF.
        /// </para>
        /// <para>
        /// Aliases: CRC-16/IBM-SDLC, CRC-16/ISO-HDLC, CRC-16/X-25, CRC-B, X-25.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x1021, init=0xffff, refin=true, refout=true, xorout=0xffff, check=0x906e
        /// </para>
        /// </remarks>
        public static Crc16 IsoIec14443_3_B => Impl.IsoIec14443_3_B.Instance;

        /// <summary>
        /// <para>
        /// Gets CRC-16/MAXIM algorithm
        /// which performs checksum computation using x^16 + x^15 + x^2 + 1 polynomial with initial value of 0.
        /// </para>
        /// <para>
        /// Aliases: CRC-16/MAXIM-DOW.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x8005, init=0x0000, refin=true, refout=true, xorout=0xffff, check=0x44c2
        /// </para>
        /// </remarks>
        public static Crc16 Maxim => Impl.Maxim.Instance;

        static class Impl
        {
            public sealed class Standard : GenericCrc16
            {
                Standard() : base(0x8005, 0, true, true, 0) { }
                public static readonly Standard Instance = new();
            }

            public sealed class Ccitt : GenericCrc16
            {
                Ccitt() : base(0x1021, 0, true, true, 0) { }
                public static readonly Ccitt Instance = new();
            }

            public sealed class IsoIec14443_3_A : GenericCrc16
            {
                IsoIec14443_3_A() : base(0x1021, 0xc6c6, true, true, 0) { }
                public static readonly IsoIec14443_3_A Instance = new();
            }

            public sealed class IsoIec14443_3_B : GenericCrc16
            {
                IsoIec14443_3_B() : base(0x1021, 0xffff, true, true, 0xffff) { }
                public static readonly IsoIec14443_3_B Instance = new();
            }

            public sealed class Maxim : GenericCrc16
            {
                Maxim() : base(0x8005, 0, true, true, 0xffff) { }
                public static readonly Maxim Instance = new();
            }
        }
    }
}
