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
        /// Attested CRC-16 algorithms.
        /// </summary>
        public static class Attested
        {
            /// <summary>
            /// <para>
            /// Gets CRC-16/CCITT algorithm
            /// which performs checksum computation using x^16 + x^12 + x^5 + 1 polynomial with initial value of 0.
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
            /// which performs checksum computation using x^16 + x^12 + x^5 + 1 polynomial with initial value of 0xC6C6.
            /// </para>
            /// <para>
            /// Alias: CRC-A.
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
            /// which performs checksum computation using x^16 + x^12 + x^5 + 1 polynomial with initial value of 0xFFFF.
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
            /// Gets CRC-16/NRSC-5 algorithm
            /// which performs checksum computation using x^16 + x^11 + x^3 + x + 1 polynomial with initial value of 0xFFFF.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x080b, init=0xffff, refin=true, refout=true, xorout=0x0000, check=0xa066.
            /// </para>
            /// </remarks>
            public static Crc16 Nrsc5 => Impl.Nrsc5.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/MAXIM algorithm
            /// which performs checksum computation using x^16 + x^15 + x^2 + 1 polynomial with initial value of 0.
            /// </para>
            /// <para>
            /// Alias: CRC-16/MAXIM-DOW.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x8005, init=0x0000, refin=true, refout=true, xorout=0xffff, check=0x44c2
            /// </para>
            /// </remarks>
            public static Crc16 Maxim => Impl.Maxim.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/SPI-FUJITSU algorithm
            /// which performs checksum computation using x^16 + x^12 + x^5 + 1 polynomial with initial value of 0x1D0F.
            /// </para>
            /// <para>
            /// Alias: CRC-16/AUG-CCITT.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x1021, init=0x1d0f, refin=false, refout=false, xorout=0x0000, check=0xe5cc.
            /// </para>
            /// </remarks>
            public static Crc16 SpiFujitsu => Impl.SpiFujitsu.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/UMTS algorithm
            /// which performs checksum computation using x^16 + x^15 + x^2 + 1 polynomial with initial value of 0.
            /// </para>
            /// <para>
            /// Aliases: CRC-16/VERIFONE, CRC-16/BUYPASS.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x8005, init=0x0000, refin=false, refout=false, xorout=0x0000, check=0xfee8.
            /// </para>
            /// </remarks>
            public static Crc16 Umts => Impl.Umts.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/USB algorithm
            /// which performs checksum computation using x^16 + x^15 + x^2 + 1 polynomial with initial value of 0.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x8005, init=0xffff, refin=true, refout=true, xorout=0xffff, check=0xb4c8.
            /// </para>
            /// </remarks>
            public static Crc16 Usb => Impl.Usb.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/XMODEM algorithm
            /// which performs checksum computation using x^16 + x^12 + x^5 + 1 polynomial with initial value of 0.
            /// </para>
            /// <para>
            /// Aliases: CRC-16/ACORN, CRC-16/LTE, CRC-16/V-41-MSB, XMODEM, ZMODEM.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x1021, init=0x0000, refin=false, refout=false, xorout=0x0000, check=0x31c3.
            /// </para>
            /// </remarks>
            public static Crc16 XModem => Impl.XModem.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/PROFIBUS algorithm
            /// which performs checksum computation using x^16 + x^12 + x^11 + x^10 + x^8 + x^7 + x^6 + x^3 + x^2 + x + 1 polynomial with initial value of 0xFFFF.
            /// </para>
            /// <para>
            /// Alias: CRC-16/IEC-61158-2.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x1dcf, init=0xffff, refin=false, refout=false, xorout=0xffff, check=0xa819.
            /// </para>
            /// </remarks>
            public static Crc16 Profibus => Impl.Profibus.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/MODBUS algorithm
            /// which performs checksum computation using x^16 + x^15 + x^2 + 1 polynomial with initial value of 0xFFFF.
            /// </para>
            /// <para>
            /// Alias: MODBUS.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x8005, init=0xffff, refin=true, refout=true, xorout=0x0000, check=0x4b37.
            /// </para>
            /// </remarks>
            public static Crc16 Modbus => Impl.Modbus.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/GENIBUS algorithm
            /// which performs checksum computation using x^16 + x^12 + x^5 + 1 polynomial with initial value of 0xFFFF.
            /// </para>
            /// <para>
            /// Aliases: CRC-16/DARC, CRC-16/EPC, CRC-16/EPC-C1G2, CRC-16/I-CODE.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x1021, init=0xffff, refin=false, refout=false, xorout=0xffff, check=0xd64e.
            /// </para>
            /// </remarks>
            public static Crc16 Genibus => Impl.Genibus.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/GSM algorithm
            /// which performs checksum computation using x^16 + x^12 + x^5 + 1 polynomial with initial value of 0.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x1021, init=0x0000, refin=false, refout=false, xorout=0xffff, check=0xce3c.
            /// </para>
            /// </remarks>
            public static Crc16 Gsm => Impl.Gsm.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/OPENSAFETY-A algorithm
            /// which performs checksum computation using x^16 + x^14 + x^12 + x^11 + x^8 + x^5 + x^4 + x^2 + 1 polynomial with initial value of 0.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x5935, init=0x0000, refin=false, refout=false, xorout=0x0000, check=0x5d38.
            /// </para>
            /// </remarks>
            public static Crc16 OpenSafetyA => Impl.OpenSafetyA.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/OPENSAFETY-B algorithm
            /// which performs checksum computation using x^16 + x^14 + x^13 + x^12 + x^10 + x^8 + x^6 + x^4 + x^3 + x + 1 polynomial with initial value of 0.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x755b, init=0x0000, refin=false, refout=false, xorout=0x0000, check=0x20fe.
            /// </para>
            /// </remarks>
            public static Crc16 OpenSafetyB => Impl.OpenSafetyB.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/TMS37157 algorithm
            /// which performs checksum computation using x^16 + x^12 + x^5 + 1 polynomial with initial value of 0x89EC.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x1021, init=0x89ec, refin=true, refout=true, xorout=0x0000, check=0x26b1.
            /// </para>
            /// </remarks>
            public static Crc16 TMS37157 => Impl.TMS37157.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/MCRF4XX algorithm
            /// which performs checksum computation using x^16 + x^12 + x^5 + 1 polynomial with initial value of 0xFFFF.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x1021, init=0xffff, refin=true, refout=true, xorout=0x0000, check=0x6f91.
            /// </para>
            /// </remarks>
            public static Crc16 MCRF4XX => Impl.MCRF4XX.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/DECT-R algorithm
            /// which performs checksum computation using x^16 + x^10 + x^8 + x^7 + x^3 + 1 polynomial with initial value of 0.
            /// </para>
            /// <para>
            /// Alias: R-CRC-16.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x0589, init=0x0000, refin=false, refout=false, xorout=0x0001, check=0x007e.
            /// </para>
            /// </remarks>
            public static Crc16 DectR => Impl.DectR.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/DECT-X algorithm
            /// which performs checksum computation using x^16 + x^10 + x^8 + x^7 + x^3 + 1 polynomial with initial value of 0.
            /// </para>
            /// <para>
            /// Alias: X-CRC-16.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x0589, init=0x0000, refin=false, refout=false, xorout=0x0000, check=0x007f.
            /// </para>
            /// </remarks>
            public static Crc16 DectX => Impl.DectX.Instance;

            /// <summary>
            /// <para>
            /// Gets CRC-16/DDS-110 algorithm
            /// which performs checksum computation using x^16 + x^15 + x^2 + 1 polynomial with initial value of 0x800D.
            /// </para>
            /// </summary>
            /// <remarks>
            /// <para>
            /// Parameters: poly=0x8005, init=0x800d, refin=false, refout=false, xorout=0x0000, check=0x9ecf.
            /// </para>
            /// </remarks>
            public static Crc16 Dds110 => Impl.Dds110.Instance;
        }

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

            public sealed class Nrsc5 : GenericCrc16
            {
                Nrsc5() : base(0x080b, 0xffff, true, true, 0) { }
                public static readonly Nrsc5 Instance = new();
            }

            public sealed class Maxim : GenericCrc16
            {
                Maxim() : base(0x8005, 0, true, true, 0xffff) { }
                public static readonly Maxim Instance = new();
            }

            public sealed class SpiFujitsu : GenericCrc16
            {
                SpiFujitsu() : base(0x1021, 0x1d0f, false, false, 0) { }
                public static readonly SpiFujitsu Instance = new();
            }

            public sealed class Umts : GenericCrc16
            {
                Umts() : base(0x8005, 0, false, false, 0) { }
                public static readonly Umts Instance = new();
            }

            public sealed class Usb : GenericCrc16
            {
                Usb() : base(0x8005, 0xffff, true, true, 0xffff) { }
                public static readonly Usb Instance = new();
            }

            public sealed class XModem : GenericCrc16
            {
                XModem() : base(0x1021, 0, false, false, 0) { }
                public static readonly XModem Instance = new();
            }

            public sealed class Profibus : GenericCrc16
            {
                Profibus() : base(0x1dcf, 0xffff, false, false, 0xffff) { }
                public static readonly Profibus Instance = new();
            }

            public sealed class Modbus : GenericCrc16
            {
                Modbus() : base(0x8005, 0xffff, true, true, 0) { }
                public static readonly Modbus Instance = new();
            }

            public sealed class Genibus : GenericCrc16
            {
                Genibus() : base(0x1021, 0xffff, false, false, 0xffff) { }
                public static readonly Genibus Instance = new();
            }

            public sealed class Gsm : GenericCrc16
            {
                Gsm() : base(0x1021, 0, false, false, 0xffff) { }
                public static readonly Gsm Instance = new();
            }

            public sealed class OpenSafetyA : GenericCrc16
            {
                OpenSafetyA() : base(0x5935, 0, false, false, 0) { }
                public static readonly OpenSafetyA Instance = new();
            }

            public sealed class OpenSafetyB : GenericCrc16
            {
                OpenSafetyB() : base(0x755b, 0, false, false, 0) { }
                public static readonly OpenSafetyB Instance = new();
            }

            public sealed class TMS37157 : GenericCrc16
            {
                TMS37157() : base(0x1021, 0x89ec, true, true, 0) { }
                public static readonly TMS37157 Instance = new();
            }

            public sealed class MCRF4XX : GenericCrc16
            {
                MCRF4XX() : base(0x1021, 0xffff, true, true, 0) { }
                public static readonly MCRF4XX Instance = new();
            }

            public sealed class DectR : GenericCrc16
            {
                DectR() : base(0x0589, 0, false, false, 0x0001) { }
                public static readonly DectR Instance = new();
            }

            public sealed class DectX : GenericCrc16
            {
                DectX() : base(0x0589, 0, false, false, 0) { }
                public static readonly DectX Instance = new();
            }

            public sealed class Dds110 : GenericCrc16
            {
                Dds110() : base(0x8005, 0x800d, false, false, 0) { }
                public static readonly Dds110 Instance = new();
            }
        }
    }
}
