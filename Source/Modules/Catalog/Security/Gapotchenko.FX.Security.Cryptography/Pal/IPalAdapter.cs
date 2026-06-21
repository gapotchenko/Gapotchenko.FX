// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography.Pal;

/// <summary>
/// Defines the interface of a platform abstraction layer (PAL) adapter.
/// </summary>
interface IPalAdapter
{
    /// <summary>
    /// Queries whether the current platform enforces a FIPS cryptographic policy.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the platform enforces a FIPS cryptographic policy;
    /// <see langword="false"/> if the platform does not enforce such a policy;
    /// <see langword="null"/> if the policy state cannot be determined.
    /// </returns>
    bool? QueryFipsPolicy();
}
