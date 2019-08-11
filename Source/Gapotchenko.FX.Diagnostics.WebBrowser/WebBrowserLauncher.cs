using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

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
                string scheme;
                int j = url.IndexOf(':');
                if (j != -1)
                    scheme = url.Substring(0, j);
                else
                    return false;

                string command = _TryGetDefaultBrowserCommand(scheme);
                if (command == null)
                    return false;

                var args = CommandLine.Split(command).ToList();

                int argsCount = args.Count;
                if (argsCount < 2)
                    return false;

                string filePath = args[0];

                bool urlSet = false;
                for (int i = 1; i < argsCount; ++i)
                {
                    if (args[i].Equals("%1", StringComparison.Ordinal))
                    {
                        args[i] = url;
                        urlSet = true;
                    }
                }

                if (!urlSet)
                    return false;

                if (!File.Exists(filePath))
                    return false;

                _RunProcess(filePath, CommandLine.Build(args.Skip(1)));
                return true;
            }
            catch (Exception e) when (!e.IsControlFlowException())
            {
                Log.TraceSource.TraceEvent(TraceEventType.Warning, 1932901051, e.Message);
                return false;
            }
        }

        static string _TryGetDefaultBrowserCommand(string scheme)
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

        static void _RunProcess(string fileName, string arguments = null)
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
