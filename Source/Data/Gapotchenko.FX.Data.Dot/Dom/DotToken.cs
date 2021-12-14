using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a token in the syntax tree.
    /// </summary>
    public abstract class DotToken
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string _text;

        /// <summary>
        /// Initializes a new <see cref="DotToken"/> instance.
        /// </summary>
        /// <param name="kind">Token kind.</param>
        /// <param name="text">Token text.</param>
        protected internal DotToken(DotTokenKind kind, string text)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            Kind = kind;
            _text = text;
        }

        /// <summary>
        /// Gets the token kind.
        /// </summary>
        public DotTokenKind Kind { get; }

        /// <summary>
        /// Gets or sets the token text.
        /// </summary>
        public virtual string Text
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
