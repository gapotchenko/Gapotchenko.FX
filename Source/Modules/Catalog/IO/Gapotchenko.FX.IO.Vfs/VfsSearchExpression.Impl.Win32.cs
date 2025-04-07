// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Memory;
using System.Text;

namespace Gapotchenko.FX.IO.Vfs;

partial struct VfsSearchExpression
{
    sealed class Win32Impl(string pattern, char directorySeparatorChar, VfsSearchExpressionOptions options) : IImpl
    {
        public bool IsMatch(ReadOnlySpan<char> input)
        {
            return MatchPattern(
                m_Pattern.AsSpan(),
                input,
                (options & VfsSearchExpressionOptions.IgnoreCase) != 0,
                true);
        }

        readonly string m_Pattern = PrepareExpression(pattern, directorySeparatorChar);

        static string PrepareExpression(string expression, char directorySeparatorChar)
        {
            if (expression == "*")
            {
                // Most common case
                return expression;
            }
            else if (expression.Length == 0 || expression == "." || expression == "*.*")
            {
                // Historically we always treated "." as "*"
                return "*";
            }
            else
            {
                // These all have special meaning in DOS name matching. '\' is the escaping character (which conveniently
                // is the directory separator and cannot be part of any path segment in Windows). The other three are the
                // special case wildcards that we'll convert some * and ? into. They're also valid as filenames on Unix,
                // which is not true in Windows and as such we'll escape any that occur on the input string.
                if (directorySeparatorChar != '\\' && expression.AsSpan().ContainsAny(@"\""<>".AsSpan()))
                {
                    // Backslash isn't the default separator, need to escape (e.g. Unix)
                    expression = expression.Replace("\\", "\\\\");

                    // Also need to escape the other special wild characters ('"', '<', and '>')
                    expression = expression.Replace("\"", "\\\"");
                    expression = expression.Replace(">", "\\>");
                    expression = expression.Replace("<", "\\<");
                }

                // Need to convert the expression to match Win32 behavior
                expression = TranslateWin32Expression(expression);

                return expression;
            }
        }

        /// <summary>
        /// Translates the given Win32 expression. 
        /// Change '*' and '?' to '&lt;', '&gt;' and '"' to match Win32 behavior.
        /// </summary>
        /// <param name="expression">The expression to translate.</param>
        /// <returns>A string with the translated Win32 expression.</returns>
        /// <remarks>
        /// For compatibility, Windows changes some wildcards to provide a closer match to historical DOS 8.3 filename matching.
        /// </remarks>
        public static string TranslateWin32Expression(string expression)
        {
            bool modified = false;

            int length = expression.Length;
            var sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                char c = expression[i];
                switch (c)
                {
                    case '.':
                        modified = true;
                        if (i >= 1 && i == length - 1 && expression[i - 1] == '*')
                        {
                            sb[^1] = '<'; // DOS_STAR (ends in *.)
                        }
                        else if (i < length - 1 && (expression[i + 1] == '?' || expression[i + 1] == '*'))
                        {
                            sb.Append('\"'); // DOS_DOT
                        }
                        else
                        {
                            sb.Append('.');
                        }
                        break;

                    case '?':
                        modified = true;
                        sb.Append('>'); // DOS_QM
                        break;

                    default:
                        sb.Append(c);
                        break;
                }
            }

            if (modified)
                return sb.ToString();
            else
                return expression;
        }
    }
}
