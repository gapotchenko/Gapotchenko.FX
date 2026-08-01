// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

abstract class AdapterArm64 : AdapterArm
{
    public sealed override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        if ((code.Length & (sizeof(uint) - 1)) != 0)
            return PatchResult.InvalidAlignment;

        var patchCode = MemoryMarshal.Cast<byte, uint>(code);
        var codeValidationResult = ValidateCode(patchCode);
        if (codeValidationResult != PatchResult.Success)
            return codeValidationResult;

        GetMethodInstructions(
            method,
            out var instructions,
            out var entryPoint,
            out var entryPointTarget,
            out bool hasExactBoundaries);
        int prologueSize = GetPatchablePrologueSize(instructions);
        if (prologueSize < 0)
            return PatchResult.UnexpectedPrologue;

        if (!hasExactBoundaries)
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
        out Span<uint> instructions,
        out Span<uint> entryPoint,
        out Span<nuint> entryPointTarget,
        out bool hasExactBoundaries);

    protected static unsafe uint* GetMethodCodePointer(
        MethodInfo method,
        out Span<uint> entryPoint,
        out Span<nuint> entryPointTarget)
    {
        RuntimeHelpers.PrepareMethod(method.MethodHandle);
        uint* p0 = (uint*)method.MethodHandle.GetFunctionPointer();
        uint* p = SkipBranches(p0);

        entryPoint = [];
        entryPointTarget = [];
        if (p0 != p)
        {
            if (TryGetIndirectBranchTargetSlot(p0, out nuint* targetSlot))
                entryPointTarget = new(targetSlot, 1);
            else
                entryPoint = new(p0, 1);
        }
        return p;
    }

    static int GetPatchablePrologueSize(ReadOnlySpan<uint> instructions)
    {
        if (instructions.Length < 1)
            return -1;

        uint instruction = instructions[0];
        bool supported =
            (instruction & 0xffc07fff) == 0xa9807bfd ||
            (instruction & 0xff8003ff) == 0xd10003ff;
        return supported ? 3 : -1;
    }

    unsafe PatchResult ApplyPatch(
        Span<uint> instructions,
        Span<uint> entryPoint,
        Span<nuint> entryPointTarget,
        ReadOnlySpan<uint> code)
    {
        int patchSize = checked(code.Length + 1);
        if (!RequiresTrampoline(code) && patchSize <= instructions.Length)
        {
            var destination = instructions[..patchSize];
            if (!IsWriteAllowed(destination))
                return PatchResult.WriteProtected;
            WriteCode(destination, code);
            return PatchResult.Success;
        }

        var bodyRedirection = instructions[..1];
        if (!IsWriteAllowed(bodyRedirection) ||
            !entryPoint.IsEmpty && !IsWriteAllowed(entryPoint) ||
            !entryPointTarget.IsEmpty && !IsWriteAllowed(entryPointTarget))
        {
            return PatchResult.WriteProtected;
        }

        Span<uint> trampoline;
        Span<uint> primaryRedirection;
        if (!entryPointTarget.IsEmpty)
        {
            trampoline = AllocateTrampoline(GetTrampolineAllocationCount(code));
            primaryRedirection = [];
        }
        else
        {
            primaryRedirection = entryPoint.IsEmpty ? bodyRedirection : entryPoint;
            if (!TryAllocateTrampolineNear(
                GetPointer(primaryRedirection),
                GetTrampolineAllocationCount(code),
                (nuint)BranchMaximumDistance,
                out trampoline))
            {
                return ApplyLongPatch(instructions, entryPoint, entryPointTarget, code);
            }
        }

        uint primaryBranch = 0;
        if (!primaryRedirection.IsEmpty && !TryEncodeBranch(GetOffset(primaryRedirection, trampoline), out primaryBranch))
            return ApplyLongPatch(instructions, entryPoint, entryPointTarget, code);

        var bodyTrampoline = Span<uint>.Empty;
        uint bodyBranch = primaryBranch;
        if (!entryPoint.IsEmpty || !entryPointTarget.IsEmpty)
        {
            if (!TryEncodeBranch(GetOffset(bodyRedirection, trampoline), out bodyBranch))
            {
                if (!TryAllocateTrampolineNear(
                        GetPointer(bodyRedirection),
                        GetTrampolineAllocationCount(code),
                        (nuint)BranchMaximumDistance,
                        out bodyTrampoline) ||
                    !TryEncodeBranch(GetOffset(bodyRedirection, bodyTrampoline), out bodyBranch))
                {
                    return ApplyLongPatch(instructions, entryPoint, entryPointTarget, code);
                }
            }
        }

        WriteTrampoline(trampoline, code);
        if (!bodyTrampoline.IsEmpty)
            WriteTrampoline(bodyTrampoline, code);

        // Patch the body before the entry point. This keeps a return into an active
        // frame valid when compilation was initiated by that frame's type initializer.
        WriteBranch(bodyRedirection, bodyBranch);

        if (!entryPoint.IsEmpty)
            WriteBranch(entryPoint, primaryBranch);
        else if (!entryPointTarget.IsEmpty)
            WriteAddress(entryPointTarget, (nuint)GetPointer(trampoline));

        return PatchResult.Success;
    }

    unsafe PatchResult ApplyLongPatch(
        Span<uint> instructions,
        Span<uint> entryPoint,
        Span<nuint> entryPointTarget,
        ReadOnlySpan<uint> code)
    {
        var bodyRedirection = instructions[..LongBranchInstructionCount];
        if (!IsWriteAllowed(bodyRedirection) ||
            !entryPoint.IsEmpty && !IsWriteAllowed(entryPoint) ||
            !entryPointTarget.IsEmpty && !IsWriteAllowed(entryPointTarget))
        {
            return PatchResult.WriteProtected;
        }

        int patchSize = checked(code.Length + 1);
        if (!TryAllocateTrampolineNear(
                GetPointer(bodyRedirection),
                GetTrampolineAllocationCount(code),
                unchecked((nuint)LongBranchMaximumDistance),
                out var trampoline) ||
            !TryEncodeLongBranch(bodyRedirection, trampoline, out uint adrp, out uint add))
        {
            return PatchResult.NoSpace;
        }

        uint entryPointBranch = 0;
        if (!entryPoint.IsEmpty && !TryEncodeBranch(GetOffset(entryPoint, bodyRedirection), out entryPointBranch))
            return PatchResult.NoSpace;

        WriteTrampoline(trampoline, code);
        WriteLongBranch(bodyRedirection, adrp, add);

        if (!entryPoint.IsEmpty)
            WriteBranch(entryPoint, entryPointBranch);
        else if (!entryPointTarget.IsEmpty)
            WriteAddress(entryPointTarget, (nuint)GetPointer(trampoline));

        return PatchResult.Success;
    }

    protected abstract bool IsWriteAllowed<T>(Span<T> span) where T : struct;
    protected virtual PatchResult ValidateCode(ReadOnlySpan<uint> code) => PatchResult.Success;
    protected virtual bool RequiresTrampoline(ReadOnlySpan<uint> code) => false;
    protected virtual int GetTrampolineAllocationCount(ReadOnlySpan<uint> code) =>
        checked(code.Length + 1);
    protected abstract Span<uint> AllocateTrampoline(int count);
    protected abstract unsafe bool TryAllocateTrampolineNear(
        void* target,
        int count,
        nuint maximumDistance,
        out Span<uint> trampoline);
    protected abstract void WriteCode(Span<uint> destination, ReadOnlySpan<uint> code);
    protected virtual void WriteTrampoline(Span<uint> destination, ReadOnlySpan<uint> code) =>
        WriteCode(destination, code);
    protected abstract void WriteBranch(Span<uint> destination, uint displacement);
    protected abstract void WriteLongBranch(Span<uint> destination, uint adrp, uint add);
    protected abstract void WriteAddress(Span<nuint> destination, nuint address);

    static unsafe void* GetPointer(Span<uint> span) =>
        Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));

    static nint GetOffset(Span<uint> source, Span<uint> target) =>
        Unsafe.ByteOffset(ref MemoryMarshal.GetReference(source), ref MemoryMarshal.GetReference(target));

    protected static unsafe uint* SkipBranches(uint* p)
    {
        for (; ; )
        {
            // B label: the signed imm26 operand is measured in four-byte instructions.
            if ((*p & 0xfc000000) == 0x14000000)
            {
                // Sign-extend the operand and scale it by four to obtain a byte displacement in one go.
                int displacement = (int)(*p << 6) >> 4;
                p = (uint*)((byte*)p + displacement);
            }
            // LDR Xt, label; BR Xt
            else if (TryGetIndirectBranchTargetSlot(p, out nuint* targetSlot))
            {
                p = (uint*)*targetSlot;
            }
            else
            {
                return p;
            }
        }
    }

    protected static unsafe bool TryGetIndirectBranchTargetSlot(uint* p, out nuint* targetSlot)
    {
        if (
            (p[0] & 0xff000000) == 0x58000000 &&
            (p[1] & 0xfffffc1f) == 0xd61f0000 &&
            (p[0] & 0x1f) == ((p[1] >> 5) & 0x1f))
        {
            // Sign-extend the imm19 operand and scale it by four.
            int displacement = ((int)((p[0] >> 5) & 0x7ffff) << 13 >> 13) << 2;
            targetSlot = (nuint*)((byte*)p + displacement);
            return true;
        }

        targetSlot = null;
        return false;
    }

    protected const uint Ret = 0xd65f03c0;
    protected const uint B = 0x14000000;
    protected const uint BrX16 = 0xd61f0200;

    protected static bool TryEncodeBranch(nint offset, out uint displacement)
    {
        if ((offset & (sizeof(uint) - 1)) != 0 || offset < -BranchMaximumDistance || offset >= BranchMaximumDistance)
        {
            displacement = 0;
            return false;
        }

        displacement = (uint)(offset >> 2) & 0x03ffffff;
        return true;
    }

    protected const nint BranchMaximumDistance = 1 << 27;

    static unsafe bool TryEncodeLongBranch(
        Span<uint> source,
        Span<uint> target,
        out uint adrp,
        out uint add)
    {
        nuint sourceAddress = (nuint)GetPointer(source);
        nuint targetAddress = (nuint)GetPointer(target);
        nint pageOffset = (nint)(targetAddress & ~(nuint)0xfff) - (nint)(sourceAddress & ~(nuint)0xfff);
        nint pageDisplacement = pageOffset >> 12;
        if (pageDisplacement < -(1 << 20) || pageDisplacement >= 1 << 20)
        {
            adrp = 0;
            add = 0;
            return false;
        }

        uint immediate = (uint)pageDisplacement & 0x1fffff;
        adrp = 0x90000010 | (immediate & 3) << 29 | (immediate >> 2) << 5;
        add = 0x91000210 | (uint)(targetAddress & 0xfff) << 10;
        return true;
    }

    const int LongBranchInstructionCount = 3;

    const ulong LongBranchMaximumDistance = 1UL << 32;
}
