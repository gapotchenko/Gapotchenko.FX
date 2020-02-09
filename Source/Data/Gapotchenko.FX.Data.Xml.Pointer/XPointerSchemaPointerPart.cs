using System;
using System.Xml;

namespace Gapotchenko.FX.Data.Xml.Pointer
{
    /// <summary>
    /// xpointer() schema based XPointer pointer part.
    /// </summary>
    internal class XPointerSchemaPointerPart : PointerPart
    {
        private string _xpath;

        public string XPath
        {
            get { return _xpath; }
            set { _xpath = value; }
        }

        public override XmlNodeList Evaluate(XmlDocument doc, XmlNamespaceManager nm)
        {
            try
            {
                return doc.SelectNodes(_xpath, nm);
            }
            catch
            {
                return null;
            }
        }

        public static XPointerSchemaPointerPart ParseSchemaData(XPointerLexer lexer)
        {
            XPointerSchemaPointerPart part = new XPointerSchemaPointerPart();
            try
            {
                part.XPath = lexer.ParseEscapedData();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Syntax error in xpointer() schema data: " + e.Message);
                return null;
            }
            return part;
        }
    }
}
