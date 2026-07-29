// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using System.Reflection;
using System.Runtime.CompilerServices;

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

        int patchSize = code.Length + sizeof(ushort) /* BX LR */;
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
            code.CopyTo(instructions);

            // End the method with a BX LR instruction.
            instructions[code.Length] = 0x70;     // BX LR
            instructions[code.Length + 1] = 0x47;

            scope.FlushInstructions();
        }

        return PatchResult.Success;
    }

    static bool IsSupportedPrologue(ReadOnlySpan<byte> instructions)
    {
        if (instructions.Length < sizeof(ushort))
            return false;

        ushort instruction = (ushort)(instructions[0] | instructions[1] << 8);
        return
            // PUSH {..., LR}
            (instruction & 0xff00) == 0xb500 ||
            // PUSH.W {..., LR}
            (instruction == 0xe92d && instructions.Length >= 4 && (instructions[3] & 0x40) != 0) ||
            // SUB SP, #imm
            (instruction & 0xff80) == 0xb080;
    }

    static unsafe Span<byte> GetMethodInstructions(MethodInfo method)
    {
        RuntimeHelpers.PrepareMethod(method.MethodHandle);
        ushort* p = SkipBranches((ushort*)method.MethodHandle.GetFunctionPointer());

        if (((nuint)p & (sizeof(ushort) - 1)) != 0)
            return [];

        return Unwind.GetMethodInstructions((byte*)p);
    }
}
