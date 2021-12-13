using System.Collections.Generic;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents an element in the syntax tree.
    /// </summary>
    public interface DotElement
    {
        /// <summary>
        /// Gets the list of trivia appearing before the element.
        /// </summary>
        IList<DotInsignificantToken> LeadingTrivia { get; }

        /// <summary>
        /// Gets the list of trivia appearing after the element.
        /// </summary>
        IList<DotInsignificantToken> TrailingTrivia { get; }

        /// <summary>
        /// Gets a value indicating whether the element has any leading trivia.
        /// </summary>
        bool HasLeadingTrivia { get; }

        /// <summary>
        /// Gets a value indicating whether the element has any trailing trivia.
        /// </summary>
        bool HasTrailingTrivia { get; }
    }
}
