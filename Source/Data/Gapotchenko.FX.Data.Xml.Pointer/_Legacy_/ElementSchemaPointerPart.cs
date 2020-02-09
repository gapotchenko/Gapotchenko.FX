using System;
using System.Collections;
using System.Xml;
using System.Text;

namespace Gapotchenko.FX.Data.Xml.Pointer._Legacy_
{
    /// <summary>
    /// element() schema based XPointer pointer part.
    /// </summary>
    internal class ElementSchemaPointerPart : PointerPart
    {
        private string _NCName;
        private string _xpath;
        private ArrayList _childSequence;

        /// <summary>
        /// Leading NCName, see http://www.w3.org/TR/xptr-element/#model
        /// </summary>
        public string NCName
        {
            get { return _NCName; }
            set { _NCName = value; }
        }

        /// <summary>
        /// Equivalent XPath expression.
        /// </summary>
        public string XPath
        {
            get { return _xpath; }
            set { _xpath = value; }
        }

        /// <summary>
        /// ChildSequence as per http://www.w3.org/TR/xptr-element/#NT-ChildSequence
        /// </summary>
        public ArrayList ChildSequence
        {
            get { return _childSequence; }
            set { _childSequence = value; }
        }

        /// <summary>
        /// Evaluates this pointer part. 
        /// </summary>
        /// <param name="doc">Document to evaluate pointer part on.</param>
        /// <param name="nm">Current namespace manager.</param>
        /// <returns>Pointed nodes</returns>
        public override XmlNodeList Evaluate(XmlDocument doc, XmlNamespaceManager nm)
        {
            return doc.SelectNodes(_xpath, nm);
        }

        /// <summary>
        /// Parses element() based pointer part and builds instance of <c>ElementSchemaPointerPart</c> class.
        /// </summary>
        /// <param name="lexer">Lexical analizer.</param>
        /// <returns>Newly created <c>ElementSchemaPointerPart</c> object.</returns>
        public static ElementSchemaPointerPart ParseSchemaData(XPointerLexer lexer)
        {
            //Productions:
            //[1]   	ElementSchemeData	   ::=   	(NCName ChildSequence?) | ChildSequence
            //[2]   	ChildSequence	   ::=   	('/' [1-9] [0-9]*)+                        
            StringBuilder xpathBuilder = new StringBuilder();
            ElementSchemaPointerPart part = new ElementSchemaPointerPart();
            lexer.NextLexeme();
            if (lexer.Kind == XPointerLexer.LexKind.NCName)
            {
                part.NCName = lexer.NCName;
                xpathBuilder.Append("id('");
                xpathBuilder.Append(lexer.NCName);
                xpathBuilder.Append("')");
                lexer.NextLexeme();
            }
            part.ChildSequence = new ArrayList();
            while (lexer.Kind == XPointerLexer.LexKind.Slash)
            {
                lexer.NextLexeme();
                if (lexer.Kind != XPointerLexer.LexKind.Number)
                {
                    Console.Error.WriteLine("Syntax error in element() schema data: Invalid token in ChildSequence");
                    return null;
                }
                if (lexer.Number == 0)
                {
                    Console.Error.WriteLine("Syntax error in element() schema data: 0 index ChildSequence");
                    return null;
                }
                part.ChildSequence.Add(lexer.Number);
                xpathBuilder.Append("/*[");
                xpathBuilder.Append(lexer.Number);
                xpathBuilder.Append("]");
                lexer.NextLexeme();
            }
            if (lexer.Kind != XPointerLexer.LexKind.RRBracket)
            {
                Console.Error.WriteLine("Syntax error in element() schema data: Invalid token in ChildSequence");
                return null;
            }
            if (part.NCName == null && part.ChildSequence.Count == 0)
            {
                Console.Error.WriteLine("Syntax error in element() schema data: empty XPointer");
                return null;
            }
            part.XPath = xpathBuilder.ToString();
            return part;
        }
    }
}
