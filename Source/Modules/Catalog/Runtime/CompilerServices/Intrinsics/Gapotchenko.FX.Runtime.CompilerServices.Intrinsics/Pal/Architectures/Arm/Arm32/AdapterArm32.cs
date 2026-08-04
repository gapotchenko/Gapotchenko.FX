// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures.Arm.Arm32;

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures.Arm;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

abstract class AdapterArm32 : AdapterArm
{
    public sealed override IntrinsicCapabilities GetCapabilities()
    {
        return IntrinsicCapabilities.FeatureSupport;
    }

    public override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        if ((code.Length & (sizeof(ushort) - 1)) != 0)
            return PatchResult.InvalidAlignment;

        var patchCode = MemoryMarshal.Cast<byte, ushort>(code);
        var codeValidationResult = ValidateCode(patchCode);
        if (codeValidationResult != PatchResult.Success)
            return codeValidationResult;

        GetMethodInstructions(method, out var instructions, out var entryPoint, out var entryPointTarget);
        int prologueSize = GetPatchablePrologueSize(instructions);
        if (prologueSize < 0)
            return PatchResult.UnexpectedPrologue;

        instructions = instructions[..Math.Min(prologueSize, instructions.Length)];
        PatchResult result;
#if TFF_CER
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

    protected abstract void GetMethodInstructions(
        MethodInfo method,
        out Span<ushort> instructions,
        out Span<ushort> entryPoint,
        out Span<uint> entryPointTarget);

    protected static unsafe ushort* GetMethodCodePointer(
        MethodInfo method,
        out Span<ushort> entryPoint,
        out Span<uint> entryPointTarget)
    {
        RuntimeHelpers.PrepareMethod(method.MethodHandle);
        ushort* p0 = (ushort*)((nuint)(void*)method.MethodHandle.GetFunctionPointer() & ~(nuint)1);
        ushort* p = SkipBranches(p0);

        entryPoint = [];
        entryPointTarget = [];
        if (p0 != p)
        {
            if (TryGetIndirectBranchTargetSlot(p0, out uint* targetSlot))
                entryPointTarget = new(targetSlot, 1);
            else
                entryPoint = new(p0, 2);
        }
        return p;
    }

    static int GetPatchablePrologueSize(ReadOnlySpan<ushort> instructions)
    {
        if (instructions.Length < 1)
            return -1;

        ushort instruction = instructions[0];
        if (InstructionsArm32.IsPushRegistersWithLinkRegister(instruction))
            return 2;
        if (InstructionsArm32.IsPushRegistersWideWithLinkRegister(instructions))
            return 4;
        if (InstructionsArm32.IsStackAllocation(instruction))
            return 3;
        return -1;
    }

    unsafe PatchResult ApplyPatch(
        Span<ushort> instructions,
        Span<ushort> entryPoint,
        Span<uint> entryPointTarget,
        ReadOnlySpan<ushort> code)
    {
        int patchSize = checked(code.Length + 1);
        if (patchSize <= instructions.Length)
        {
            var destination = instructions[..patchSize];
            if (!IsWriteAllowed(destination))
                return PatchResult.WriteProtected;
            WriteCode(destination, code);
            return PatchResult.Success;
        }

        var bodyRedirection = instructions[..2];
        if (!IsWriteAllowed(bodyRedirection) ||
            !entryPoint.IsEmpty && !IsWriteAllowed(entryPoint) ||
            !entryPointTarget.IsEmpty && !IsWriteAllowed(entryPointTarget))
        {
            return PatchResult.WriteProtected;
        }

        Span<ushort> trampoline;
        Span<ushort> primaryRedirection;
        if (!entryPointTarget.IsEmpty)
        {
            trampoline = AllocateTrampoline(patchSize);
            primaryRedirection = [];
        }
        else
        {
            primaryRedirection = entryPoint.IsEmpty ? bodyRedirection : entryPoint;
            if (!TryAllocateTrampolineNear(GetPointer(primaryRedirection), patchSize, out trampoline))
                return PatchResult.NoSpace;
        }

        ushort primaryBranchFirst = 0, primaryBranchSecond = 0;
        if (!primaryRedirection.IsEmpty &&
            !TryEncodeBranch(GetBranchOffset(primaryRedirection, trampoline), out primaryBranchFirst, out primaryBranchSecond))
        {
            return PatchResult.NoSpace;
        }

        var bodyTrampoline = Span<ushort>.Empty;
        ushort bodyBranchFirst = primaryBranchFirst, bodyBranchSecond = primaryBranchSecond;
        if (!entryPoint.IsEmpty || !entryPointTarget.IsEmpty)
        {
            if (!TryEncodeBranch(GetBranchOffset(bodyRedirection, trampoline), out bodyBranchFirst, out bodyBranchSecond))
            {
                if (!TryAllocateTrampolineNear(GetPointer(bodyRedirection), patchSize, out bodyTrampoline) ||
                    !TryEncodeBranch(GetBranchOffset(bodyRedirection, bodyTrampoline), out bodyBranchFirst, out bodyBranchSecond))
                {
                    return PatchResult.NoSpace;
                }
            }
        }

        WriteCode(trampoline, code);
        if (!bodyTrampoline.IsEmpty)
            WriteCode(bodyTrampoline, code);

        WriteBranch(bodyRedirection, bodyBranchFirst, bodyBranchSecond);

        if (!entryPoint.IsEmpty)
            WriteBranch(entryPoint, primaryBranchFirst, primaryBranchSecond);
        else if (!entryPointTarget.IsEmpty)
            WriteAddress(entryPointTarget, (uint)(nuint)GetPointer(trampoline) | 1);

        return PatchResult.Success;
    }

