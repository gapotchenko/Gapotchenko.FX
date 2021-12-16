using Gapotchenko.FX.Data.Dot.Serialization;
using System;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a DOT document.
    /// </summary>
    public class DotDocument
    {
        /// <summary>
        /// Gets or sets a document root.
        /// </summary>
        public DotGraphNode? Root { get; set; }

        /// <summary>
        /// Loads a DOT document from the given <see cref="DotReader" />.
        /// </summary>
        public static DotDocument Load(DotReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var parser = new DotParser(reader);
            parser.Parse();
            var root = parser.Root;
            if (root is null)
                throw new InvalidOperationException("Cannot parse DOT document.");

            return new DotDocument
            {
                Root = root,
            };
        }

        /// <summary>
        /// Saves a DOT document to the given <see cref="DotWriter"/>.
        /// </summary>
        /// <param name="writer"></param>
        public void Save(DotWriter writer)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            if (Root is not null)
            {
                var domWriter = new DotDomWriter(writer);
                Root.Accept(domWriter);
            }
        }

        /// <summary>
        /// Returns the string representation of this document.
        /// </summary>
        public override string ToString()
        {
            return Root?.ToString() ?? string.Empty;
        }
    }
}
