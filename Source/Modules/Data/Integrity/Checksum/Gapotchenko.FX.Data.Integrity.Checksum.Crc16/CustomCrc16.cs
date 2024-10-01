// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Customizable CRC-16 checksum algorithm.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CustomCrc16"/> class with specified CRC-16 algorithm parameters.
/// </remarks>
/// <inheritdoc/>
[CLSCompliant(false)]
public class CustomCrc16(
    ushort polynomial,
    ushort initialValue,
    bool reflectedInput,
    bool reflectedOutput,
    ushort xorOutput) : GenericCrc16(
          polynomial,
          initialValue,
          reflectedInput,
          reflectedOutput,
          xorOutput)
{
}
