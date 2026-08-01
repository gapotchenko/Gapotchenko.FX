// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

using System.Reflection;
using System.Runtime.CompilerServices;
using Gapotchenko.FX.Runtime.CompilerServices.Utils;

abstract class AdapterX64 : AdapterX86Base
{
    public sealed override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        var codeValidationResult = ValidateCode(code);
        if (codeValidationResult != PatchResult.Success)
            return codeValidationResult;

        GetMethodInstructions(method, out var instructions, out var entryPoint, out bool hasExactBoundaries);
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
            result = ApplyPatch(instructions, entryPoint, code);
        }
        return result;
    }

    protected abstract void GetMethodInstructions(
        MethodInfo method,
        out Span<byte> instructions,
        out Span<byte> entryPoint,
        out bool hasExactBoundaries);

    protected static unsafe byte* GetMethodCodePointer(
        MethodInfo method,
        bool useEntryPoint,
        out Span<byte> entryPoint)
    {
        RuntimeHelpers.PrepareMethod(method.MethodHandle);
        byte* p0 = (byte*)method.MethodHandle.GetFunctionPointer();
        byte* p = SkipBranches(p0);

        // .NET x64 uses a 20-byte precode beginning with an indirect jump,
        // followed by MOV R10,[RIP+disp32]. It safely accommodates JmpAbs64.
        if (useEntryPoint && p0 != p &&
            p0[0] == 0xff && p0[1] == 0x25 &&
            p0[6] == 0x4c && p0[7] == 0x8b && p0[8] == 0x15)
        {
            entryPoint = new(p0, InstructionsX64.JmpAbs64Size);
        }
        else
        {
            entryPoint = [];
        }

        return p;
    }

    PatchResult ApplyPatch(Span<byte> instructions, Span<byte> entryPoint, ReadOnlySpan<byte> code)
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

        var redirection = entryPoint.IsEmpty ? instructions : entryPoint;
        int redirectionSize = RedirectionSize;
        if (redirection.Length < redirectionSize)
            return PatchResult.NoSpace;

        redirection = redirection[..redirectionSize];
        if (!IsWriteAllowed(redirection))
            return PatchResult.WriteProtected;

        var bodyRedirection = Span<byte>.Empty;
        int bodyRedirectionSize = BodyRedirectionSize;
        if (!entryPoint.IsEmpty && bodyRedirectionSize != 0)
        {
            if (instructions.Length < bodyRedirectionSize)
                return PatchResult.NoSpace;
            bodyRedirection = instructions[..bodyRedirectionSize];
            if (!IsWriteAllowed(bodyRedirection))
                return PatchResult.WriteProtected;
        }

        if (!TryAllocateTrampoline(redirection, GetTrampolineAllocationSize(code), out var trampoline))
            return PatchResult.NoSpace;

        WriteTrampoline(trampoline, code);
        WriteRedirection(redirection, trampoline);
        if (!bodyRedirection.IsEmpty)
            WriteBodyRedirection(bodyRedirection, redirection);
        return PatchResult.Success;
    }

    static int GetPatchablePrologueSize(ReadOnlySpan<byte> instructions) =>
        InstructionOperations.GetPatchablePrologueSize(instructions, m_SupportedPrologues);

    // The table defines a conservative minimum method body extent:
    // complete prologue plus the shortest matching restoration/return sequence.
    static readonly (byte[] Instructions, int Size)[] m_SupportedPrologues =
    [
        ([0x48, 0x83, 0xec, 0x18, 0x48, 0x89, 0x34, 0x24], 8 + 9),
        ([0x48, 0x89, 0x54, 0x24], 5 + 1),
        ([0x55, 0x57, 0x56, 0x48, 0x83, 0xec, 0x30], 7 + 8),
        ([0x57, 0x56, 0x48, 0x83, 0xec, 0x28], 6 + 7),
        ([0x41, 0x57, 0x53, 0x48, 0x83, 0xec], 7 + 8),
        ([0x48, 0x81, 0xec], 7 + 8),
        ([0x48, 0x83, 0xec], 4 + 5),
        ([0x53, 0x48, 0x83, 0xec], 5 + 6),
        ([0x50, 0x48, 0x8d, 0x05], 1 + 7),
        ([0x50, 0x48, 0xbf], 1 + 5),
        ([0x50, 0xf6, 0x05], 1 + 5),
        ([0x55, 0x41, 0x57], 3 + 4),
        ([0x55, 0x48, 0x83, 0xec], 5 + 6),
        ([0x55, 0x48, 0x89, 0xe5], 4 + 2),
        ([0x55, 0x48, 0x8b, 0xec], 4 + 2),
        ([0x55, 0x53, 0x50, 0x48, 0x8d, 0x6c, 0x24, 0x10], 8 + 7),
        ([0x56, 0x48, 0x83, 0xec], 5 + 6),
        ([0x57, 0x48, 0x83, 0xec], 5 + 6)
    ];

    protected abstract bool IsWriteAllowed(Span<byte> span);

    protected virtual PatchResult ValidateCode(ReadOnlySpan<byte> code) => PatchResult.Success;

    protected virtual bool RequiresTrampoline(ReadOnlySpan<byte> code) => false;

    protected virtual int GetTrampolineAllocationSize(ReadOnlySpan<byte> code) =>
        checked(code.Length + 1);

    protected abstract bool TryAllocateTrampoline(Span<byte> redirection, int size, out Span<byte> trampoline);
    protected abstract void WriteCode(Span<byte> destination, ReadOnlySpan<byte> code);
    protected abstract void WriteTrampoline(Span<byte> destination, ReadOnlySpan<byte> code);

    protected abstract int RedirectionSize { get; }

    protected abstract void WriteRedirection(Span<byte> destination, Span<byte> trampoline);

    protected virtual int BodyRedirectionSize => 0;

    protected virtual void WriteBodyRedirection(Span<byte> destination, Span<byte> entryPoint) =>
        throw new NotSupportedException();
}
