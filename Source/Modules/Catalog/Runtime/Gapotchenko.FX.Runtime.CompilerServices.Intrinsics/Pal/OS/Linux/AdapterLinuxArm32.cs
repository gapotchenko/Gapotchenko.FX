// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Linux;

#if NET
[SupportedOSPlatform("linux")]
#endif
sealed class AdapterLinuxArm32 : AdapterArm32
{
    public override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        // Thumb instructions are at least two bytes long and aligned accordingly.
        if ((code.Length & (sizeof(ushort) - 1)) != 0)
            return PatchResult.InvalidAlignment;

        var instructions = GetMethodInstructions(method);
        if (!IsSupportedPrologue(instructions))
            return PatchResult.UnexpectedPrologue;

        var patchCode = MemoryMarshal.Cast<byte, ushort>(code);

        int patchSize = patchCode.Length + 1 /* BX LR */;
        if (patchSize > instructions.Length)
            return PatchResult.NoSpace;

        instructions = instructions[..patchSize];

#if TFF_CER
        // Ensure that code changes are atomic by using the constrained execution region.
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
#endif
        {
            using var scope = MemoryProtectionScope.Create(instructions, NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute);

            // Put the intrinsic code.
            patchCode.CopyTo(instructions);

            // End the method with a BX LR instruction.
            instructions[patchCode.Length] = BxLr;

            scope.FlushInstructions();
        }

        return PatchResult.Success;
    }

    static bool IsSupportedPrologue(ReadOnlySpan<ushort> instructions)
    {
        if (instructions.Length < 1)
            return false;

        ushort instruction = instructions[0];
        return
            // PUSH {..., LR}
            (instruction & 0xff00) == 0xb500 ||
            // PUSH.W {..., LR}
            (instruction == 0xe92d && instructions.Length >= 2 && (instructions[1] & 0x4000) != 0) ||
            // SUB SP, #imm
            (instruction & 0xff80) == 0xb080;
    }

    static unsafe Span<ushort> GetMethodInstructions(MethodInfo method)
    {
        RuntimeHelpers.PrepareMethod(method.MethodHandle);
        ushort* p = SkipBranches((ushort*)method.MethodHandle.GetFunctionPointer());

        if (((nuint)p & (sizeof(ushort) - 1)) != 0)
            return [];

        var bytes = Unwind.GetMethodInstructions((byte*)p);
        return MemoryMarshal.Cast<byte, ushort>(bytes);
    }
}
