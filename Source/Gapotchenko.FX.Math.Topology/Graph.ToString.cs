using System;
using System.IO;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T> : IFormattable
    {
        /// <inheritdoc/>
        public virtual string ToString(string? format, IFormatProvider? formatProvider = null)
        {
            if (format == "D")
            {
                // GraphVis DOT format.
                var writer = new StringWriter();
                var serializer = new GraphVizDotSerializer
                {
                    FormatProvider = formatProvider
                };
                serializer.Serialize(this, writer);
                return writer.ToString();
            }

            return ToString() ?? string.Empty;
        }
    }
}
