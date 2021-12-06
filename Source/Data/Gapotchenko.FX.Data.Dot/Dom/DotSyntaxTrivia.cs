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
    public readonly struct DotSyntaxTrivia
    {
        /// <summary>
        /// Creates <see cref="DotSyntaxTrivia"/> instance.
        /// </summary>
        /// <param name="kind">Trivia kind.</param>
        /// <param name="value">Trivia value.</param>
        public DotSyntaxTrivia(DotToken kind, string value)
        {
            Kind = kind;
            Value = value;
        }

        /// <summary>
        /// Trivia kind.
        /// </summary>
        public DotToken Kind { get; }

        /// <summary>
        /// Trivia value.
        /// </summary>
        public string Value { get; }
    }
}
