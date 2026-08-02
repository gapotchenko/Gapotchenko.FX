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
        if (!code.IsEmpty &&
            (InstructionsArm32.IsPushRegisters(code[0]) ||
             InstructionsArm32.IsPushRegistersWide(code) ||
             InstructionsArm32.IsStackAllocation(code[0])))
        {
            return UnwindAnalysisResult.Unsupported;
        }

        return UnwindAnalysisResult.Leaf;
    }
}
