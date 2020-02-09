using System;
using System.Xml;

namespace Gapotchenko.FX.Data.Xml.Pointer
{
    /// <summary>
    /// Generic XPointer exception.
    /// </summary>
    public abstract class XPointerException : XmlException
    {
        public XPointerException(string message) : base(message) { }
        public XPointerException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class XPointerSyntaxException : XPointerException
    {
        public XPointerSyntaxException(string message) : base(message) { }
        public XPointerSyntaxException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class XPointerMatchException : XPointerException
    {
        public XPointerMatchException(string message) : base(message) { }
        public XPointerMatchException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
