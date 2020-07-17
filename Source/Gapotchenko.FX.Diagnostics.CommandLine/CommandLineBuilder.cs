using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

#nullable enable

namespace Gapotchenko.FX.Diagnostics
{
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
        /// If value is <c>null</c>, the new <see cref="CommandLineBuilder"/> will be empty.
        /// </param>
        public CommandLineBuilder(string? commandLine)
        {
            m_CommandLine = new StringBuilder(commandLine);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        StringBuilder m_CommandLine;

        /// <summary>
        /// Appends a specified <see cref="String"/> command line argument to this instance.
        /// The argument text is automatically quoted according to the command line rules.
        /// </summary>
        /// <param name="value">The <see cref="String"/> command line argument to append.</param>
        /// <returns>The instance of command line builder.</returns>
        public CommandLineBuilder AppendArgument(string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _DelimitArguments();
                _AppendTextWithQuoting(value);
            }
            return this;
        }

        /// <summary>
        /// Appends a specified <see cref="Char"/> command line argument to this instance.
        /// The argument text is automatically quoted according to the command line rules.
        /// </summary>
        /// <param name="value">The <see cref="Char"/> command line argument to append.</param>
        /// <returns>The instance of command line builder.</returns>
        public CommandLineBuilder AppendArgument(char value) => AppendArgument(value.ToString());

        /// <summary>
        /// Appends a specified <see cref="Byte"/> command line argument to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Byte"/> command line argument to append.</param>
        /// <returns>The instance of command line builder.</returns>
        public CommandLineBuilder AppendArgument(byte value) => AppendArgument(value.ToString());

        /// <summary>
        /// Appends a specified <see cref="SByte"/> command line argument to this instance.
        /// </summary>
        /// <param name="value">The <see cref="SByte"/> command line argument to append.</param>
        /// <returns>The instance of command line builder.</returns>
        [CLSCompliant(false)]
        public CommandLineBuilder AppendArgument(sbyte value) => AppendArgument(value.ToString());

        /// <summary>
        /// Appends a specified <see cref="Int16"/> command line argument to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Int16"/> command line argument to append.</param>
        /// <returns>The instance of command line builder.</returns>
        public CommandLineBuilder AppendArgument(short value) => AppendArgument(value.ToString());

        /// <summary>
        /// Appends a specified <see cref="UInt16"/> command line argument to this instance.
        /// </summary>
        /// <param name="value">The <see cref="UInt16"/> command line argument to append.</param>
        /// <returns>The instance of command line builder.</returns>
        [CLSCompliant(false)]
        public CommandLineBuilder AppendArgument(ushort value) => AppendArgument(value.ToString());

        /// <summary>
        /// Appends a specified <see cref="Int32"/> command line argument to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Int32"/> command line argument to append.</param>
        /// <returns>The instance of command line builder.</returns>
        public CommandLineBuilder AppendArgument(int value) => AppendArgument(value.ToString());

        /// <summary>
        /// Appends a specified <see cref="UInt32"/> command line argument to this instance.
        /// </summary>
        /// <param name="value">The <see cref="UInt32"/> command line argument to append.</param>
        /// <returns>The instance of command line builder.</returns>
        [CLSCompliant(false)]
        public CommandLineBuilder AppendArgument(uint value) => AppendArgument(value.ToString());

        /// <summary>
        /// Appends a specified <see cref="Int64"/> command line argument to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Int64"/> command line argument to append.</param>
        /// <returns>The instance of command line builder.</returns>
        public CommandLineBuilder AppendArgument(long value) => AppendArgument(value.ToString());

        /// <summary>
        /// Appends a specified <see cref="UInt64"/> command line argument to this instance.
        /// </summary>
        /// <param name="value">The <see cref="UInt64"/> command line argument to append.</param>
        /// <returns>The instance of command line builder.</returns>
        [CLSCompliant(false)]
        public CommandLineBuilder AppendArgument(ulong value) => AppendArgument(value.ToString());

        /// <summary>
        /// Appends a specified <see cref="Object"/> command line argument to this instance.
        /// The argument text is automatically quoted according to the command line rules.
        /// </summary>
        /// <param name="value">The <see cref="Object"/> command line argument to append.</param>
        /// <returns>The instance of command line builder.</returns>
        public CommandLineBuilder AppendArgument(object? value) => AppendArgument(value?.ToString());

        /// <summary>
        /// Appends a specified command line argument that represents a file name.
        /// The file name is automatically quoted according to the command line rules.
        /// </summary>
        /// <param name="value">The command line argument that represents a file name to append.</param>
        /// <returns>The instance of command line builder.</returns>
        public CommandLineBuilder AppendFileName(string? value) => AppendArgument(CommandLine.Escape.EncodeFileName(value));

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
        /// Delimits command line arguments by ensuring that either the last appended character is a whitespace or the built command line is empty.
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
            _DelimitArguments();
            return this;
        }

        void _DelimitArguments()
        {
            var commandLine = m_CommandLine;

            if (commandLine.Length != 0 && commandLine[commandLine.Length - 1] != CommandLine.ArgumentSeparator)
                commandLine.Append(CommandLine.ArgumentSeparator);
        }

        void _AppendTextWithQuoting(string text) => CommandLine.Escape.AppendQuotedText(m_CommandLine, text);

        /// <summary>
        /// Converts the value of this instance to a <see cref="System.String"/>.
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
        public override string ToString() => m_CommandLine.ToString();
    }
}
