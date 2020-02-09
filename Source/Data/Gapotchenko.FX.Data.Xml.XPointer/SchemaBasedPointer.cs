using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Gapotchenko.FX.Data.Xml.XPointer
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
            XmlNodeList result;
            var nm = new XmlNamespaceManager(doc.NameTable);
            for (int i = 0; i < _parts.Count; i++)
            {
                PointerPart part = (PointerPart)_parts[i];
                result = part.Evaluate(doc, nm);
                if (result != null && result.Count > 0)
                    return result;
            }
            throw new XPointerMatchException("XPointer does not match any sub-resource.");
        }

        public override IEnumerable<XNode> Evaluate(XDocument document)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
