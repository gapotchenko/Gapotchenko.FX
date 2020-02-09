using System;
using System.Collections;
using System.Xml;

namespace Gapotchenko.FX.Data.Xml.XPointer
{
    static class XPointerParser
    {
        private static Hashtable _schemas = XPointerSchema.Schemas;

        public static XPointer ParseXPointer(string xpointer, bool throwOnError)
        {
            ArrayList parts;
            XPointerLexer lexer;
            lexer = new XPointerLexer(xpointer);
            lexer.NextLexeme();
            if (lexer.Kind == XPointerLexer.LexKind.NCName && !lexer.CanBeSchemaName)
            {
                //Shorthand pointer
                XPointer ptr = new ShorthandPointer(lexer.NCName);
                lexer.NextLexeme();
                if (lexer.Kind != XPointerLexer.LexKind.Eof)
                    throw new XPointerSyntaxException("Invalid token after shorthand pointer");
                return ptr;
            }
            else
            {
                //SchemaBased pointer
                parts = new ArrayList();
                while (lexer.Kind != XPointerLexer.LexKind.Eof)
                {
                    if ((lexer.Kind == XPointerLexer.LexKind.NCName ||
                        lexer.Kind == XPointerLexer.LexKind.QName) &&
                        lexer.CanBeSchemaName)
                    {
                        XPointerSchema.SchemaType schemaType = GetSchema(lexer, parts);
                        //Move to '('
                        lexer.NextLexeme();
                        switch (schemaType)
                        {
                            case XPointerSchema.SchemaType.Element:
                                ElementSchemaPointerPart elemPart = ElementSchemaPointerPart.ParseSchemaData(lexer);
                                if (elemPart != null)
                                    parts.Add(elemPart);
                                break;
                            case XPointerSchema.SchemaType.Xmlns:
                                XmlnsSchemaPointerPart xmlnsPart = XmlnsSchemaPointerPart.ParseSchemaData(lexer);
                                if (xmlnsPart != null)
                                    parts.Add(xmlnsPart);
                                break;
                            case XPointerSchema.SchemaType.XPath1:
                                XPath1SchemaPointerPart xpath1Part = XPath1SchemaPointerPart.ParseSchemaData(lexer);
                                if (xpath1Part != null)
                                    parts.Add(xpath1Part);
                                break;
                            case XPointerSchema.SchemaType.XPointer:
                                XPointerSchemaPointerPart xpointerPart = XPointerSchemaPointerPart.ParseSchemaData(lexer);
                                if (xpointerPart != null)
                                    parts.Add(xpointerPart);
                                break;
                            default:
                                //Unknown schema
                                lexer.ParseEscapedData();
                                break;
                        }
                        //Skip ')'
                        lexer.NextLexeme();
                        //Skip possible whitespace
                        if (lexer.Kind == XPointerLexer.LexKind.Space)
                            lexer.NextLexeme();
                    }
                    else
                        throw new XPointerSyntaxException("Invalid token");
                }
                return new SchemaBasedPointer(parts);
            }
        }

        private static XPointerSchema.SchemaType GetSchema(XPointerLexer lexer, ArrayList parts)
        {
            string schemaNSURI;
            if (lexer.Prefix != String.Empty)
            {
                schemaNSURI = null;
                //resolve prefix
                for (int i = parts.Count - 1; i >= 0; i--)
                {
                    PointerPart part = (PointerPart)parts[i];
                    if (part is XmlnsSchemaPointerPart)
                    {
                        XmlnsSchemaPointerPart xmlnsPart = (XmlnsSchemaPointerPart)part;
                        if (xmlnsPart.Prefix == lexer.Prefix)
                        {
                            schemaNSURI = xmlnsPart.Uri;
                            break;
                        }
                    }
                }
                if (schemaNSURI == null)
                    throw new XPointerSyntaxException("Undeclared prefix " + lexer.Prefix);
            }
            else
                schemaNSURI = String.Empty;
            object o = _schemas[schemaNSURI + ':' + lexer.NCName];
            if (o == null)
                return XPointerSchema.SchemaType.Unknown;
            else
                return (XPointerSchema.SchemaType)o;
        }
    }
}
