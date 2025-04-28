using System.Diagnostics;
using System.Text;

namespace Gapotchenko.FX.Diagnostics;

/// <summary>
/// Represents a mutable string of command line parameters.
/// </summary>
public sealed class CommandLineBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLineBuilder"/> class.
    /// </summary>
    public CommandLineBuilder()
    {
        m_CommandLine = new StringBuilder();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLineBuilder"/> class using the specified command line.
    /// </summary>
    /// <param name="commandLine">
    /// The command line used to initialize the value of the instance.
    /// If value is <see langword="null"/>, the new <see cref="CommandLineBuilder"/> will be empty.
    /// </param>
    public CommandLineBuilder(string? commandLine)
    {
        m_CommandLine = new StringBuilder(commandLine);
    }

    /// <summary>
    /// Appends a specified <see cref="char"/> command-line argument to this instance.
    /// The argument text is automatically quoted according to the command line rules.
    /// </summary>
    /// <param name="value">The <see cref="char"/> command-line argument to append.</param>
    /// <returns>The instance of command line builder.</returns>
    public CommandLineBuilder AppendArgument(char value) => AppendArgument(value.ToString());

    /// <summary>
    /// Appends a specified <see cref="byte"/> command-line argument to this instance.
    /// </summary>
    /// <param name="value">The <see cref="byte"/> command-line argument to append.</param>
    /// <returns>The instance of command line builder.</returns>
    public CommandLineBuilder AppendArgument(byte value) => AppendArgument(value.ToString());

    /// <summary>
    /// Appends a specified <see cref="sbyte"/> command-line argument to this instance.
    /// </summary>
    /// <param name="value">The <see cref="sbyte"/> command-line argument to append.</param>
    /// <returns>The instance of command line builder.</returns>
    [CLSCompliant(false)]
    public CommandLineBuilder AppendArgument(sbyte value) => AppendArgument(value.ToString());

    /// <summary>
    /// Appends a specified <see cref="short"/> command-line argument to this instance.
    /// </summary>
    /// <param name="value">The <see cref="short"/> command-line argument to append.</param>
    /// <returns>The instance of command line builder.</returns>
    public CommandLineBuilder AppendArgument(short value) => AppendArgument(value.ToString());

    /// <summary>
    /// Appends a specified <see cref="ushort"/> command-line argument to this instance.
    /// </summary>
    /// <param name="value">The <see cref="ushort"/> command-line argument to append.</param>
    /// <returns>The instance of command line builder.</returns>
    [CLSCompliant(false)]
    public CommandLineBuilder AppendArgument(ushort value) => AppendArgument(value.ToString());

    /// <summary>
    /// Appends a specified <see cref="int"/> command-line argument to this instance.
    /// </summary>
    /// <param name="value">The <see cref="int"/> command-line argument to append.</param>
    /// <returns>The instance of command line builder.</returns>
    public CommandLineBuilder AppendArgument(int value) => AppendArgument(value.ToString());

    /// <summary>
    /// Appends a specified <see cref="uint"/> command-line argument to this instance.
    /// </summary>
    /// <param name="value">The <see cref="uint"/> command-line argument to append.</param>
    /// <returns>The instance of command line builder.</returns>
    [CLSCompliant(false)]
    public CommandLineBuilder AppendArgument(uint value) => AppendArgument(value.ToString());

    /// <summary>
    /// Appends a specified <see cref="long"/> command-line argument to this instance.
    /// </summary>
    /// <param name="value">The <see cref="long"/> command-line argument to append.</param>
    /// <returns>The instance of command line builder.</returns>
    public CommandLineBuilder AppendArgument(long value) => AppendArgument(value.ToString());

    /// <summary>
    /// Appends a specified <see cref="ulong"/> command-line argument to this instance.
    /// </summary>
    /// <param name="value">The <see cref="ulong"/> command-line argument to append.</param>
    /// <returns>The instance of command line builder.</returns>
    [CLSCompliant(false)]
    public CommandLineBuilder AppendArgument(ulong value) => AppendArgument(value.ToString());

    /// <summary>
    /// Appends a specified <see cref="object"/> command-line argument to this instance.
    /// The argument text is automatically quoted according to the command line rules.
    /// </summary>
    /// <param name="value">The <see cref="object"/> command-line argument to append.</param>
    /// <returns>The instance of command line builder.</returns>
    public CommandLineBuilder AppendArgument(object? value) => AppendArgument(value?.ToString());

    /// <summary>
    /// Appends a specified command-line argument that represents a file name.
    /// The file name is automatically quoted according to the command line rules.
    /// </summary>
    /// <param name="value">The command-line argument that represents a file name to append.</param>
    /// <returns>The instance of command line builder.</returns>
    public CommandLineBuilder AppendFileName(string? value) => AppendArgument(CommandLine.Escape.EncodeFileName(value));

    /// <summary>
    /// Appends a specified <see cref="string"/> command-line argument to this instance.
    /// The argument text is automatically quoted according to the command line rules.
    /// </summary>
    /// <param name="value">The <see cref="string"/> command-line argument to append.</param>
    /// <returns>The instance of command line builder.</returns>
    public CommandLineBuilder AppendArgument(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            DelimitArgumentsCore();
            CommandLine.Escape.AppendQuotedText(m_CommandLine, value);
        }
        return this;
    }

    /// <summary>
    /// Removes all characters from the current <see cref="CommandLineBuilder"/> instance.
    /// </summary>
    /// <returns>The instance of command line builder.</returns>
    public CommandLineBuilder Clear()
    {
        m_CommandLine.Clear();
        return this;
    }

    /// <summary>
    /// <para>
    /// Gets the raw string builder for the command line.
    /// </para>
    /// <para>
    /// The purpose of raw access is to cover non-conventional notations and scenarios of a command line.
    /// </para>
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public StringBuilder Raw => m_CommandLine;

    /// <summary>
    /// <para>
    /// Delimits command-line arguments by ensuring that either the last appended character is a whitespace or the built command line is empty.
    /// </para>
    /// <para>
    /// This method should be used in conjunction with <see cref="Raw"/> property.
    /// There is no need to call the method when conventional methods like <see cref="AppendArgument(string)"/> are used.
    /// </para>
    /// </summary>
    /// <returns>The instance of command line builder.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public CommandLineBuilder DelimitArguments()
    {
        DelimitArgumentsCore();
        return this;
    }

    void DelimitArgumentsCore()
    {
        var commandLine = m_CommandLine;

        if (commandLine.Length != 0 && commandLine[^1] != CommandLine.ArgumentSeparator)
            commandLine.Append(CommandLine.ArgumentSeparator);
    }

    /// <summary>
    /// Converts the value of this instance to a <see cref="string"/>.
    /// </summary>
    /// <returns>A string whose value is the same as this instance.</returns>
    public override string ToString() => m_CommandLine.ToString();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly StringBuilder m_CommandLine;
}
