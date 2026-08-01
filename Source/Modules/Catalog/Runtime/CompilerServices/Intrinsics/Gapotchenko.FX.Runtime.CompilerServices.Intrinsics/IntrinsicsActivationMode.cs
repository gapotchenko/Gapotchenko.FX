// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices;

/// <summary>
/// Specifies the activation mode of the intrinsic compiler.
/// </summary>
public enum IntrinsicsActivationMode
{
    /// <summary>
    /// Instructs the intrinsic compiler to automatically select "on" or "off" activation mode
    /// based on the current operating environment.
    /// </summary>
    /// <remarks>
    /// This is the default mode.
    /// </remarks>
    Auto,

    /// <summary>
    /// Instructs the intrinsic compiler to be turned off.
    /// In this mode, subsequent intrinsic initialization is disabled.
    /// </summary>
    /// <remarks>
    /// Intrinsic methods that have already been compiled are not reverted to their managed fallback implementations.
    /// </remarks>
    AlwaysOff,

    /// <summary>
    /// Instructs the intrinsic compiler to activate itself
    /// whenever the current operating environment permits just-in-time machine code execution.
    /// </summary>
    /// <remarks>
    /// Choosing this mode does not guarantee the activation of machine code,
    /// it just expresses the strongest preference for that to happen.
    /// </remarks>
    PreferablyOn
}
