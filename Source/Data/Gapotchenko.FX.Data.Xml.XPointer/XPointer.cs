using System;
using System.Xml;
using System.Xml.Linq;

namespace Gapotchenko.FX.Data.Xml.XPointer
{
    /// <summary>
    /// The abstract XPointer definition.
    /// </summary>
    public abstract class XPointer
    {
        /// <summary>
        /// Evaluates XPointer and returns the selected nodes.
        /// </summary>
        /// <param name="document">Document to evaluate the XPointer on.</param>
        /// <returns>The list of selected nodes.</returns>	    					
        public abstract XmlNodeList Evaluate(XmlDocument document);

        // TODO
        //public abstract IEnumerable<XNode> Evaluate(XDocument document);
    }
}
