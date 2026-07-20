// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Windows;

/// <summary>
/// Intrinsic patcher for Windows OS and ARM-based 64-bit processor architecture.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
sealed unsafe class PatcherWindowsArm64 : Patcher
{
    public override PatchResult PatchMethod(MethodInfo method, byte[] code)
    {
        ArgumentNullException.ThrowIfNull(code);

        // Every ARM64 instruction is four bytes long.
        if ((code.Length & (InstructionSize - 1)) != 0)
            return PatchResult.InvalidAlignment;

        uint* p = GetPointerToMethodInstructions(method);
        if (!IsSupportedPrologue(p))
            return PatchResult.UnexpectedEpilogue;

#if TFF_CER
        // Ensure that code changes are atomic by using the constrained execution region.
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
#endif
        {
            // Temporarily allow memory modification in order to apply the intrinsic code.
            using var scope = new VirtualProtectionScope(
                p,
                code.Length + InstructionSize /* RET */,
                NativeMethods.PageProtect.ExecuteReadWrite);

            var body = scope.GetSpan<uint>();

            // Put the intrinsic code.
            var codeInstructions = MemoryMarshal.Cast<byte, uint>(code);
            codeInstructions.CopyTo(body);

            // End the method with a RET instruction.
            body[codeInstructions.Length] = 0xd65f03c0;

            // ARM64 has non-coherent data and instruction caches. Make the newly written
            // instructions visible to the processor before making the page read-only again.
            if (!NativeMethods.FlushInstructionCache(new IntPtr(-1), p, (nuint)scope.Size))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        return PatchResult.Success;
    }

    const int InstructionSize = sizeof(uint);

    static bool IsSupportedPrologue(uint* instructions)
    {
        uint instruction = *instructions;
        return
            // STP X29, X30, [SP, #-imm]!
            (instruction & 0xffc07fff) == 0xa9807bfd ||
            // SUB SP, SP, #imm{, LSL #12}
            (instruction & 0xff8003ff) == 0xd10003ff;
    }

    static uint* GetPointerToMethodInstructions(MethodInfo method)
    {
        // Compile the method.
        RuntimeHelpers.PrepareMethod(method.MethodHandle);

        // Get pointer to the first instruction.
        uint* p = (uint*)method.MethodHandle.GetFunctionPointer();
        p = SkipBranches(p);
        return p;

        static uint* SkipBranches(uint* p)
        {
            // B label: the signed imm26 operand is measured in four-byte instructions.
            while ((*p & 0xfc000000) == 0x14000000)
            {
                int displacement = (int)(*p << 6) >> 4;
                p = (uint*)((byte*)p + displacement);
            }
            return p;
        }
    }
}
