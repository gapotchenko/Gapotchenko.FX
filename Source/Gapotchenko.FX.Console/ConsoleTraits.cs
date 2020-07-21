using Gapotchenko.FX.Diagnostics;
using System;
using System.Diagnostics;
using System.IO;
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

        enum BoolTrait : byte
        {
            None,
            True,
            False
        }

        static BoolTrait m_CachedIsColorInhibited;

        /// <summary>
        /// <para>
        /// Gets a value indicating whether console color is inhibited.
        /// </para>
        /// <para>
        /// Console color can be inhibited by the host system or a user preference.
        /// For example, a NO_COLOR environment variable can be used to inhibit console colors as described by corresponding <a href="https://no-color.org/">specification</a>.
        /// </para>
        /// </summary>
        public static bool IsColorInhibited
        {
            get
            {
                var value = m_CachedIsColorInhibited;
                if (value == BoolTrait.None)
                {
                    value = IsColorInhibitedCore() ? BoolTrait.True : BoolTrait.False;
                    m_CachedIsColorInhibited = value;
                }
                return value == BoolTrait.True;
            }
        }

        static bool IsColorInhibitedCore() =>
            Environment.GetEnvironmentVariable("NO_COLOR") != null;  // https://no-color.org/

        /// <summary>
        /// <para>
        /// Gets a value indicating whether console color is enabled.
        /// </para>
        /// <para>
        /// The console color is enabled when it is <see cref="IsColorAvailable">available</see> and not <see cref="IsColorInhibited">inhibited</see>.
        /// </para>
        /// </summary>
        public static bool IsColorEnabled => IsColorAvailable && !IsColorInhibited;

        // This cached value should not be discarded as it represents an immutable trait.
        static BoolTrait m_CachedWillDisappearOnExit;

        /// <summary>
        /// Gets a value indicating whether a console window will immediately disappear on program exit.
        /// </summary>
        public static bool WillDisappearOnExit
        {
            get
            {
                var value = m_CachedWillDisappearOnExit;
                if (value == BoolTrait.None)
                {
                    value = WillDisappearOnExitCore() ? BoolTrait.True : BoolTrait.False;
                    m_CachedWillDisappearOnExit = value;
                }
                return value == BoolTrait.True;
            }
        }

        static bool WillDisappearOnExitCore() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
            WindowsExplorerDetector.IsStartedByExplorer();

        static class WindowsExplorerDetector
        {
            public static bool IsStartedByExplorer()
            {
                try
                {
                    var parentProcess = Process.GetCurrentProcess().GetParent();
                    return
                        parentProcess != null &&
                        Path.GetFileName(parentProcess.GetImageFileName()).Equals("Explorer.EXE", StringComparison.OrdinalIgnoreCase);
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
            m_CachedIsColorInhibited = default;
        }
    }
}
