using System;

namespace Gapotchenko.FX.Diagnostics.Pal.Windows
{
    static class EnvironmentInfo
    {
        static EnvironmentInfo()
        {
            var os = Environment.OSVersion;
            if (os.Platform == PlatformID.Win32NT && os.Version < new Version(6, 0))
            {
                // Windows Server 2003 and Windows XP: The maximum size of the environment block for the process is 32,767 characters.                
                MaxSize = 32767;
            }
            else
            {
                // Starting with Windows Vista and Windows Server 2008, there is no technical limitation on the size of the environment block.
                MaxSize = -1;
            }
        }

        /// <summary>
        /// Gets the maximum environment block size or -1 if there is no limit.
        /// </summary>
        public static int MaxSize { get; }
    }
}
