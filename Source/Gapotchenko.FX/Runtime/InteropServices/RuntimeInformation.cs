using Gapotchenko.FX.Threading;
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
    public static partial class RuntimeInformation
    {
        /// <summary>
        /// Indicates whether the current app is running on the specified platform.
        /// </summary>
        /// <param name="osPlatform">A platform.</param>
        /// <returns><c>true</c> if the current app is running on the specified platform; otherwise, <c>false</c>.</returns>
        public static bool IsOSPlatform(OSPlatform osPlatform) => _OSPlatform.Value == osPlatform;

        static EvaluateOnce<OSPlatform> _OSPlatform = EvaluateOnce.Create(_GetOSPlatform);

        static OSPlatform _GetOSPlatform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32Windows:
                case PlatformID.Win32S:
                    return OSPlatform.Windows;

                case PlatformID.MacOSX:
                    return OSPlatform.OSX;

                case PlatformID.Unix:
                    // TODO: Support Mono on Linux/macOS/... scenario.
                    return OSPlatform.Create("UNIX");

                default:
                    return OSPlatform.Create("UNKNOWN");
            }
        }

        /// <summary>
        /// Gets the architecture of the currently running process.
        /// </summary>
        public static Architecture ProcessArchitecture => _ProcessArchitecture.Value;

        static EvaluateOnce<Architecture> _ProcessArchitecture = EvaluateOnce.Create(_GetProcessArchitecture);

        static Architecture _GetProcessArchitecture()
        {
            if (IsOSPlatform(OSPlatform.Windows))
                return Windows.GetProcessArchitecture();
            else
                throw new PlatformNotSupportedException();
        }

        /// <summary>
        /// Gets the operating system architecture on which the current process is running.
        /// </summary>
        public static Architecture OSArchitecture => _OSArchitecture.Value;

        static EvaluateOnce<Architecture> _OSArchitecture = EvaluateOnce.Create(_GetOSArchitecture);

        static Architecture _GetOSArchitecture()
        {
            if (IsOSPlatform(OSPlatform.Windows))
                return Windows.GetOSArchitecture();
            else
                throw new PlatformNotSupportedException();
        }
    }
}

#else

[assembly: TypeForwardedTo(typeof(RuntimeInformation))]

#endif
