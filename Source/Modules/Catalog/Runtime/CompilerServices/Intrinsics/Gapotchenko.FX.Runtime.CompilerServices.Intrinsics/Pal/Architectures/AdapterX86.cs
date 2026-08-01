// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Reflection;
using Gapotchenko.FX.Runtime.CompilerServices.Utils;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

abstract class AdapterX86 : AdapterX86Base
{
    public sealed override PatchResult PatchMethod(MethodInfo method, ReadOnlySpan<byte> code)
    {
        GetMethodInstructions(method, out var instructions, out bool hasExactBoundaries);
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
            result = ApplyPatch(instructions, code);
        }
        return result;
    }

    protected abstract void GetMethodInstructions(
        MethodInfo method,
        out Span<byte> instructions,
        out bool hasExactBoundaries);

    PatchResult ApplyPatch(Span<byte> instructions, ReadOnlySpan<byte> code)
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

        if (instructions.Length < InstructionsX86.JmpRel32Size)
            return PatchResult.NoSpace;

        var redirection = instructions[..InstructionsX86.JmpRel32Size];
        if (!IsWriteAllowed(redirection))
            return PatchResult.WriteProtected;

        var trampoline = AllocateTrampoline(patchSize);
        WriteCode(trampoline, code);
        WriteRedirection(redirection, trampoline);
        return PatchResult.Success;
    }

    static int GetPatchablePrologueSize(ReadOnlySpan<byte> instructions)
    {
        // Prologue-less leaf method: MOV EAX,imm32; RET.
        if (instructions.Length >= 6 && instructions[0] == 0xb8 && instructions[5] == InstructionsX86.Ret)
            return 6;

        return InstructionOperations.GetPatchablePrologueSize(instructions, m_SupportedPrologues);
    }

    static readonly (byte[] Instructions, int Size)[] m_SupportedPrologues =
    [
        ([0x8b, 0xff, 0x55, 0x8b, 0xec], 5 + 2), // MOV EDI,EDI; PUSH EBP; MOV EBP,ESP
        ([0x55, 0x8b, 0xec], 3 + 2),             // PUSH EBP; MOV EBP,ESP
        ([0x55, 0x89, 0xe5], 3 + 2),             // PUSH EBP; MOV EBP,ESP
        ([0x53, 0x56, 0x57], 3 + 4),             // PUSH EBX; PUSH ESI; PUSH EDI
        ([0x53, 0x83, 0xec], 4 + 5),             // PUSH EBX; SUB ESP,imm8
        ([0x56, 0x83, 0xec], 4 + 5),             // PUSH ESI; SUB ESP,imm8
        ([0x57, 0x83, 0xec], 4 + 5),             // PUSH EDI; SUB ESP,imm8
        ([0x83, 0xec], 3 + 4),                   // SUB ESP,imm8
        ([0x81, 0xec], 6 + 7)                    // SUB ESP,imm32
    ];

    protected abstract bool IsWriteAllowed(Span<byte> span);
    protected abstract Span<byte> AllocateTrampoline(int size);
    protected abstract void WriteCode(Span<byte> destination, ReadOnlySpan<byte> code);
    protected abstract void WriteRedirection(Span<byte> destination, Span<byte> trampoline);
}
