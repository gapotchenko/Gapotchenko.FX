using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Diagnostics
{
    /// <summary>
    /// Provides access to the local web browser.
    /// </summary>
    public static class WebBrowser
    {
        /// <summary>
        /// Starts a local web browser and navigates to a specified URL.
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
