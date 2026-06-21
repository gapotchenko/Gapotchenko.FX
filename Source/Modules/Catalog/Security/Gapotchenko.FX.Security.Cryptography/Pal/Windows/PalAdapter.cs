// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if !HAS_TARGET_PLATFORM || WINDOWS

using System.Diagnostics;

namespace Gapotchenko.FX.Security.Cryptography.Pal.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
sealed class PalAdapter : IPalAdapter
{
    public static PalAdapter Instance { get; } = new();

    public bool? QueryFipsPolicy()
    {
        if (Environment.OSVersion.Version.Major >= 6)
        {
            // Windows Vista or newer.
            uint status = NativeMethods.BCryptGetFipsAlgorithmMode(out bool fipsEnabled);
            if (status is NativeMethods.STATUS_SUCCESS or NativeMethods.STATUS_OBJECT_NAME_NOT_FOUND)
                return fipsEnabled;
        }
        else
        {
            // On older systems, it's possible to query FIPSAlgorithmPolicy value of HKLM\System\CurrentControlSet\Control\Lsa registry key.
            // Then, the FIPS policy state can be calculated as:
            //
            //     fipsEnabled =
            //         value is not null &&
            //         (value is not int num || num != 0)
            //
            // Not implementing it here because all project target frameworks can only run on Windows Vista+.

            Debug.Fail("Expected to run on Windows Vista+ only.");
        }

        return null;
    }
}

#endif
