// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

using Gapotchenko.FX.Security.Cryptography.Properties;

namespace Gapotchenko.FX.Security.Cryptography;

#if BINARY_COMPATIBILITY || SOURCE_COMPATIBILITY
/// <summary>
/// Provides a managed implementation of the Alleged Rivest Cipher 4 (ARC4) algorithm.
/// </summary>
[Obsolete("Use Arc4 type instead.")] // 2026
[EditorBrowsable(EditorBrowsableState.Never)]
public
#endif
sealed class Arc4Managed : Arc4ManagedBase
{
#if BINARY_COMPATIBILITY || SOURCE_COMPATIBILITY
    /// <summary>
    /// Initializes a new instance of the <see cref="Arc4Managed"/> class.
    /// </summary>
    [Obsolete("Use Arc4.Create() method instead.", true)] // 2026
    public Arc4Managed() : this(true)
    {
    }
#endif

    internal Arc4Managed(bool enforcePolicies)
    {
        if (enforcePolicies)
        {
            if (CryptographyPolicy.AllowOnlyFipsAlgorithms)
                throw new CryptographicException(string.Format(Resources.XAlgorithmCannotBeUsedDueFips, AlgorithmName));
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}
