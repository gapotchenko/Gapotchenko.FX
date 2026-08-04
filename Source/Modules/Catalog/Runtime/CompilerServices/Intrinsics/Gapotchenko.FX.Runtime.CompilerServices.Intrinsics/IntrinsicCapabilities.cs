// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices;

/// <summary>
/// Describes intrinsic capabilities available in an execution environment.
/// </summary>
[Flags]
public enum IntrinsicCapabilities
{
    /// <summary>
    /// No intrinsic capabilities available.
    /// </summary>
    None = 0,

    /// <summary>
    /// Machine code intrinsic feature support detection capability
    /// provided by <see cref="Intrinsics.IsFeatureSupported(MachineCodeIntrinsicFeature)"/> method.
    /// </summary>
    FeatureSupport = 1 << 0,

    /// <summary>
    /// Machine code intrinsic compilation capability
    /// provided by <see cref="Intrinsics.InitializeType(Type)"/> method.
    /// </summary>
    Compilation = 1 << 1
}
