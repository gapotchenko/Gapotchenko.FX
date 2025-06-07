using Gapotchenko.FX.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Gapotchenko.FX.Console;

using Console = System.Console;

/// <summary>
/// Provides extended functionality for console.
/// </summary>
public static class ConsoleEx
{
    /// <summary>
    /// Enables Unicode support for the console if it is not enabled yet.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if console Unicode support is enabled;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool EnableUnicode()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (Console.OutputEncoding.IsUnicodeScheme)
                return true;

            if (Environment.OSVersion.Version.Major >= 10)
            {
                try
                {
                    if (IsUnicodeConsoleFont())
                    {
                        var encoding = Encoding.UTF8;
                        Console.OutputEncoding = encoding;
                        Console.InputEncoding = encoding;
                        return true;
                    }
                }
                catch (Exception e) when (!e.IsControlFlowException())
                {
                    Debug.WriteLine(e);
                }
            }

            return false;
        }
        else
        {
            return Console.OutputEncoding.IsUnicodeScheme;
        }
    }

#if NET
    [SupportedOSPlatform("windows")]
#endif
    static bool IsUnicodeConsoleFont()
    {
        IntPtr handle = NativeMethods.GetStdHandle(NativeMethods.STD_OUTPUT_HANDLE);
        if (handle == NativeMethods.INVALID_HANDLE_VALUE)
            return false;

        var info = new NativeMethods.CONSOLE_FONT_INFO_EX();
        info.cbSize = Marshal.SizeOf(info);
        if (!NativeMethods.GetCurrentConsoleFontEx(handle, false, ref info))
            return false;

        return (info.FontFamily & NativeMethods.TMPF_TRUETYPE) != 0;
    }

    /// <summary>
    /// Reads a password.
    /// </summary>
    /// <returns>The password.</returns>
    public static SecureString ReadPassword() => ReadPassword(default);

    /// <summary>
    /// Reads a password by using a specified masking character.
    /// </summary>
    /// <param name="mask">The masking character.</param>
    /// <returns>The password.</returns>
    public static SecureString ReadPassword(char mask) => ReadPassword(mask, Console.Error);

    static SecureString ReadPassword(char mask, TextWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);

        var password = new SecureString();

        for (; ; )
        {
            char c = Console.ReadKey(true).KeyChar;
            if (c == 13)
            {
                writer.WriteLine();
                break;
            }

            switch (c)
            {
                case (char)8: // Backspace
                case (char)127: // Ctrl+Backspace
                    if (password.Length == 0)
                    {
                        // Ignore backspace on an empty password.
                    }
                    else
                    {
                        writer.Write("\b \b");
                        password.RemoveAt(password.Length - 1);
                    }
                    break;

                case (char)0:
                case (char)9:
                case (char)10:
                case (char)27:
                    // Ignore filtered characters.
                    break;

                default:
                    password.AppendChar(c);
                    if (mask != 0)
                        writer.Write(mask);
                    break;
            }
        }

        password.MakeReadOnly();

        return password;
    }
}
