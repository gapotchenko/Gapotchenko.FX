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
        /// <param name="value">Trivia value.</param>
        public DotTrivia(DotTokenKind kind, string? value = default)
        {
            if (value is null &&
                !kind.TryGetDefaultValue(out value))
            {
                throw new ArgumentException("Value cannot deducted from the kind.", nameof(value));
            }

            Kind = kind;
            Value = value;
        }

        /// <summary>
        /// Trivia kind.
        /// </summary>
        public DotTokenKind Kind { get; }

        /// <summary>
        /// Trivia value.
        /// </summary>
        public string Value { get; }
    }
}
