using System;
using System.Xml;

namespace Gapotchenko.FX.Data.Xml.Pointer._Legacy_
{
    /// <summary>
    /// xmlns() schema based XPointer pointer part.
    /// </summary>
    internal class XmlnsSchemaPointerPart : PointerPart
    {
        private string _prefix, _uri;

        public XmlnsSchemaPointerPart(string prefix, string uri)
        {
            _prefix = prefix;
            _uri = uri;
        }

        public string Prefix
        {
            get { return _prefix; }
        }

        public string Uri
        {
            get { return _uri; }
        }

        public override XmlNodeList Evaluate(XmlDocument doc, XmlNamespaceManager nm)
        {
            nm.AddNamespace(_prefix, _uri);
            return null;
        }

        public static XmlnsSchemaPointerPart ParseSchemaData(XPointerLexer lexer)
        {
            //[1]   	XmlnsSchemeData	   ::=   	 NCName S? '=' S? EscapedNamespaceName
            //[2]   	EscapedNamespaceName	   ::=   	EscapedData*                      	                    
            //Read prefix as NCName
            lexer.NextLexeme();
            if (lexer.Kind != XPointerLexer.LexKind.NCName)
            {
                Console.Error.WriteLine("Syntax error in xmlns() schema data: Invalid token in XmlnsSchemaData");
                return null;
            }
            string prefix = lexer.NCName;
            lexer.SkipWhiteSpace();
            lexer.NextLexeme();
            if (lexer.Kind != XPointerLexer.LexKind.Eq)
            {
                Console.Error.WriteLine("Syntax error in xmlns() schema data: Invalid token in XmlnsSchemaData");
                return null;
            }
            lexer.SkipWhiteSpace();
            string nsURI;
            try
            {
                nsURI = lexer.ParseEscapedData();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Syntax error in xmlns() schema data: " + e.Message);
                return null;
            }
            return new XmlnsSchemaPointerPart(prefix, nsURI);
        }

    }
}
