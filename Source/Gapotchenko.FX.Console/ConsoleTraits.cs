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
    /// A tristate atomic data type for thread-safe representation of a nullable boolean value in accordance with .NET memory model.
    /// </summary>
    enum AtomicNullableBool : byte
    {
        Null,
        False,
        True
    }

    static AtomicNullableBool m_CachedIsColorInhibited;

    /// <summary>
    /// <para>
    /// Gets a value indicating whether console color output is inhibited.
    /// </para>
    /// <para>
    /// Console color may be inhibited by the host system or a user preference.
    /// For example, a NO_COLOR environment variable can be used to inhibit console colors as described by corresponding <a href="https://no-color.org/">specification</a>.
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

    // This cached value should not be discarded as it represents an immutable trait.
    static AtomicNullableBool m_CachedWillDisappearOnExit;

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
