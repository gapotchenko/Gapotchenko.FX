// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Security.Cryptography.Pal;
using Gapotchenko.FX.Threading;
using System.Diagnostics;

namespace Gapotchenko.FX.Security.Cryptography;

/// <summary>
/// Provides information about the cryptographic policy enforced by the current environment.
/// </summary>
/// <remarks>
/// The cryptographic policy is determined from the host operating system,
/// runtime environment, and cryptographic providers.
/// </remarks>
public static class CryptographyPolicy
{
    /// <summary>
    /// Gets a value indicating whether the current environment permits only
    /// FIPS-approved cryptographic algorithms.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the current environment permits only FIPS-approved cryptographic algorithms;
    /// otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// <para>
    /// The value may be determined by inspecting host cryptographic policy
    /// configuration or by probing whether a known FIPS-incompatible algorithm
    /// can be used in the current environment.
    /// </para>
    /// <para>
    /// The validity of FIPS certification for the underlying cryptographic
    /// implementation is the responsibility of the operating system and its
    /// cryptographic providers.
    /// </para>
    /// </remarks>
    public static bool AllowOnlyFipsAlgorithms =>
        m_EnforceFipsAlgorithmsOnly ||
        m_CachedAllowOnlyFipsAlgorithms.Value;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static EvaluateOnce<bool> m_CachedAllowOnlyFipsAlgorithms = new(AreOnlyFipsAlgorithmsAllowed, m_SyncLock);

    static bool AreOnlyFipsAlgorithmsAllowed()
    {
        return
            CryptoConfig.AllowOnlyFipsAlgorithms || // this alone is not enough for some .NET runtime versions
            (QueryFips() ?? ProbeFips()); // query the OS and fallback to probing if the result is inconclusive
    }

    static bool? QueryFips()
    {
        return PalServices.Adapter?.QueryFipsPolicy();
    }

#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms

    static bool ProbeFips()
    {
        try
        {
            using var des = DES.Create();

            byte[] data = [0, 1, 2, 3, 4, 5, 6, 7];
            des.Key = data;
            des.IV = data;

            using var encryptor = des.CreateEncryptor();
            _ = encryptor.TransformFinalBlock(data, 0, data.Length);
        }
        catch (CryptographicException)
        {
            // Cryptographic error when trying to work with a legacy encryption algorithm
            // implies that the system may be in FIPS mode.
            return true;
        }

        // Certainly not a FIPS environment.
        return false;
    }

#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static readonly Lock m_SyncLock = new();

    /// <summary>
    /// Overrides the effective cryptographic policy to indicate that only
    /// FIPS-approved cryptographic algorithms are permitted.
    /// </summary>
    /// <remarks>
    /// <para>
    /// After this method has been called, <see cref="AllowOnlyFipsAlgorithms"/>
    /// always returns <see langword="true"/> for the lifetime of the current
    /// process.
    /// </para>
    /// <para>
    /// This method cannot weaken restrictions imposed by the host environment.
    /// It is primarily intended for enforcing FIPS algorithms at the application level.
    /// Another use case is testing and validation of application behavior
    /// under FIPS-only policy conditions.
    /// </para>
    /// </remarks>
    public static void EnforceFipsAlgorithmsOnly()
    {
        m_EnforceFipsAlgorithmsOnly = true;
    }

    static bool m_EnforceFipsAlgorithmsOnly;
}
