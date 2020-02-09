using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

#nullable enable

namespace Gapotchenko.FX.Data.Xml.Pointer
{
    /// <summary>
    /// The abstract XPointer definition.
    /// </summary>
    public abstract class XPointer
    {
        /// <summary>
        /// Evaluates XPointer and returns a list of selected nodes.
        /// </summary>
        /// <param name="document">Document to evaluate the XPointer on.</param>
        /// <returns>The list of selected nodes.</returns>	    					
        public abstract XmlNodeList Evaluate(XmlDocument document);

        /// <summary>
        /// Evaluates XPointer and returns a sequence of selected nodes.
        /// </summary>
        /// <param name="document">Document to evaluate the XPointer on.</param>
        /// <returns>The sequence of selected nodes.</returns>	    					
        public abstract IEnumerable<XNode> Evaluate(XDocument document);

        /// <summary>
        /// Converts string representation of an XPointer to an equivalent <see cref="XPointer"/> object.
        /// </summary>
        /// <param name="input">A string containing an XPointer to convert.</param>
        /// <returns>An object that is equivalent to the XPointer contained in <paramref name="input"/>.</returns>
        public static XPointer Parse(string input) => XPointerParser.ParseXPointer(input, true);

        /// <summary>
        /// Converts string representation of an XPointer to an equivalent <see cref="XPointer"/> object
        /// and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="input">A string containing an XPointer to convert.</param>
        /// <param name="result">
        /// <para>
        /// When this method returns,
        /// contains the <see cref="XPointer"/> value equivalent to the string representation contained in <paramref name="input"/>, if the conversion succeeded,
        /// or <c>null</c> if the conversion failed.
        /// </para>
        /// <para>
        /// The conversion fails if the <paramref name="input"/> parameter is <c>null</c>,
        /// is an empty string (""),
        /// or does not contain a valid string representation of an XPointer.
        /// </para>
        /// </param>
        /// <returns><c>true</c> if the <paramref name="input"/> parameter was converted successfully; otherwise, <c>false</c>.</returns>
        public static bool TryParse(string? input, out XPointer? result)
        {
            var xptr = TryParse(input);
            result = xptr;
            return xptr != null;
        }

        /// <summary>
        /// Converts string representation of an XPointer to an equivalent <see cref="XPointer"/> object
        /// and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="input">A string containing an XPointer to convert.</param>
        /// <returns>
        /// <para>
        /// An object that is equivalent to the XPointer contained in <paramref name="input"/>, if the conversion succeeded,
        /// or <c>null</c> if the conversion failed.
        /// </para>
        /// The conversion fails if the <paramref name="input"/> parameter is <c>null</c>,
        /// is an empty string (""),
        /// or does not contain a valid string representation of an XPointer.
        /// <para>
        /// </para>
        /// </returns>
        public static XPointer? TryParse(string? input) => XPointerParser.ParseXPointer(input, false);
    }
}
