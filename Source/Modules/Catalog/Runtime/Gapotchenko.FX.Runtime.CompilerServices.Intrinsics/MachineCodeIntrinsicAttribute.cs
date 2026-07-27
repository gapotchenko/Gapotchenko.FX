// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices;

/// <summary>
/// Defines machine code intrinsic for a specified processor architecture.
/// </summary>
[CLSCompliant(false)]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MachineCodeIntrinsicAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MachineCodeIntrinsicAttribute"/> class.
    /// </summary>
    /// <param name="architecture">The processor architecture.</param>
    /// <param name="code">The machine code.</param>
    public MachineCodeIntrinsicAttribute(Architecture architecture, params byte[] code)
    {
        Architecture = architecture;
        Code = code;
    }

    /// <summary>
    /// Gets the processor architecture targeted by the machine code.
    /// </summary>
    public Architecture Architecture { get; }

    /// <summary>
    /// Gets or initializes the additional processor architectures targeted by the machine code.
    /// </summary>
    public Architecture[] AdditionalArchitectures { get; init; } = [];

    /// <summary>
    /// Gets the machine code.
    /// </summary>
    public byte[] Code { get; }

    /// <summary>
    /// Gets or initializes the processor features required to execute the machine code.
    /// </summary>
    /// <remarks>
    /// An empty array indicates that the machine code has no processor requirements
    /// beyond those implied by <see cref="Architecture"/>.
    /// </remarks>
    public MachineCodeIntrinsicFeature[] RequiredFeatures { get; init; } = [];

    /// <summary>
    /// Gets or initializes the platforms or operating systems supported by the machine code.
    /// </summary>
    /// <remarks>
    /// An empty array (default) indicates that the machine code supports all platforms and operating systems.
    /// </remarks>
    public string[] SupportedOSPlatforms { get; init; } = [];

    /// <summary>
    /// Gets or initializes the intrinsic priority.
    /// </summary>
    /// <remarks>
    /// The lower the priority value the more preferred the intrinsic.
    /// </remarks>
    public int Priority { get; init; }
}
