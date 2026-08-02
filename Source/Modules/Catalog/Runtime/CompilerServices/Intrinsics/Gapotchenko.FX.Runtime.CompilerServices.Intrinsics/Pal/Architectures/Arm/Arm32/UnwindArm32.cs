// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures.Arm.Arm32;

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

/// <summary>
/// Analyzes ARM32 unwind prologues and epilogues.
/// </summary>
static class UnwindArm32
{
    public static UnwindAnalysisResult Analyze(ReadOnlySpan<ushort> code)
    {
        if (!code.IsEmpty &&
            (InstructionsArm32.IsPushRegisters(code[0]) ||
             InstructionsArm32.IsPushRegistersWide(code) ||
             InstructionsArm32.IsStackAllocation(code[0])))
        {
            return UnwindAnalysisResult.Supported;
        }

        return UnwindAnalysisResult.Leaf;
    }
}
