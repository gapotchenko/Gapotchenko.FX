// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Customizable CRC-8 checksum algorithm.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CustomCrc8"/> class with specified CRC-8 algorithm parameters.
/// </remarks>
/// <inheritdoc/>
public class CustomCrc8(
    byte polynomial,
    byte initialValue,
    bool reflectedInput,
    bool reflectedOutput,
    byte xorOutput) : GenericCrc8(
        polynomial,
        initialValue,
        reflectedInput,
        reflectedOutput,
        xorOutput)
{
}
