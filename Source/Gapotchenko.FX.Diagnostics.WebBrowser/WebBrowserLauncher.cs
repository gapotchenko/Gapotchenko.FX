using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace Gapotchenko.FX.Diagnostics
{
    sealed class WebBrowserLauncher
    {
        public static void OpenUrl(string url)
        {
            if (!_TryOpenUrlWithDefaultBrowser(url))
                _RunProcess(url);
        }

        static bool _TryOpenUrlWithDefaultBrowser(string url)
        {
            try
            {
                int j = url.IndexOf(':');
                if (j == -1)
                    return false;

                string scheme = url.Substring(0, j);

                string? command = _TryGetDefaultBrowserCommand(scheme);
                if (command == null)
                    return false;

                if (command[0] == '"')
                    j = command.IndexOf('"', 1);
                else
                    j = command.IndexOf(' ');

                if (j == -1)
                    j = command.Length;
                else
                    ++j;

                string args = command.Substring(j).Trim();
                string filePath = command.Substring(0, j).Trim('"');

                string pattern = args;

                args = pattern.Replace("%1", url);

                if (args.Equals(pattern, StringComparison.Ordinal))
                {
                    // URL is not set.
                    return false;
                }

                if (!File.Exists(filePath))
                    return false;

                _RunProcess(filePath, args);
                return true;
            }
            catch (Exception e) when (!e.IsControlFlowException())
            {
                Log.TraceSource.TraceEvent(TraceEventType.Warning, 1932901051, e.Message);
                return false;
            }
        }

        static string? _TryGetDefaultBrowserCommand(string scheme)
        {
            var os = Environment.OSVersion;

            if (os.Platform == PlatformID.Win32NT && os.Version.Major >= 6)
            {
                // Windows Vista and newer versions provide user choice for a default browser.
                using (var key = Registry.CurrentUser.OpenSubKey($@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\{scheme}\UserChoice"))
                    scheme = Empty.NullifyWhiteSpace(key?.GetValue("ProgId") as string) ?? scheme;
            }

            using (var key = Registry.ClassesRoot.OpenSubKey($@"{scheme}\shell\open\command"))
            {
                if (key != null)
                    return Empty.NullifyWhiteSpace(key.GetValue(null) as string);
            }

            return null;
        }

        static void _RunProcess(string fileName, string? arguments = null)
        {
            try
            {
                Process.Start(fileName, arguments);
            }
            catch (Exception e) when (!e.IsControlFlowException())
            {
                Log.TraceSource.TraceEvent(TraceEventType.Verbose, 1932901050, e.Message);
            }
        }
    }
}
