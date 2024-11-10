// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Customizable CRC-32 checksum algorithm.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CustomCrc32"/> class with specified CRC-32 algorithm parameters.
/// </remarks>
/// <inheritdoc/>
[CLSCompliant(false)]
public class CustomCrc32(
    uint polynomial,
    uint initialValue,
    bool reflectedInput,
    bool reflectedOutput,
    uint xorOutput) : GenericCrc32(
        polynomial,
        initialValue,
        reflectedInput,
        reflectedOutput,
        xorOutput)
{
}
