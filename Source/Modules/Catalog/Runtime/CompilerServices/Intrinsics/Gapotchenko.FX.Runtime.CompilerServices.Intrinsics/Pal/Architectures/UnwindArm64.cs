// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

/// <summary>
/// Analyzes ARM64 unwind prologues and epilogues.
/// </summary>
static class UnwindArm64
{
    public static UnwindAnalysisResult Analyze(ReadOnlySpan<uint> code, out UnwindInfo info)
    {
        info = default;
        if (code.IsEmpty)
            return UnwindAnalysisResult.Leaf;

        FrameKind frameKind;
        int frameSize;
        int prologueInstructionCount;

        if (TryDecodeStackAllocation(code[0], out frameSize))
        {
            frameKind = FrameKind.StackAllocation;
            prologueInstructionCount = 1;
            if (code.Length < 2 || !IsStackDeallocation(code[^1], frameSize))
                return UnwindAnalysisResult.Unsupported;
        }
        else if (TryDecodeFpLrSave(code[0], out frameSize))
        {
            frameKind = FrameKind.FrameChain;
            prologueInstructionCount = 2;
            if (code.Length < 3 ||
                code[1] != InstructionsArm64.MovFpSp ||
                !IsFpLrRestore(code[^1], frameSize))
            {
                return UnwindAnalysisResult.Unsupported;
            }
        }
        else
        {
            return IsPotentialPrologueInstruction(code[0]) ?
                UnwindAnalysisResult.Unsupported :
                UnwindAnalysisResult.Leaf;
        }

        if (frameSize == 0 || frameSize % StackAlignment != 0)
            return UnwindAnalysisResult.Unsupported;

        info = new(frameKind, frameSize, prologueInstructionCount);
        return UnwindAnalysisResult.Supported;
    }

    static bool TryDecodeStackAllocation(uint instruction, out int frameSize)
    {
        if ((instruction & InstructionsArm64.AddSubSpSpImmediateMask) != InstructionsArm64.SubSpSpImmediate)
        {
            frameSize = 0;
            return false;
        }

        frameSize = InstructionsArm64.DecodeImmediate12(instruction);
        return true;
    }

    static bool IsStackDeallocation(uint instruction, int frameSize) =>
        (instruction & InstructionsArm64.AddSubSpSpImmediateMask) == InstructionsArm64.AddSpSpImmediate &&
        InstructionsArm64.DecodeImmediate12(instruction) == frameSize;

    static bool TryDecodeFpLrSave(uint instruction, out int frameSize)
    {
        if ((instruction & InstructionsArm64.StpFpLrPreIndexMask) != InstructionsArm64.StpFpLrPreIndex)
        {
            frameSize = 0;
            return false;
        }

        int offset = InstructionsArm64.DecodeSignedImmediate7Scaled8(instruction);
        frameSize = -offset;
        return offset < 0;
    }

    static bool IsFpLrRestore(uint instruction, int frameSize) =>
        (instruction & InstructionsArm64.LdpFpLrPostIndexMask) == InstructionsArm64.LdpFpLrPostIndex &&
        InstructionsArm64.DecodeSignedImmediate7Scaled8(instruction) == frameSize;

    static bool IsPotentialPrologueInstruction(uint instruction) =>
        (instruction & InstructionsArm64.AddSubSpSpImmediateMask) == InstructionsArm64.AddSpSpImmediate ||
        (instruction & InstructionsArm64.StpRegistersPreIndexSpMask) == InstructionsArm64.StpRegistersPreIndexSp ||
        instruction is InstructionsArm64.MovFpSp or InstructionsArm64.Pacibsp;

    public readonly record struct UnwindInfo(
        FrameKind FrameKind,
        int FrameSize,
        int PrologueInstructionCount);

    public enum FrameKind
    {
        StackAllocation,
        FrameChain
    }

    public const int StackAlignment = 16;
}
