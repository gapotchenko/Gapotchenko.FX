namespace Gapotchenko.FX.Diagnostics;

/// <summary>
/// Provides access to a web browser.
/// </summary>
public static class WebBrowser
{
    /// <summary>
    /// Launches the default web browser and navigates to a specified URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    public static void Launch(string url)
    {
        ArgumentNullException.ThrowIfNull(url);
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            throw new ArgumentException("Malformed URL.", nameof(url));

        WebBrowserLauncher.OpenUrl(url);
    }
}
