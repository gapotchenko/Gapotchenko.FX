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

        var instructions = GetMethodInstructions(method, out var entryPoint, out var entryPointTarget);
        int patchablePrologueSize = GetPatchablePrologueSize(instructions);
        if (patchablePrologueSize < 0)
            return PatchResult.UnexpectedPrologue;

        instructions = instructions[..Math.Min(patchablePrologueSize, instructions.Length)];

        var patchCode = MemoryMarshal.Cast<byte, ushort>(code);

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
            result = ApplyPatch(instructions, entryPoint, entryPointTarget, patchCode);
        }
        return result;
    }

    static int GetPatchablePrologueSize(ReadOnlySpan<ushort> instructions)
    {
        if (instructions.Length < 1)
            return -1;

        ushort instruction = instructions[0];
        if ((instruction & 0xff00) == 0xb500) // PUSH {..., LR}
            return 2; // PUSH, POP {..., PC}
        if (instruction == 0xe92d && instructions.Length >= 2 && (instructions[1] & 0x4000) != 0) // PUSH.W {..., LR}
            return 4; // PUSH.W, POP.W {..., PC}
        if ((instruction & 0xff80) == 0xb080) // SUB SP, #imm
            return 3; // SUB SP, ADD SP, BX LR
        return -1;
    }

    static unsafe Span<ushort> GetMethodInstructions(
        MethodInfo method,
        out Span<ushort> entryPoint,
        out Span<uint> entryPointTarget)
    {
        RuntimeHelpers.PrepareMethod(method.MethodHandle);
        ushort* p0 = (ushort*)((nuint)(void*)method.MethodHandle.GetFunctionPointer() & ~(nuint)1);
        ushort* p = SkipBranches(p0);
        entryPoint = p0 != p ? new(p0, 2) : [];
        entryPointTarget =
            p0 != p && TryGetIndirectBranchTargetSlot(p0, out uint* targetSlot) ?
            new(targetSlot, 1) :
            [];

        if (((nuint)p & (sizeof(ushort) - 1)) != 0)
            return [];

        var bytes = Unwind.GetMethodInstructions((byte*)p);
        return MemoryMarshal.Cast<byte, ushort>(bytes);
    }

    static unsafe PatchResult ApplyPatch(
        Span<ushort> instructions,
        Span<ushort> entryPoint,
        Span<uint> entryPointTarget,
        ReadOnlySpan<ushort> code)
    {
        int patchSize = checked(code.Length + 1 /* BX LR */);

        var trampoline = Span<ushort>.Empty;
        ushort branchFirst = 0, branchSecond = 0;
        if (patchSize > instructions.Length)
        {
            var redirection = entryPoint.IsEmpty ? instructions[..Math.Min(2, instructions.Length)] : entryPoint;
            if (entryPointTarget.IsEmpty)
            {
                if (redirection.Length < 2)
                    return PatchResult.NoSpace;

                ref ushort instruction = ref MemoryMarshal.GetReference(instructions);
                void* target = Unsafe.AsPointer(ref instruction);
                if (!TrampolineAllocator.TryAllocateNear(
                    target,
                    patchSize,
                    (nuint)BranchMaximumDistance,
                    out trampoline))
                {
                    return PatchResult.NoSpace;
                }

                nint offset = checked(Unsafe.ByteOffset(
                    ref MemoryMarshal.GetReference(redirection),
                    ref MemoryMarshal.GetReference(trampoline)) - 4);
                if (!TryEncodeBranch(offset, out branchFirst, out branchSecond))
                    return PatchResult.NoSpace;
            }
            else
            {
                trampoline = MemoryMarshal.Cast<byte, ushort>(
                    TrampolineAllocator.Allocate(checked(patchSize * sizeof(ushort))));
            }

            using (var trampolineScope = MemoryProtectionScope.Create(
                trampoline,
                NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute))
            {
                code.CopyTo(trampoline);
                trampoline[code.Length] = BxLr;
                trampolineScope.FlushInstructions();
            }

            if (entryPointTarget.IsEmpty)
            {
                instructions = redirection;
            }
            else
            {
                using var targetScope = MemoryProtectionScope.Create(
                    entryPointTarget,
                    NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute);
                entryPointTarget[0] = (uint)(nuint)Unsafe.AsPointer(ref MemoryMarshal.GetReference(trampoline)) | 1;
                return PatchResult.Success;
            }
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
            instructions[0] = branchFirst;
            instructions[1] = branchSecond;
        }
        else
        {
            code.CopyTo(instructions);
            instructions[code.Length] = BxLr;
        }

        scope.FlushInstructions();
        return PatchResult.Success;
    }
}
