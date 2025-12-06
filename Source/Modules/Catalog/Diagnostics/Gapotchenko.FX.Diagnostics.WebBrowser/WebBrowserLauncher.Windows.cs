using Microsoft.Win32;

namespace Gapotchenko.FX.Diagnostics;

partial class WebBrowserLauncher
{
#if NET
    [SupportedOSPlatform("windows")]
#endif
    static bool TryOpenUrlWithDefaultBrowser_Windows(string url)
    {
        int j = url.IndexOf(':');
        if (j == -1)
            return false;

        string scheme = url[..j];

        string? command = TryGetDefaultBrowserCommand_Windows(scheme);
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

        string args = command.AsSpan(j).Trim().ToString();
        string filePath = command.AsSpan(0, j).Trim('"').ToString();

        string pattern = args;

        args = pattern.Replace("%1", url);

        if (args.Equals(pattern, StringComparison.Ordinal))
        {
            // URL cannot be passed in an argument.
            return false;
        }

        if (!File.Exists(filePath))
            return false;

        RunProcess(filePath, args);
        return true;
    }

#if NET
    [SupportedOSPlatform("windows")]
#endif
    static string? TryGetDefaultBrowserCommand_Windows(string scheme)
    {
        var os = Environment.OSVersion;

        if (os.Platform == PlatformID.Win32NT && os.Version.Major >= 6)
        {
            // Windows Vista and newer versions provide user choice for a default browser.
            using var key = Registry.CurrentUser.OpenSubKey($@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\{scheme}\UserChoice");
            scheme = Empty.NullifyWhiteSpace(key?.GetValue("ProgId") as string) ?? scheme;
        }

        using (var key = Registry.ClassesRoot.OpenSubKey($@"{scheme}\shell\open\command"))
        {
            if (key != null)
                return Empty.NullifyWhiteSpace(key.GetValue(null) as string);
        }

        return null;
    }
}
