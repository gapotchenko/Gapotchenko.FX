// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if NET8_0_OR_GREATER
#define TFF_BITOPERATIONS_CRC32C
#endif

using Gapotchenko.FX.Runtime.CompilerServices;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Numerics;

/// <summary>
/// Provides polyfill extension methods for the <see cref="BitOperations"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class BitOperationsPolyfills
{
#if !TFF_BITOPERATIONS_CRC32C
    static BitOperationsPolyfills()
    {
        Intrinsics.InitializeType(typeof(BitOperationsPolyfills));
    }
#endif

    extension(BitOperations)
    {
        /// <summary>
        /// Computes the CRC-32C checksum of the specified byte, starting with the specified checksum.
        /// </summary>
        /// <param name="crc">The checksum to start with.</param>
        /// <param name="data">The byte to process.</param>
        /// <returns>The updated checksum.</returns>
        [CLSCompliant(false)]
#if !TFF_BITOPERATIONS_CRC32C
        [MachineCodeIntrinsic(
            Architecture.X86,
            0x8b, 0xc1,                    // MOV EAX,ECX
            0xf2, 0x0f, 0x38, 0xf0, 0xc2,  // CRC32 EAX,DL
            AdditionalArchitectures = [Architecture.X64],
            RequiredFeatures = [MachineCodeIntrinsicFeature.Crc32],
            SupportedOSPlatforms = ["windows"])]
        [MachineCodeIntrinsic(
            Architecture.X86,
            0x8b, 0x44, 0x24, 0x04,                    // MOV EAX,[ESP+4]
            0xf2, 0x0f, 0x38, 0xf0, 0x44, 0x24, 0x08,  // CRC32 EAX,BYTE PTR [ESP+8]
            RequiredFeatures = [MachineCodeIntrinsicFeature.Crc32],
            SupportedOSPlatforms = ["linux"])]
        [MachineCodeIntrinsic(
            Architecture.X64,
            0x8b, 0xc7,                          // MOV EAX,EDI
            0xf2, 0x40, 0x0f, 0x38, 0xf0, 0xc6,  // CRC32 EAX,SIL
            RequiredFeatures = [MachineCodeIntrinsicFeature.Crc32],
            SupportedOSPlatforms = ["linux", "macos"])]
        [MachineCodeIntrinsic(
            Architecture.Arm64,
            0x00, 0x50, 0xc1, 0x1a,  // CRC32CB W0,W0,W1
            RequiredFeatures = [MachineCodeIntrinsicFeature.Crc32],
            SupportedOSPlatforms = ["windows", "linux", "macos"])]
        [MethodImpl(MethodImplOptions.NoInlining)]
#endif
        public static uint Crc32C(uint crc, byte data)
        {
#if TFF_BITOPERATIONS_CRC32C
            return BitOperations.Crc32C(crc, data);
#else
            return (crc >> 8) ^ Crc32CImpl.Table[(byte)(crc ^ data)];
#endif
        }
    }

#if !TFF_BITOPERATIONS_CRC32C

    static class Crc32CImpl
    {
        public static uint[] Table { get; } = CreateTable();

        static uint[] CreateTable()
        {
            const uint polynomial = 0x82f63b78U;
            uint[] table = new uint[256];

            for (int i = 0; i < table.Length; ++i)
            {
                uint value = (uint)i;
                for (int j = 0; j < 8; ++j)
                    value = (value >> 1) ^ (polynomial & (uint)-(int)(value & 1));
                table[i] = value;
            }

            return table;
        }
    }

#endif
}
