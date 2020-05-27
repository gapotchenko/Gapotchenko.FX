﻿using Gapotchenko.FX.Diagnostics;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Console
{
    using Console = System.Console;

    /// <summary>
    /// Provides console traits and characteristics.
    /// </summary>
    public static class ConsoleTraits
    {
        /// <summary>
        /// <para>
        /// Gets a value indicating whether console color is available.
        /// </para>
        /// <para>
        /// Console color is usually always available unless program output streams are redirected.
        /// </para>
        /// </summary>
        public static bool IsColorAvailable => !(Console.IsOutputRedirected || Console.IsErrorRedirected);

        /// <summary>
        /// <para>
        /// Gets a value indicating whether console color is inhibited.
        /// </para>
        /// <para>
        /// Console color can be inhibited by the host system or a user preference.
        /// For example, a NO_COLOR environment variable can be used to inhibit console colors as described by corresponding <a href="https://no-color.org/">specification</a>.
        /// </para>
        /// </summary>
        public static bool IsColorInhibited => !(NoColor.IsPresent);

        /// <summary>
        /// Implements NO_COLOR specification according to https://no-color.org/.
        /// </summary>
        static class NoColor
        {
            static NoColor()
            {
                Refresh();
            }

            public static bool IsPresent { get; private set; }

            static bool IsPresentCore() => Environment.GetEnvironmentVariable("NO_COLOR") != null;

            public static void Refresh()
            {
                IsPresent = IsPresentCore();
            }
        }

        /// <summary>
        /// <para>
        /// Gets a value indicating whether console color is enabled.
        /// </para>
        /// <para>
        /// The console color is enabled when it is <see cref="IsColorAvailable">available</see> and not <see cref="IsColorInhibited">inhibited</see>.
        /// </para>
        /// </summary>
        public static bool IsColorEnabled => IsColorAvailable && !IsColorInhibited;

        // This cache should not be discarded as it is fully static.
        static int m_CachedWillDisappearOnExit;

        /// <summary>
        /// Gets a value indicating whether a console window immediately disappears on program exit.
        /// </summary>
        public static bool WillDisappearOnExit
        {
            get
            {
                int v = m_CachedWillDisappearOnExit;
                if (v == 0)
                {
                    v = _WillDisappearOnExitCore() ? 1 : 2;
                    m_CachedWillDisappearOnExit = v;
                }
                return v == 1;
            }
        }

        static bool _WillDisappearOnExitCore() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
            WindowsExplorerDetector.WasProbablyStartedByExplorer &&
            Environment.UserInteractive &&
            WindowsExplorerDetector.IsStartedByExplorer();

        static class WindowsExplorerDetector
        {
            static WindowsExplorerDetector()
            {
                try
                {
                    // http://support.microsoft.com/kb/99115
                    WasProbablyStartedByExplorer = Console.CursorLeft == 0 && Console.CursorTop == 0;
                }
                catch (Exception e) when (!e.IsControlFlowException())
                {
                    WasProbablyStartedByExplorer = false;
                }
            }

            public static bool WasProbablyStartedByExplorer { get; }

            public static bool IsStartedByExplorer()
            {
                try
                {
                    var parentProcess = Process.GetCurrentProcess().GetParent();
                    if (parentProcess == null)
                        return false;

                    return parentProcess.GetImageFileName().Equals("explorer.exe", StringComparison.OrdinalIgnoreCase);
                }
                catch (Exception e) when (!e.IsControlFlowException())
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Discards any information about console traits that has been cached.
        /// </summary>
        public static void Refresh()
        {
            NoColor.Refresh();
        }
    }
}
