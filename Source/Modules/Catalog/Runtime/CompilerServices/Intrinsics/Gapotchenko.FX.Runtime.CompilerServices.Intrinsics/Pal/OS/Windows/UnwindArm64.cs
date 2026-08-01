// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

/// <summary>
/// Analyzes and encodes packed Windows ARM64 unwind information.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
static class UnwindArm64
{
    public enum AnalysisResult
    {
        Leaf,
        Supported,
        Unsupported
    }

    public static AnalysisResult Analyze(ReadOnlySpan<uint> code, out uint unwindData)
    {
        unwindData = 0;
        if (code.IsEmpty)
            return AnalysisResult.Leaf;

        int frameSize;
        int prologueInstructionCount;
        int chained;

        if (TryDecodeStackAllocation(code[0], out frameSize))
        {
            prologueInstructionCount = 1;
            chained = 0;
            if (code.Length < 2 || !IsStackDeallocation(code[^1], frameSize))
                return AnalysisResult.Unsupported;
        }
        else if (TryDecodeFpLrSave(code[0], out frameSize))
        {
            prologueInstructionCount = 2;
            chained = 3;
            if (code.Length < 3 ||
                code[1] != InstructionsArm64.MovFpSp ||
                !IsFpLrRestore(code[^1], frameSize))
            {
                return AnalysisResult.Unsupported;
            }
        }
        else
        {
            return IsPotentialPrologueInstruction(code[0]) ?
                AnalysisResult.Unsupported :
                AnalysisResult.Leaf;
        }

        int functionLength = checked(code.Length + 1);
        if (functionLength > MaximumFunctionLength ||
            frameSize == 0 || frameSize % StackAlignment != 0 || frameSize > MaximumFrameSize)
        {
            return AnalysisResult.Unsupported;
        }

        // Packed unwind data assumes the canonical prologue starts at the
        // function beginning and its inverse epilogue ends immediately before RET.
        if (prologueInstructionCount >= functionLength)
            return AnalysisResult.Unsupported;

        unwindData = EncodePackedUnwindData(functionLength, frameSize, chained);
        return AnalysisResult.Supported;
    }

    static uint EncodePackedUnwindData(int functionLength, int frameSize, int chained)
    {
        return PackedUnwindFlag |
        (uint)functionLength << FunctionLengthOffset |
        (uint)chained << ChainedOffset |
        (uint)(frameSize / StackAlignment) << FrameSizeOffset;
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

    static bool IsStackDeallocation(uint instruction, int frameSize)
    {
        return
            (instruction & InstructionsArm64.AddSubSpSpImmediateMask) == InstructionsArm64.AddSpSpImmediate &&
            InstructionsArm64.DecodeImmediate12(instruction) == frameSize;
    }

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

    static bool IsFpLrRestore(uint instruction, int frameSize)
    {
        return
            (instruction & InstructionsArm64.LdpFpLrPostIndexMask) == InstructionsArm64.LdpFpLrPostIndex &&
            InstructionsArm64.DecodeSignedImmediate7Scaled8(instruction) == frameSize;
    }

    static bool IsPotentialPrologueInstruction(uint instruction)
    {
        return
            (instruction & InstructionsArm64.AddSubSpSpImmediateMask) == InstructionsArm64.AddSpSpImmediate ||
            (instruction & InstructionsArm64.StpRegistersPreIndexSpMask) == InstructionsArm64.StpRegistersPreIndexSp ||
            instruction is InstructionsArm64.MovFpSp or InstructionsArm64.Pacibsp;
    }

    const uint PackedUnwindFlag = 1;
    const int FunctionLengthOffset = 2;
    const int ChainedOffset = 21;
    const int FrameSizeOffset = 23;
    const int StackAlignment = 16;
    const int MaximumFunctionLength = 0x7ff;
    const int MaximumFrameSize = 0x1ff * StackAlignment;
}
