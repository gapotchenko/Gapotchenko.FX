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
            if (InstructionsArm64.TryGetIndirectBranchTargetSlot(p0, out nuint* targetSlot))
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

        return InstructionsArm64.IsStackFrameSetup(instructions[0]) ? 3 : -1;
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
                (nuint)InstructionsArm64.BranchMaximumDistance,
                out trampoline))
            {
                return ApplyLongPatch(instructions, entryPoint, entryPointTarget, code);
            }
        }

        uint primaryBranch = 0;
        if (!primaryRedirection.IsEmpty &&
            !InstructionsArm64.TryEncodeBranch(GetOffset(primaryRedirection, trampoline), out primaryBranch))
        {
            return ApplyLongPatch(instructions, entryPoint, entryPointTarget, code);
        }

        var bodyTrampoline = Span<uint>.Empty;
        uint bodyBranch = primaryBranch;
        if (!entryPoint.IsEmpty || !entryPointTarget.IsEmpty)
        {
            if (!InstructionsArm64.TryEncodeBranch(GetOffset(bodyRedirection, trampoline), out bodyBranch))
            {
                if (!TryAllocateTrampolineNear(
                        GetPointer(bodyRedirection),
                        GetTrampolineAllocationCount(code),
                        (nuint)InstructionsArm64.BranchMaximumDistance,
                        out bodyTrampoline) ||
                    !InstructionsArm64.TryEncodeBranch(GetOffset(bodyRedirection, bodyTrampoline), out bodyBranch))
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
        var bodyRedirection = instructions[..InstructionsArm64.LongBranchInstructionCount];
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
                unchecked((nuint)InstructionsArm64.LongBranchMaximumDistance),
                out var trampoline) ||
            !InstructionsArm64.TryEncodeLongBranch(
                (nuint)GetPointer(bodyRedirection),
                (nuint)GetPointer(trampoline),
                out uint adrp,
                out uint add))
        {
            return PatchResult.NoSpace;
        }

        uint entryPointBranch = 0;
        if (!entryPoint.IsEmpty &&
            !InstructionsArm64.TryEncodeBranch(GetOffset(entryPoint, bodyRedirection), out entryPointBranch))
        {
            return PatchResult.NoSpace;
        }

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

    static unsafe uint* SkipBranches(uint* p)
    {
        for (; ; )
        {
            if (InstructionsArm64.TryDecodeBranch(*p, out int displacement))
                p = (uint*)((byte*)p + displacement);
            else if (InstructionsArm64.TryGetIndirectBranchTargetSlot(p, out nuint* targetSlot))
                p = (uint*)*targetSlot;
            else
                return p;
        }
    }
}
