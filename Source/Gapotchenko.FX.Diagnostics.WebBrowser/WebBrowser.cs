using System;

namespace Gapotchenko.FX.Diagnostics
{
    /// <summary>
    /// Provides access to the web browser.
    /// </summary>
    public static class WebBrowser
    {
        /// <summary>
        /// Starts a web browser and navigates to a specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        public static void Start(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            WebBrowserLauncher.OpenUrl(url);
        }
    }
}
