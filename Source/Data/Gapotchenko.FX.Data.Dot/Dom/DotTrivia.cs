using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a trivia in the syntax tree.
    /// </summary>
    public readonly struct DotTrivia
    {
        /// <summary>
        /// Initializes a new <see cref="DotTrivia"/> instance.
        /// </summary>
        /// <param name="kind">Trivia kind.</param>
        /// <param name="text">Trivia text.</param>
        public DotTrivia(DotTokenKind kind, string? text = default)
        {
            if (text is null &&
                !kind.TryGetDefaultValue(out text))
            {
                throw new ArgumentException("Value cannot deducted from the kind.", nameof(text));
            }

            Kind = kind;
            Text = text;
        }

        /// <summary>
        /// Trivia kind.
        /// </summary>
        public DotTokenKind Kind { get; }

        /// <summary>
        /// Trivia text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Returns the string representation of this trivia.
        /// </summary>
        public override string ToString() =>
            Text ?? string.Empty;
    }
}
