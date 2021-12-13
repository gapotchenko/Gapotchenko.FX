using System;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents an insignificant token in the syntax tree.
    /// </summary>
    public sealed class DotInsignificantToken
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string _text;

        /// <summary>
        /// Initializes a new <see cref="DotInsignificantToken"/> instance.
        /// </summary>
        /// <param name="kind">Token kind.</param>
        /// <param name="text">Token text.</param>
        public DotInsignificantToken(DotInsignificantTokenKind kind, string text)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            Kind = kind;
            _text = text;
        }

        /// <summary>
        /// Token kind.
        /// </summary>
        public DotInsignificantTokenKind Kind { get; }

        /// <summary>
        /// Trivia text.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                _text = value;
            }
        }
        /// <summary>
        /// Returns the string representation of this token.
        /// </summary>
        public override string ToString() => _text;
    }
}
