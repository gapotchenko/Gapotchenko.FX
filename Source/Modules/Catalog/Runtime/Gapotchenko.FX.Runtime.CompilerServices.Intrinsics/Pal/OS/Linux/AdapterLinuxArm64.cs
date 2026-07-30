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
        int patchablePrologueSize = GetPatchablePrologueSize(instructions);
        if (patchablePrologueSize < 0)
            return PatchResult.UnexpectedPrologue;

        instructions = instructions[..Math.Min(patchablePrologueSize, instructions.Length)];

        var patchCode = MemoryMarshal.Cast<byte, uint>(code);

        PatchResult result;
#if TFF_CER
        // Ensure that code changes are atomic by using the constrained execution region.
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
#endif
        {
            result = ApplyPatch(instructions, patchCode);
        }
        return result;
    }

    static int GetPatchablePrologueSize(ReadOnlySpan<uint> instructions)
    {
        if (instructions.Length < 1)
            return -1;

        uint instruction = instructions[0];
        bool supported =
            (instruction & 0xffc07fff) == 0xa9807bfd || // STP X29, X30, [SP, #-imm]!
            (instruction & 0xff8003ff) == 0xd10003ff;   // SUB SP, SP, #imm{, LSL #12}
        return supported ? 3 : -1; // Prologue, shortest matching restoration, RET
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

    static unsafe PatchResult ApplyPatch(Span<uint> instructions, ReadOnlySpan<uint> code)
    {
        int patchSize = checked(code.Length + 1 /* RET */);

        var trampoline = Span<uint>.Empty;
        uint branchDisplacement = 0;
        if (patchSize > instructions.Length)
        {
            ref uint instruction = ref MemoryMarshal.GetReference(instructions);
            void* target = Unsafe.AsPointer(ref instruction);

            if (!TrampolineAllocator.TryAllocateNear(
                target,
                patchSize,
                (nuint)BranchMaximumDistance,
                out trampoline))
            {
                return PatchResult.NoSpace;
            }

            nint offset = Unsafe.ByteOffset(ref instruction, ref MemoryMarshal.GetReference(trampoline));
            if (!TryEncodeBranch(offset, out branchDisplacement))
                return PatchResult.NoSpace;

            using (var trampolineScope = MemoryProtectionScope.Create(
                trampoline,
                NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute))
            {
                code.CopyTo(trampoline);
                trampoline[code.Length] = Ret;
                trampolineScope.FlushInstructions();
            }

            instructions = instructions[..1];
        }
        else
        {
            instructions = instructions[..patchSize];
        }

        using var scope = MemoryProtectionScope.Create(
            instructions,
            NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute);

        if (!trampoline.IsEmpty)
        {
            instructions[0] = B | branchDisplacement;
        }
        else
        {
            code.CopyTo(instructions);
            instructions[code.Length] = Ret;
        }

        scope.FlushInstructions();
        return PatchResult.Success;
    }
}
