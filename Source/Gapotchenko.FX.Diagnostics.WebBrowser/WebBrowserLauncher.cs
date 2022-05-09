using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Diagnostics
{
    sealed partial class WebBrowserLauncher
    {
        public static void OpenUrl(string url)
        {
            if (!TryOpenUrlWithDefaultBrowser(url))
                RunProcess(url);
        }

        static bool TryOpenUrlWithDefaultBrowser(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return TryOpenUrlWithDefaultBrowser_Windows(url);
                else
                    return false;
            }
            catch (Exception e) when (!e.IsControlFlowException())
            {
                Log.TraceSource.TraceEvent(TraceEventType.Warning, 1932901051, e.Message);
                return false;
            }
        }

        static void RunProcess(string fileName, string? arguments = null)
        {
            try
            {
                var psi = new ProcessStartInfo(fileName, arguments ?? string.Empty)
                {
#if NETSTANDARD || NET
                    UseShellExecute = true // .NET Framework has UseShellExecute set to true by default, .NET Core hasn't.
#endif
                };
                Process.Start(psi)?.Dispose();
            }
            catch (Exception e) when (!e.IsControlFlowException())
            {
                Log.TraceSource.TraceEvent(TraceEventType.Verbose, 1932901050, e.Message);
            }
        }
    }
}
