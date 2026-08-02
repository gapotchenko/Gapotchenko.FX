// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures.Arm.Arm64;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

/// <summary>
/// Analyzes and encodes packed Windows ARM64 unwind information.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
static class UnwindWindowsArm64
{
    public static UnwindArm64.Analysis Analyze(ReadOnlySpan<uint> code)
    {
        var analysis = UnwindArm64.Analyze(code);
        if (analysis.Result != UnwindAnalysisResult.Supported)
            return analysis;

        int functionLength = checked(code.Length + 1);
        if (functionLength > MaximumFunctionLength ||
            analysis.Info.FrameSize > MaximumFrameSize)
        {
            return analysis with { Result = UnwindAnalysisResult.Unsupported };
        }

        // Packed unwind data assumes the canonical prologue starts at the
        // function beginning and its inverse epilogue ends immediately before RET.
        if (analysis.Info.PrologueInstructionCount >= functionLength)
            return analysis with { Result = UnwindAnalysisResult.Unsupported };

        return analysis;
    }

    public static uint GetUnwindData(ReadOnlySpan<uint> code, in UnwindArm64.Analysis unwindAnalysis)
    {
        int chained = unwindAnalysis.Info.FrameKind == UnwindArm64.FrameKind.FrameChain ? 3 : 0;
        return EncodePackedUnwindData(
            checked(code.Length + 1),
            unwindAnalysis.Info.FrameSize,
            chained);
    }

    static uint EncodePackedUnwindData(int functionLength, int frameSize, int chained)
    {
        return PackedUnwindFlag |
        (uint)functionLength << FunctionLengthOffset |
        (uint)chained << ChainedOffset |
        (uint)(frameSize / UnwindArm64.StackAlignment) << FrameSizeOffset;
    }

    const uint PackedUnwindFlag = 1;
    const int FunctionLengthOffset = 2;
    const int ChainedOffset = 21;
    const int FrameSizeOffset = 23;
    const int MaximumFunctionLength = 0x7ff;
    const int MaximumFrameSize = 0x1ff * UnwindArm64.StackAlignment;
}
