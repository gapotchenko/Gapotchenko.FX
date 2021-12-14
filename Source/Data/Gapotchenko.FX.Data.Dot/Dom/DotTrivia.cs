using Gapotchenko.FX.Data.Dot.Serialization;
using System;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a trivia in the syntax tree.
    /// </summary>
    public sealed class DotTrivia : DotToken
    {
        /// <summary>
        /// Initializes a new <see cref="DotTrivia"/> instance.
        /// </summary>
        /// <param name="kind">Token kind.</param>
        /// <param name="text">Token text.</param>
        public DotTrivia(DotTokenKind kind, string text)
            : base(kind, text)
        {
            switch (kind)
            {
                case DotTokenKind.Whitespace:
                case DotTokenKind.Comment:
                case DotTokenKind.MultilineComment:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind));
            }
        }
    }
}
