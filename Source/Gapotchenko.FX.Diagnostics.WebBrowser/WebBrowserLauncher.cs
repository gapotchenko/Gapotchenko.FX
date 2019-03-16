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
            bool handled = false;

            try
            {
                string scheme = null;

                int j;

                j = url.IndexOf(':');
                if (j != -1)
                    scheme = url.Substring(0, j);

                if (scheme != null)
                {
                    string defaultBrowserCommand = _TryGetDefaultBrowserCommand(scheme);
                    if (!string.IsNullOrWhiteSpace(defaultBrowserCommand))
                    {
                        if (defaultBrowserCommand[0] == '"')
                            j = defaultBrowserCommand.IndexOf('"', 1);
                        else
                            j = defaultBrowserCommand.IndexOf(' ');

                        if (j == -1)
                            j = defaultBrowserCommand.Length;
                        else
                            ++j;

                        string arguments = defaultBrowserCommand.Substring(j).Trim();
                        defaultBrowserCommand = defaultBrowserCommand.Substring(0, j);

                        defaultBrowserCommand = defaultBrowserCommand.Trim('"');

                        string argumentsPattern = arguments;
                        arguments = arguments.Replace("%1", url);

                        if (arguments != argumentsPattern)
                        {
                            if (File.Exists(defaultBrowserCommand))
                            {
                                _RunProcess(defaultBrowserCommand, arguments);
                                handled = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e) when (!e.IsControlFlowException())
            {
                Log.TraceSource.TraceEvent(TraceEventType.Warning, 1, e.Message);
            }

            return handled;
        }

        static bool _IsWindowsVistaOrHigher()
        {
            var os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT && os.Version.Major >= 6;
        }

        static string _TryGetDefaultBrowserCommand(string scheme)
        {
            string command = null;

            try
            {
                var os = Environment.OSVersion;

                if (os.Platform == PlatformID.Win32NT && os.Version.Major >= 6)
                {
                    // Windows Vista or higher provides a user choice for default browser.
                    using (var key = Registry.CurrentUser.OpenSubKey($@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\{scheme}\UserChoice"))
                        scheme = Empty.NullifyWhiteSpace(key?.GetValue("ProgId") as string) ?? scheme;
                }

                using (var key = Registry.ClassesRoot.OpenSubKey($@"{scheme}\shell\open\command"))
                {
                    if (key != null)
                        command = Empty.NullifyWhiteSpace(key.GetValue(null) as string);
                }
            }
            catch (Exception e) when (!e.IsControlFlowException())
            {
                Log.TraceSource.TraceEvent(TraceEventType.Warning, 2, e.Message);
            }

            return command;
        }

        static void _RunProcess(string fileName, string arguments = null)
        {
            try
            {
                Process.Start(fileName, arguments);
            }
            catch (Exception e) when (!e.IsControlFlowException())
            {
                Log.TraceSource.TraceEvent(TraceEventType.Verbose, 0, e.Message);
            }
        }
    }
}
