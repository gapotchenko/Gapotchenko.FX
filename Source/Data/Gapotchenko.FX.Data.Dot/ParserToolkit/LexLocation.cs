namespace Gapotchenko.FX.Data.Dot.ParserToolkit
{
    /// <summary>
    /// Represents the location information.
    /// </summary>
    public sealed class LexLocation : IMerge<LexLocation>
    {
        /// <summary>
        /// The line at which the text span starts.
        /// </summary>
        public int StartLine { get; }

        /// <summary>
        /// The column at which the text span starts.
        /// </summary>
        public int StartColumn { get; }

        /// <summary>
        /// The line on which the text span ends.
        /// </summary>
        public int EndLine { get; }

        /// <summary>
        /// The column of the first character
        /// beyond the end of the text span.
        /// </summary>
        public int EndColumn { get; }

        /// <summary>
        /// Creates a new <see cref="LexLocation"/> instance with default content.
        /// </summary>
        public LexLocation()
        {
        }

        /// <summary>
        /// Creates a new <see cref="LexLocation"/> instance with provided content.
        /// </summary>
        public LexLocation(int startLine, int startColumn, int endLine, int endColumn)
        {
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
        }

        /// <inheritdoc />
        public LexLocation Merge(LexLocation last) =>
            new(StartLine, StartColumn, last.EndLine, last.EndColumn);
    }
}
