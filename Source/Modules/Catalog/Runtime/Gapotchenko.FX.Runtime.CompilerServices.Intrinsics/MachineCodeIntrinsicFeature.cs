// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices;

/// <summary>
/// Identifies a processor feature that may be required to execute a machine-code intrinsic.
/// </summary>
/// <remarks>
/// A processor feature represents instruction set support available to the current process,
/// including any operating system support required to use it.
/// </remarks>
public enum MachineCodeIntrinsicFeature
{
    /// <summary>
    /// x86/x64 <c>POPCNT</c> instruction.
    /// </summary>
    /// <remarks>
    /// Hardware indication: <c>CPUID.01H:ECX[23]</c>.
    /// </remarks>
    Popcnt = 1
}
