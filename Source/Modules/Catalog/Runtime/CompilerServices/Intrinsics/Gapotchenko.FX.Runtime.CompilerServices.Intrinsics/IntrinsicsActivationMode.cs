// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices;

/// <summary>
/// Specifies the mode of activation for <see cref="Intrinsics"/> compiler.
/// </summary>
public enum IntrinsicsActivationMode
{
    /// <summary>
    /// Instructs <see cref="Intrinsics"/> compiler to automatically select "on" or "off" activation mode
    /// based on the current operating environment.
    /// This is the default mode.
    /// </summary>
    Auto,

    /// <summary>
    /// Instructs <see cref="Intrinsics"/> compiler to be turned off.
    /// In this mode, intrinsic methods always use their managed fallback implementations,
    /// no machine code is executed.
    /// </summary>
    AlwaysOff,

    /// <summary>
    /// Instructs <see cref="Intrinsics"/> compiler to activate itself
    /// whenever the current operating environment permits just-in-time machine code execution.
    /// </summary>
    /// <remarks>
    /// Choosing this mode does not guarantee the activation of machine code,
    /// it just expresses the strongest preference for that to happen.
    /// </remarks>
    PreferablyOn
}
