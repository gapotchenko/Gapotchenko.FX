// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX;

/// <summary>
/// Allows general-purpose access to <see cref="Optional{T}"/> instance members
/// without knowing the underlying <see cref="Optional{T}"/> type.
/// </summary>
public interface IOptional
{
    /// <summary>
    /// Gets a value indicating whether the current <see cref="IOptional"/> has a valid underlying value.
    /// </summary>
    bool HasValue { get; }

    /// <summary>
    /// Gets the value of the current <see cref="IOptional"/> if it has been assigned a valid underlying value.
    /// </summary>
    /// <exception cref="InvalidOperationException">The <see cref="HasValue"/> property is <see langword="false"/>.</exception>
    object? Value { get; }
}
