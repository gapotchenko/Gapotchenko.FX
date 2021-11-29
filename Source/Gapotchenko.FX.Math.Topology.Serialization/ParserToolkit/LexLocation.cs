namespace Gapotchenko.FX.Math.Topology.Serialization.ParserToolkit
{
    /// <summary>
    /// This is the default class that carries location
    /// information from the scanner to the parser.
    /// If you don't declare "%YYLTYPE Foo" the parser
    /// will expect to deal with this type.
    /// </summary>
    sealed class LexLocation : IMerge<LexLocation>
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

        public LexLocation()
        {
        }

        public LexLocation(int sl, int sc, int el, int ec)
        {
            StartLine = sl;
            StartColumn = sc;
            EndLine = el;
            EndColumn = ec;
        }

        /// <inheritdoc />
        public LexLocation Merge(LexLocation last) =>
            new LexLocation(StartLine, StartColumn, last.EndLine, last.EndColumn);
    }
}
