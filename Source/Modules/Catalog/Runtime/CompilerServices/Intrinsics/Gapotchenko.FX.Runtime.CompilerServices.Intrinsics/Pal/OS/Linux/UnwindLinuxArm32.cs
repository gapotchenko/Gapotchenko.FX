// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures.Arm.Arm32;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Linux;

#if NET
[SupportedOSPlatform("linux")]
#endif
static class UnwindLinuxArm32
{
    public static UnwindAnalysisResult Analyze(ReadOnlySpan<ushort> code)
    {
        return UnwindArm32.Analyze(code) == UnwindAnalysisResult.Leaf ?
            UnwindAnalysisResult.Leaf :
            UnwindAnalysisResult.Unsupported;
    }
}
