// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

/// <summary>
/// Intrinsic patcher for Windows OS and ARM-based 64-bit processor architecture.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
sealed class AdapterWindowsArm64 : Adapter
{
    public override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        // Every ARM64 instruction is four bytes long.
        if ((code.Length & (sizeof(uint) - 1)) != 0)
            return PatchResult.InvalidAlignment;

        var methodInstructions = GetMethodInstructions(method);
        if (!IsSupportedPrologue(methodInstructions))
            return PatchResult.UnexpectedPrologue;

        var patchInstructions = MemoryMarshal.Cast<byte, uint>(code);

        int patchSize = patchInstructions.Length + 1 /* RET */;
        if (patchSize > methodInstructions.Length)
            return PatchResult.NoSpace;

        methodInstructions = methodInstructions[..patchSize];

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
            using var scope = VirtualProtectionScope.Create(methodInstructions, NativeMethods.PageProtect.ExecuteReadWrite);

            // Put the intrinsic code.
            patchInstructions.CopyTo(methodInstructions);

            // End the method with a RET instruction.
            methodInstructions[patchInstructions.Length] = 0xd65f03c0;

            scope.FlushInstructions();
        }

        return PatchResult.Success;
    }

    static bool IsSupportedPrologue(ReadOnlySpan<uint> instructions)
    {
        if (instructions.Length < 1)
            return false;

        uint instruction = instructions[0];
        return
            // STP X29, X30, [SP, #-imm]!
            (instruction & 0xffc07fff) == 0xa9807bfd ||
            // SUB SP, SP, #imm{, LSL #12}
            (instruction & 0xff8003ff) == 0xd10003ff;
    }

    static unsafe Span<uint> GetMethodInstructions(MethodInfo method)
    {
        // Compile the method.
        RuntimeHelpers.PrepareMethod(method.MethodHandle);

        // Get pointer to the first instruction.
        uint* p = (uint*)method.MethodHandle.GetFunctionPointer();
        p = SkipBranches(p);

        return new(p, int.MaxValue);

        static uint* SkipBranches(uint* p)
        {
            // B label: the signed imm26 operand is measured in four-byte instructions.
            while ((*p & 0xfc000000) == 0x14000000)
            {
                // Sign-extend the operand and scale it by four to obtain a byte displacement in one go.
                int displacement = (int)(*p << 6) >> 4;
                p = (uint*)((byte*)p + displacement);
            }
            return p;
        }
    }
}
