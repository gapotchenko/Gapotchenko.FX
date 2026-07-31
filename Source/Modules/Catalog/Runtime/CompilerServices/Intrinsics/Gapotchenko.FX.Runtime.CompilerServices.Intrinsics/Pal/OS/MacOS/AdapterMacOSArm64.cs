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

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.MacOS;

#if NET
[SupportedOSPlatform("macos")]
#endif
sealed class AdapterMacOSArm64 : AdapterArm64
{
    public override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        // Every ARM64 instruction is four bytes long.
        if ((code.Length & (sizeof(uint) - 1)) != 0)
            return PatchResult.InvalidAlignment;

        var instructions = GetMethodInstructions(method, out var entryPoint);
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
            result = ApplyPatch(instructions, entryPoint, patchCode);
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

    static unsafe Span<uint> GetMethodInstructions(MethodInfo method, out Span<uint> entryPoint)
    {
        RuntimeHelpers.PrepareMethod(method.MethodHandle);
        uint* p0 = (uint*)method.MethodHandle.GetFunctionPointer();
        uint* p = SkipBranches(p0);
        entryPoint = p0 != p ? new(p0, 1) : [];

        if (((nuint)p & (sizeof(uint) - 1)) != 0)
            return [];

        var bytes = Unwind.GetMethodInstructions((byte*)p);
        return MemoryMarshal.Cast<byte, uint>(bytes);
    }

    static unsafe PatchResult ApplyPatch(Span<uint> instructions, Span<uint> entryPoint, ReadOnlySpan<uint> code)
    {
        int patchSize = checked(code.Length + 1 /* RET */);

        var trampoline = Span<uint>.Empty;
        uint branchDisplacement = 0;
        if (patchSize > instructions.Length)
        {
            var redirection = entryPoint.IsEmpty ? instructions[..1] : entryPoint;
            if (!MemoryMap.IsWriteAllowed(redirection))
                return PatchResult.WriteProtected;

            ref uint instruction = ref MemoryMarshal.GetReference(redirection);
            void* target = Unsafe.AsPointer(ref instruction);

            if (!TrampolineAllocator.TryAllocateNear(target, patchSize, (nuint)BranchMaximumDistance, out trampoline))
                return PatchResult.NoSpace;

            nint offset = Unsafe.ByteOffset(ref instruction, ref MemoryMarshal.GetReference(trampoline));
            if (!TryEncodeBranch(offset, out branchDisplacement))
                return PatchResult.NoSpace;

            var bodyTrampoline = Span<uint>.Empty;
            var bodyRedirection = Span<uint>.Empty;
            uint bodyBranchDisplacement = 0;

            if (!entryPoint.IsEmpty)
            {
                bodyRedirection = instructions[..1];
                if (!MemoryMap.IsWriteAllowed(bodyRedirection))
                    return PatchResult.WriteProtected;

                nint bodyOffset = Unsafe.ByteOffset(
                    ref MemoryMarshal.GetReference(bodyRedirection),
                    ref MemoryMarshal.GetReference(trampoline));
                if (!TryEncodeBranch(bodyOffset, out bodyBranchDisplacement))
                {
                    ref uint bodyInstruction = ref MemoryMarshal.GetReference(bodyRedirection);
                    if (!TrampolineAllocator.TryAllocateNear(
                        Unsafe.AsPointer(ref bodyInstruction),
                        patchSize,
                        (nuint)BranchMaximumDistance,
                        out bodyTrampoline))
                    {
                        return PatchResult.NoSpace;
                    }

                    bodyOffset = Unsafe.ByteOffset(
                        ref bodyInstruction,
                        ref MemoryMarshal.GetReference(bodyTrampoline));
                    if (!TryEncodeBranch(bodyOffset, out bodyBranchDisplacement))
                        return PatchResult.NoSpace;
                }
            }

            WriteTrampoline(trampoline, code);
            if (!bodyTrampoline.IsEmpty)
                WriteTrampoline(bodyTrampoline, code);

            if (!bodyRedirection.IsEmpty)
            {
                using var bodyScope = JitWriteProtectionScope.Create(bodyRedirection);
                bodyRedirection[0] = B | bodyBranchDisplacement;
                bodyScope.FlushInstructions();
            }

            instructions = redirection;
        }
        else
        {
            instructions = instructions[..patchSize];
            if (!MemoryMap.IsWriteAllowed(instructions))
                return PatchResult.WriteProtected;
        }

        using var scope = JitWriteProtectionScope.Create(instructions);

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

        static void WriteTrampoline(Span<uint> destination, ReadOnlySpan<uint> code)
        {
            using var scope = JitWriteProtectionScope.Create(destination);
            code.CopyTo(destination);
            destination[code.Length] = Ret;
            scope.FlushInstructions();
        }
    }
}
