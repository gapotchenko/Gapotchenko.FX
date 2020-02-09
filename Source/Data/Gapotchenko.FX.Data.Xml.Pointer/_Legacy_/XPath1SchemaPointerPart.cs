using System;
using System.Xml;

namespace Gapotchenko.FX.Data.Xml.Pointer._Legacy_
{
    /// <summary>
    /// xpath1() schema based XPointer pointer part.
    /// </summary>
    internal class XPath1SchemaPointerPart : PointerPart
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

        public static XPath1SchemaPointerPart ParseSchemaData(XPointerLexer lexer)
        {
            XPath1SchemaPointerPart part = new XPath1SchemaPointerPart();
            try
            {
                part.XPath = lexer.ParseEscapedData();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Syntax error in xpath1() schema data: " + e.Message);
                return null;
            }
            return part;
        }
    }
}
