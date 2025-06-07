using System.Security;

namespace Gapotchenko.FX.Console;

using Console = System.Console;

/// <summary>
/// Provides extended functionality for console.
/// </summary>
public static class ConsoleEx
{
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
