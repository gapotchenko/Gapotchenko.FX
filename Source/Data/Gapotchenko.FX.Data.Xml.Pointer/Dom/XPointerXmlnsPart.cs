using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

#nullable enable

namespace Gapotchenko.FX.Data.Xml.Pointer.Dom
{
    public sealed class XPointerXmlnsPart : XPointerPart
    {
        public XPointerXmlnsPart()
        {
            Namespace = XNamespace.None;
        }

        public XPointerXmlnsPart(string prefix, XNamespace ns)
        {
            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));
            if (ns == null)
                throw new ArgumentNullException(nameof(ns));

            Prefix = prefix;
            Namespace = ns;
        }

        string m_Prefix = string.Empty;

        public string Prefix
        {
            get => m_Prefix;
            set => m_Prefix = XmlConvert.VerifyNCName(value ?? throw new ArgumentNullException(nameof(value)));
        }

        public XNamespace Namespace { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static XName m_SchemeName = XName.Get("xmlns");

        public override XName SchemeName => m_SchemeName;

        public override string SchemeData
        {
            get => $"{Prefix}={Namespace}";
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                int j = value.IndexOf('=');
                if (j == -1)
                    throw new FormatException("XPointer xmlns expression does not contain '=' operator.");

                Prefix = value.Substring(0, j);
                Namespace = XNamespace.Get(value.Substring(j + 1));
            }
        }
    }
}