    protected abstract bool IsWriteAllowed<T>(Span<T> span) where T : struct;
    protected virtual PatchResult ValidateCode(ReadOnlySpan<ushort> code) => PatchResult.Success;
    protected abstract Span<ushort> AllocateTrampoline(int count);
    protected abstract unsafe bool TryAllocateTrampolineNear(void* target, int count, out Span<ushort> trampoline);
    protected abstract void WriteCode(Span<ushort> destination, ReadOnlySpan<ushort> code);
    protected abstract void WriteBranch(Span<ushort> destination, ushort first, ushort second);
    protected abstract void WriteAddress(Span<uint> destination, uint address);

    static unsafe void* GetPointer(Span<ushort> span) =>
        Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));

    static nint GetBranchOffset(Span<ushort> source, Span<ushort> target) =>
        checked(Unsafe.ByteOffset(ref MemoryMarshal.GetReference(source), ref MemoryMarshal.GetReference(target)) - sizeof(uint));

    protected static unsafe ushort* SkipBranches(ushort* p)
    {
        // Function pointers for Thumb code have bit 0 set.
        p = (ushort*)((nuint)p & ~(nuint)1);

        for (; ; )
        {
            // LDR.W PC, [PC, #imm12]
            if (TryGetIndirectBranchTargetSlot(p, out uint* targetSlot))
            {
                p = (ushort*)(nuint)(*targetSlot);
                p = (ushort*)((nuint)p & ~(nuint)1);
            }
            // B label: the signed imm11 operand is measured in two-byte instructions.
            else if ((p[0] & 0xf800) == 0xe000)
            {
                int displacement = ((p[0] & 0x07ff) << 21 >> 20) + 4;
                p = (ushort*)((byte*)p + displacement);
            }
            else
            {
                return p;
            }
        }
    }

    protected static unsafe bool TryGetIndirectBranchTargetSlot(ushort* p, out uint* targetSlot)
    {
        if (p[0] == 0xf8df && (p[1] & 0xf000) == 0xf000)
        {
            byte* pc = (byte*)(((nuint)p + 4) & ~(nuint)3);
            targetSlot = (uint*)(pc + (p[1] & 0x0fff));
            return true;
        }

        targetSlot = null;
        return false;
    }

    protected static bool TryEncodeBranch(nint offset, out ushort first, out ushort second)
    {
        if ((offset & 1) != 0 || offset < -InstructionsArm32.BranchMaximumDistance || offset >= InstructionsArm32.BranchMaximumDistance)
        {
            first = second = 0;
            return false;
        }

        uint displacement = (uint)offset;
        uint s = displacement >> 24 & 1;
        uint i1 = displacement >> 23 & 1;
        uint i2 = displacement >> 22 & 1;
        uint j1 = ~(i1 ^ s) & 1;
        uint j2 = ~(i2 ^ s) & 1;

        first = (ushort)(0xf000 | s << 10 | displacement >> 12 & 0x03ff);
        second = (ushort)(0x9000 | j1 << 13 | j2 << 11 | displacement >> 1 & 0x07ff);
        return true;
    }
}
