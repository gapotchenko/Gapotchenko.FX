using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Gapotchenko.FX.Data.Xml.Pointer._Legacy_
{
    sealed class SchemaBasedPointer : XPointer
    {
        private ArrayList _parts;

        public ArrayList Parts
        {
            get { return _parts; }
        }

        public SchemaBasedPointer(ArrayList parts)
        {
            _parts = parts;
        }

        public override XmlNodeList Evaluate(XmlDocument doc)
        {
            var nm = new XmlNamespaceManager(doc.NameTable);
            for (int i = 0; i < _parts.Count; i++)
            {
                PointerPart part = (PointerPart)_parts[i];
                var result = part.Evaluate(doc, nm);
                if (result.IsNullOrEmpty())
                    return result;
            }
            return XmlEmptyNodeList.Instance;
        }

        public override IEnumerable<XNode> Evaluate(XDocument document)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
