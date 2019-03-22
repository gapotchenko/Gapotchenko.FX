using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !TFF_RUNTIME_INFORMATION

namespace System.Runtime.InteropServices
{
    /// <summary>
    /// <para>
    /// Provides information about the .NET runtime installation.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    public static class RuntimeInformation
    {
        /// <summary>
        /// Indicates whether the current app is running on the specified platform.
        /// </summary>
        /// <param name="osPlatform">A platform.</param>
        /// <returns><c>true</c> if the current app is running on the specified platform; otherwise, <c>false</c>.</returns>
        public static bool IsOSPlatform(OSPlatform osPlatform)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32Windows:
                case PlatformID.Win32S:
                case PlatformID.WinCE:
                    return osPlatform == OSPlatform.Windows;

                case PlatformID.MacOSX:
                    return osPlatform == OSPlatform.OSX;

                case PlatformID.Unix:
                    // TODO: Support Mono on Linux/... scenario.
                    return false;

                default:
                    return false;
            }
        }
    }
}

#else

[assembly: TypeForwardedTo(typeof(RuntimeInformation))]

#endif
