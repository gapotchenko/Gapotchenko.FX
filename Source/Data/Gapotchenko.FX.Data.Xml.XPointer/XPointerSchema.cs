using System;
using System.Collections;

namespace Gapotchenko.FX.Data.Xml.XPointer
{
    /// <summary>
    /// XPointer schema.
    /// </summary>
    class XPointerSchema
    {
        internal enum SchemaType
        {
            Element,
            Xmlns,
            XPath1,
            XPointer,
            Unknown
        }
        internal static Hashtable Schemas = CreateSchemasTable();

        internal static Hashtable CreateSchemasTable()
        {
            Hashtable table = new Hashtable();
            //<namespace uri>:<ncname>
            table.Add(":element", SchemaType.Element);
            table.Add(":xmlns", SchemaType.Xmlns);
            table.Add(":xpath1", SchemaType.XPath1);
            table.Add(":xpointer", SchemaType.XPointer);
            return table;
        }
    }
}
