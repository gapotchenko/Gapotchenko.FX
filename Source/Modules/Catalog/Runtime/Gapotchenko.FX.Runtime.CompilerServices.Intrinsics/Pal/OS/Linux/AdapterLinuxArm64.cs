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
sealed class AdapterLinuxArm64 : AdapterArm64
{
    public override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        // Every ARM64 instruction is four bytes long.
        if ((code.Length & (sizeof(uint) - 1)) != 0)
            return PatchResult.InvalidAlignment;

        var instructions = GetMethodInstructions(method);
        if (!IsSupportedPrologue(instructions))
            return PatchResult.UnexpectedPrologue;

        var patchCode = MemoryMarshal.Cast<byte, uint>(code);

        int patchSize = patchCode.Length + 1 /* RET */;
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

            // End the method with a RET instruction.
            instructions[patchCode.Length] = Ret;

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
        RuntimeHelpers.PrepareMethod(method.MethodHandle);
        uint* p = SkipBranches((uint*)method.MethodHandle.GetFunctionPointer());

        if (((nuint)p & (sizeof(uint) - 1)) != 0)
            return [];

        var bytes = Unwind.GetMethodInstructions((byte*)p);
        return MemoryMarshal.Cast<byte, uint>(bytes);
    }
}
