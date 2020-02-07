using System;
using System.Xml;

namespace Gapotchenko.FX.Data.Xml.XPointer
{
    /// <summary>
    /// Shorthand XPointer pointer.
    /// </summary>
    internal class ShorthandPointer : XPointer
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
            XmlNodeList result = doc.SelectNodes("id('" + _NCName + "')");
            if (result != null && result.Count > 0)
                return result;
            else
                throw new XPointerMatchException("XPointer doesn't identify any subresource");
        }
    }
}
