using Gapotchenko.FX.Runtime.CompilerServices;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Harness.Console;

static class HWAcceleration
{
    static HWAcceleration()
    {
        // Ensure that intrinsic methods are initialized (compiled) before they can be used.
        Intrinsics.InitializeType(typeof(HWAcceleration));
    }

    static readonly int[] m_Log2DeBruijn32 =
    [
         0,  9,  1, 10, 13, 21,  2, 29,
        11, 14, 16, 18, 22, 25,  3, 30,
         8, 12, 20, 28, 15, 17, 24,  7,
        19, 27, 23,  6, 26,  5,  4, 31
    ];

    // Define machine code intrinsic for the method
    [MachineCodeIntrinsic(
        Architecture.X86,
        0x83, 0xc9, 0x01,  // OR ECX,1
        0x0f, 0xbd, 0xc1,  // BSR EAX,ECX
        AdditionalArchitectures = [Architecture.X64])]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int Log2_Intrinsic(uint value)
    {
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;

        uint index = (value * 0x07c4acddU) >> 27;
        return m_Log2DeBruijn32[index];
    }
}

