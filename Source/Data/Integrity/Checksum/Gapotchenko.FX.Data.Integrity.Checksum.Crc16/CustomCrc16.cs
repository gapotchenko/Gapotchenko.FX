using System;

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Customizable CRC-16 checksum algorithm.
/// </summary>
[CLSCompliant(false)]
public class CustomCrc16 : GenericCrc16
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCrc16"/> class with specified CRC-16 algorithm parameters.
    /// </summary>
    /// <inheritdoc/>
    public CustomCrc16(
        ushort polynomial,
        ushort initialValue,
        bool reflectedInput,
        bool reflectedOutput,
        ushort xorOutput)
        : base(
              polynomial,
              initialValue,
              reflectedInput,
              reflectedOutput,
              xorOutput)
    {
    }
}
