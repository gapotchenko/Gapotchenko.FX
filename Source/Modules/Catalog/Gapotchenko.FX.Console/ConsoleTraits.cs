using Gapotchenko.FX.Diagnostics;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Console;

using Console = System.Console;

/// <summary>
/// Provides console traits and characteristics.
/// </summary>
public static class ConsoleTraits
{
    /// <summary>
    /// <para>
    /// Gets a value indicating whether console color output is available.
    /// </para>
    /// <para>
    /// Console color is usually always available unless program standard output streams are redirected.
    /// </para>
    /// </summary>
    public static bool IsColorAvailable => !(Console.IsOutputRedirected || Console.IsErrorRedirected);


    /// <summary>
    /// <para>
    /// Gets a value indicating whether console color output is inhibited.
    /// </para>
    /// <para>
    /// Console color may be inhibited by a host system or a user preference.
    /// For example, <c>NO_COLOR</c> environment variable can be used to inhibit console colors as described by the corresponding <a href="https://no-color.org/">specification</a>.
    /// </para>
    /// </summary>
    public static bool IsColorInhibited
    {
        get
        {
            var value = m_CachedIsColorInhibited;
            if (value == AtomicNullableBool.Null)
            {
                value = IsColorInhibitedCore() ? AtomicNullableBool.True : AtomicNullableBool.False;
                m_CachedIsColorInhibited = value;
                Thread.MemoryBarrier();  // Freshness improvement.
            }
            return value == AtomicNullableBool.True;
        }
    }

    static AtomicNullableBool m_CachedIsColorInhibited;

    static bool IsColorInhibitedCore() =>
        Environment.GetEnvironmentVariable("NO_COLOR") != null;  // https://no-color.org/

    /// <summary>
    /// <para>
    /// Gets a value indicating whether console color output is enabled.
    /// </para>
    /// <para>
    /// The console color is enabled when it is <see cref="IsColorAvailable">available</see> and not <see cref="IsColorInhibited">inhibited</see>.
    /// </para>
    /// </summary>
    public static bool IsColorEnabled => IsColorAvailable && !IsColorInhibited;

    /// <summary>
    /// Gets a value indicating whether a console window will immediately disappear on program exit.
    /// </summary>
    public static bool WillDisappearOnExit
    {
        get
        {
            var value = m_CachedWillDisappearOnExit;
            if (value == AtomicNullableBool.Null)
            {
                value = WillDisappearOnExitCore() ? AtomicNullableBool.True : AtomicNullableBool.False;
                m_CachedWillDisappearOnExit = value;
                Thread.MemoryBarrier();  // Freshness improvement.
            }
            return value == AtomicNullableBool.True;
        }
    }

    // This cached value should not be discarded as it represents an immutable trait.
    static AtomicNullableBool m_CachedWillDisappearOnExit;

    static bool WillDisappearOnExitCore() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
        WindowsExplorerDetector.IsStartedByExplorer();

#if NET
    [SupportedOSPlatform("windows")]
#endif
    static class WindowsExplorerDetector
    {
        public static bool IsStartedByExplorer()
        {
            try
            {
                var parentProcess = Process.GetCurrentProcess().GetParent();
                return
                    parentProcess != null &&
                    string.Equals(
                        Path.GetFileName(parentProcess.GetImageFileName()),
                        "explorer.exe",
                        StringComparison.OrdinalIgnoreCase);
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
        Thread.MemoryBarrier();  // Freshness improvement.
    }
}
