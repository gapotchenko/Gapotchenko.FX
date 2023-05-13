// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Customizable CRC-8 checksum algorithm.
/// </summary>
public class CustomCrc8 : GenericCrc8
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCrc8"/> class with specified CRC-8 algorithm parameters.
    /// </summary>
    /// <inheritdoc/>
    public CustomCrc8(
        byte polynomial,
        byte initialValue,
        bool reflectedInput,
        bool reflectedOutput,
        byte xorOutput)
        : base(
              polynomial,
              initialValue,
              reflectedInput,
              reflectedOutput,
              xorOutput)
    {
    }
}
