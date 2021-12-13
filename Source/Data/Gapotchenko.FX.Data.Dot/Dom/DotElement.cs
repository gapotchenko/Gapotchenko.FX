using System.Collections.Generic;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents an element in the syntax tree.
    /// </summary>
    public abstract class DotElement
    {
        /// <summary>
        /// Gets the list of trivia appearing before the element.
        /// </summary>
        public abstract IList<DotInsignificantToken> LeadingTrivia { get; }

        /// <summary>
        /// Gets the list of trivia appearing after the element.
        /// </summary>
        public abstract IList<DotInsignificantToken> TrailingTrivia { get; }

        /// <summary>
        /// Gets a value indicating whether the element has any leading trivia.
        /// </summary>
        public abstract bool HasLeadingTrivia { get; }

        /// <summary>
        /// Gets a value indicating whether the element has any trailing trivia.
        /// </summary>
        public abstract bool HasTrailingTrivia { get; }
    }
}
