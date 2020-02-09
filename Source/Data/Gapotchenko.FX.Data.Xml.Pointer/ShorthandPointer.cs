using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Gapotchenko.FX.Data.Xml.Pointer
{
    /// <summary>
    /// Shorthand XPointer pointer.
    /// </summary>
    sealed class ShorthandPointer : XPointer
    {
        string _NCName;

        public string NCName
        {
            get { return _NCName; }
        }

        public ShorthandPointer(string n)
        {
            _NCName = n;
        }

        public override XmlNodeList Evaluate(XmlDocument doc)
        {
            XmlNodeList result = doc.SelectNodes($"//*[@id=\"{_NCName}\"]");
            if (result != null && result.Count > 0)
                return result;
            else
                throw new XPointerMatchException("XPointer doesn't identify any subresource");
        }

        public override IEnumerable<XNode> Evaluate(XDocument document)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
