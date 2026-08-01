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
static class UnwindWindowsArm64
{
    public static UnwindAnalysisResult Analyze(ReadOnlySpan<uint> code, out uint unwindData)
    {
        unwindData = 0;
        var result = UnwindArm64.Analyze(code, out var info);
        if (result != UnwindAnalysisResult.Supported)
            return result;

        int functionLength = checked(code.Length + 1);
        if (functionLength > MaximumFunctionLength ||
            info.FrameSize > MaximumFrameSize)
        {
            return UnwindAnalysisResult.Unsupported;
        }

        // Packed unwind data assumes the canonical prologue starts at the
        // function beginning and its inverse epilogue ends immediately before RET.
        if (info.PrologueInstructionCount >= functionLength)
            return UnwindAnalysisResult.Unsupported;

        int chained = info.FrameKind == UnwindArm64.FrameKind.FrameChain ? 3 : 0;
        unwindData = EncodePackedUnwindData(functionLength, info.FrameSize, chained);
        return UnwindAnalysisResult.Supported;
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
