// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Represents a generic iterator for a checksum computation.
/// </summary>
/// <typeparam name="T">The type of the checksum value.</typeparam>
public interface IChecksumIterator<T> : IChecksumIterator
    where T : struct
{
    /// <inheritdoc cref="IChecksumIterator.ComputeFinal"/>
    new T ComputeFinal();
}
