using System;
using System.Xml;
using System.IO;
using System.Collections;

namespace Gapotchenko.FX.Data.Xml.Pointer._Legacy_
{
    /// <summary>
    /// XPointer-aware XML reader.
    /// </summary>
    public class XPointerReader : XmlReader
    {
        //Underlying reader
        XmlReader _reader, _extReader;
        Uri _uri;
        string _xpointer;
        Stream _stream;
        XmlNameTable _nt;
        IEnumerator _pointedNodes;

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given uri, stream, nametable and xpointer.
        /// </summary>	    
        public XPointerReader(Uri uri, Stream stream, XmlNameTable nt, string xpointer)
        {
            _uri = uri;
            _stream = stream;
            _nt = nt;
            _xpointer = xpointer;
        }

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given XmlReader and xpointer.
        /// </summary>	    
        public XPointerReader(XmlReader reader, XmlNameTable nt, string xpointer)
        {
            _extReader = reader;
            _xpointer = xpointer;
            _nt = nt;
        }

        public override int AttributeCount
        {
            get { return _reader.AttributeCount; }
        }

        public override string BaseURI
        {
            get { return _reader.BaseURI; }
        }

        public override bool HasValue
        {
            get { return _reader.HasValue; }
        }

        public override bool IsDefault
        {
            get { return _reader.IsDefault; }
        }

        public override string Name
        {
            get { return _reader.Name; }
        }

        public override string LocalName
        {
            get { return _reader.LocalName; }
        }

        public override string NamespaceURI
        {
            get { return _reader.NamespaceURI; }
        }

        public override XmlNameTable NameTable
        {
            get { return _reader.NameTable; }
        }

        public override XmlNodeType NodeType
        {
            get { return _reader.NodeType; }
        }

        public override string Prefix
        {
            get { return _reader.Prefix; }
        }

        public override char QuoteChar
        {
            get { return _reader.QuoteChar; }
        }

        public override void Close()
        {
            if (_reader != null)
                _reader.Close();
        }

        public override int Depth
        {
            get { return _reader.Depth; }
        }

        public override bool EOF
        {
            get { return _reader.EOF; }
        }

        public override string GetAttribute(int i)
        {
            return _reader.GetAttribute(i);
        }

        public override string GetAttribute(string name)
        {
            return _reader.GetAttribute(name);
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            return _reader.GetAttribute(name, namespaceURI);
        }

        public override bool IsEmptyElement
        {
            get { return _reader.IsEmptyElement; }
        }

        public override String LookupNamespace(String prefix)
        {
            return _reader.LookupNamespace(prefix);
        }

        public override void MoveToAttribute(int i)
        {
            _reader.MoveToAttribute(i);
        }

        public override bool MoveToAttribute(string name)
        {
            return _reader.MoveToAttribute(name);
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return _reader.MoveToAttribute(name, ns);
        }

        public override bool MoveToElement()
        {
            return _reader.MoveToElement();
        }

        public override bool MoveToFirstAttribute()
        {
            return _reader.MoveToFirstAttribute();
        }

        public override bool MoveToNextAttribute()
        {
            return _reader.MoveToNextAttribute();
        }

        public override bool ReadAttributeValue()
        {
            return _reader.ReadAttributeValue();
        }

        public override ReadState ReadState
        {
            get { return _reader.ReadState; }
        }

        public override String this[int i]
        {
            get { return _reader[i]; }
        }

        public override string this[string name]
        {
            get { return _reader[name]; }
        }

        public override string this[string name, string namespaceURI]
        {
            get { return _reader[name, namespaceURI]; }
        }

        public override void ResolveEntity()
        {
            _reader.ResolveEntity();
        }

        public override string XmlLang
        {
            get { return _reader.XmlLang; }
        }

        public override XmlSpace XmlSpace
        {
            get { return _reader.XmlSpace; }
        }

        public override string Value
        {
            get { return _reader.Value; }
        }

        public override string ReadInnerXml()
        {
            return _reader.ReadInnerXml();
        }

        public override string ReadOuterXml()
        {
            return _reader.ReadOuterXml();
        }

        public override string ReadString()
        {
            return _reader.ReadString();
        }

        public override bool Read()
        {
            if (_reader == null)
            {
                var doc = new XmlDocument(_nt);
                doc.PreserveWhitespace = true;
                if (_extReader == null)
                    doc.Load(new XmlTextReader(_uri.AbsoluteUri, new StreamReader(_stream)));
                else
                    doc.Load(_extReader);
                var pointer = XPointer.Parse(_xpointer);
                var list = pointer.Evaluate(doc);
                _pointedNodes = list.GetEnumerator();
                _pointedNodes.MoveNext();
                _reader = new XmlNodeReader(_pointedNodes.Current as XmlNode);
            }

            bool baseRead = _reader.Read();
            if (baseRead)
                return true;

            if (_pointedNodes != null)
            {
                if (_pointedNodes.MoveNext())
                {
                    _reader = new XmlNodeReader(_pointedNodes.Current as XmlNode);
                    return _reader.Read();
                }
            }

            return false;
        }
    }
}
