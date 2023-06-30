// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Data.Integrity.Checksum;

partial class Crc8
{
    /// <summary>
    /// <para>
    /// Gets the standard CRC-8 algorithm
    /// which performs checksum computation using <c>x^8 + x^2 + x + 1</c> polynomial with the initial value of 0.
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
        /// which performs checksum computation using <c>x^8 + x^4 + x^3 + x^2 + 1</c> polynomial with the initial value of 0xFF.
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
        /// which performs checksum computation using <c>x^8 + x^4 + x^3 + x^2 + 1</c> polynomial with the initial value of 0xFF.
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
        /// which performs checksum computation using <c>x^8 + x^5 + x^3 + x^2 + x + 1</c> polynomial with the initial value of 0.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x2f, init=0x00, refin=false, refout=false, xorout=0x00, check=0x3e.
        /// </para>
        /// </remarks>
        public static Crc8 OpenSafety => Impl.OpenSafety.Instance;

        /// <summary>
        /// <para>
        /// Gets CRC-8/NRSC-5 algorithm
        /// which performs checksum computation using <c>x^8 + x^5 + x^4 + 1</c> polynomial with the initial value of 0xFF.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x31, init=0xff, refin=false, refout=false, xorout=0x00, check=0xf7.
        /// </para>
        /// </remarks>
        public static Crc8 Nrsc5 => Impl.Nrsc5.Instance;

        /// <summary>
        /// <para>
        /// Gets CRC-8/MIFARE-MAD algorithm
        /// which performs checksum computation using <c>x^8 + x^4 + x^3 + x^2 + 1</c> polynomial with the initial value of 0xC7.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x1d, init=0xc7, refin=false, refout=false, xorout=0x00, check=0x99.
        /// </para>
        /// </remarks>
        public static Crc8 MifareMad => Impl.MifareMad.Instance;

        /// <summary>
        /// <para>
        /// Gets CRC-8/MAXIM algorithm
        /// which performs checksum computation using <c>x^8 + x^5 + x^4 + 1</c> polynomial with the initial value of 0.
        /// </para>
        /// <para>
        /// Aliases: CRC-8/MAXIM-DOW, DOW-CRC.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x31, init=0x00, refin=true, refout=true, xorout=0x00, check=0xa1.
        /// </para>
        /// </remarks>
        public static Crc8 Maxim => Impl.Maxim.Instance;

        /// <summary>
        /// <para>
        /// Gets CRC-8/I-CODE algorithm
        /// which performs checksum computation using <c>x^8 + x^4 + x^3 + x^2 + 1</c> polynomial with the initial value of 0xFD.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x1d, init=0xfd, refin=false, refout=false, xorout=0x00, check=0x7e.
        /// </para>
        /// </remarks>
        public static Crc8 ICode => Impl.ICode.Instance;

        /// <summary>
        /// <para>
        /// Gets CRC-8/HITAG algorithm
        /// which performs checksum computation using <c>x^8 + x^4 + x^3 + x^2 + 1</c> polynomial with the initial value of 0xFF.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x1d, init=0xff, refin=false, refout=false, xorout=0x00, check=0xb4.
        /// </para>
        /// </remarks>
        public static Crc8 Hitag => Impl.Hitag.Instance;

        /// <summary>
        /// <para>
        /// Gets CRC-8/DARC algorithm
        /// which performs checksum computation using <c>x^8 + x^5 + x^4 + x^3 + 1</c> polynomial with the initial value of 0.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x39, init=0x00, refin=true, refout=true, xorout=0x00, check=0x15.
        /// </para>
        /// </remarks>
        public static Crc8 Darc => Impl.Darc.Instance;

        /// <summary>
        /// <para>
        /// Gets CRC-8/BLUETOOTH algorithm
        /// which performs checksum computation using <c>x^8 + x^7 + x^5 + x^2 + x + 1</c> polynomial with the initial value of 0.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0xa7, init=0x00, refin=true, refout=true, xorout=0x00, check=0x26.
        /// </para>
        /// </remarks>
        public static Crc8 Bluetooth => Impl.Bluetooth.Instance;

        /// <summary>
        /// <para>
        /// Gets CRC-8/AUTOSAR algorithm
        /// which performs checksum computation using <c>x^8 + x^5 + x^3 + x^2 + x + 1</c>  polynomial with the initial value of 0xFF.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x2f, init=0xff, refin=false, refout=false, xorout=0xff, check=0xdf.
        /// </para>
        /// </remarks>
        public static Crc8 Autosar => Impl.Autosar.Instance;
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

        public sealed class Nrsc5 : GenericCrc8
        {
            Nrsc5() : base(0x31, 0xff, false, false, 0) { }
            public static readonly Nrsc5 Instance = new();
        }

        public sealed class MifareMad : GenericCrc8
        {
            MifareMad() : base(0x1d, 0xc7, false, false, 0) { }
            public static readonly MifareMad Instance = new();
        }

        public sealed class Maxim : GenericCrc8
        {
            Maxim() : base(0x31, 0, true, true, 0) { }
            public static readonly Maxim Instance = new();
        }

        public sealed class ICode : GenericCrc8
        {
            ICode() : base(0x1d, 0xfd, false, false, 0) { }
            public static readonly ICode Instance = new();
        }

        public sealed class Hitag : GenericCrc8
        {
            Hitag() : base(0x1d, 0xff, false, false, 0) { }
            public static readonly Hitag Instance = new();
        }

        public sealed class Darc : GenericCrc8
        {
            Darc() : base(0x39, 0, true, true, 0) { }
            public static readonly Darc Instance = new();
        }

        public sealed class Bluetooth : GenericCrc8
        {
            Bluetooth() : base(0xa7, 0, true, true, 0) { }
            public static readonly Bluetooth Instance = new();
        }

        public sealed class Autosar : GenericCrc8
        {
            Autosar() : base(0x2f, 0xff, false, false, 0xff) { }
            public static readonly Autosar Instance = new();
        }
    }
}
