using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Gapotchenko.FX.Data.Xml.Include
{
    class XIncludeKeywords
    {
        XmlNameTable nameTable;

        public XIncludeKeywords(XmlNameTable nt)
        {
            nameTable = nt;
            //Preload some keywords
            _XIncludeNamespace = nameTable.Add(s_XIncludeNamespace);
            _OldXIncludeNamespace = nameTable.Add(s_OldXIncludeNamespace);
            _Include = nameTable.Add(s_Include);
            _Href = nameTable.Add(s_Href);
            _Parse = nameTable.Add(s_Parse);
        }

        //
        // Keyword strings
        private const string s_XIncludeNamespace = "http://www.w3.org/2003/XInclude";
        private const string s_OldXIncludeNamespace = "http://www.w3.org/2001/XInclude";
        private const string s_Include = "include";
        private const string s_Href = "href";
        private const string s_Parse = "parse";
        private const string s_Xml = "xml";
        private const string s_Text = "text";
        private const string s_Xpointer = "xpointer";
        private const string s_Accept = "accept";
        private const string s_AcceptCharset = "accept-charset";
        private const string s_AcceptLanguage = "accept-language";
        private const string s_Encoding = "encoding";
        private const string s_Fallback = "fallback";
        private const string s_XmlNamespace = "http://www.w3.org/XML/1998/namespace";
        private const string s_Base = "base";
        private const string s_XmlBase = "xml:base";

        //
        // Properties
        private string _XIncludeNamespace;
        private string _OldXIncludeNamespace;
        private string _Include;
        private string _Href;
        private string _Parse;
        private string _Xml;
        private string _Text;
        private string _Xpointer;
        private string _Accept;
        private string _AcceptCharset;
        private string _AcceptLanguage;
        private string _Encoding;
        private string _Fallback;
        private string _XmlNamespace;
        private string _Base;
        private string _XmlBase;

        // http://www.w3.org/2003/XInclude
        internal string XIncludeNamespace
        {
            get { return _XIncludeNamespace; }
        }

        // http://www.w3.org/2001/XInclude
        internal string OldXIncludeNamespace
        {
            get { return _OldXIncludeNamespace; }
        }

        // include
        internal string Include
        {
            get { return _Include; }
        }

        // href
        internal string Href
        {
            get { return _Href; }
        }

        // parse
        internal string Parse
        {
            get { return _Parse; }
        }

        // xml
        internal string Xml
        {
            get
            {
                if (_Xml == null)
                    _Xml = nameTable.Add(s_Xml);
                return _Xml;
            }
        }

        // text
        internal string Text
        {
            get
            {
                if (_Text == null)
                    _Text = nameTable.Add(s_Text);
                return _Text;
            }
        }

        // xpointer
        internal string Xpointer
        {
            get
            {
                if (_Xpointer == null)
                    _Xpointer = nameTable.Add(s_Xpointer);
                return _Xpointer;
            }
        }

        // accept
        internal string Accept
        {
            get
            {
                if (_Accept == null)
                    _Accept = nameTable.Add(s_Accept);
                return _Accept;
            }
        }

        // accept-charset
        internal string AcceptCharset
        {
            get
            {
                if (_AcceptCharset == null)
                    _AcceptCharset = nameTable.Add(s_AcceptCharset);
                return _AcceptCharset;
            }
        }

        // accept-language
        internal string AcceptLanguage
        {
            get
            {
                if (_AcceptLanguage == null)
                    _AcceptLanguage = nameTable.Add(s_AcceptLanguage);
                return _AcceptLanguage;
            }
        }

        // encoding
        internal string Encoding
        {
            get
            {
                if (_Encoding == null)
                    _Encoding = nameTable.Add(s_Encoding);
                return _Encoding;
            }
        }

        // fallback
        internal string Fallback
        {
            get
            {
                if (_Fallback == null)
                    _Fallback = nameTable.Add(s_Fallback);
                return _Fallback;
            }
        }

        // Xml namespace
        internal string XmlNamespace
        {
            get
            {
                if (_XmlNamespace == null)
                    _XmlNamespace = nameTable.Add(s_XmlNamespace);
                return _XmlNamespace;
            }
        }

        // Base
        internal string Base
        {
            get
            {
                if (_Base == null)
                    _Base = nameTable.Add(s_Base);
                return _Base;
            }
        }

        // xml:base
        internal string XmlBase
        {
            get
            {
                if (_XmlBase == null)
                    _XmlBase = nameTable.Add(s_XmlBase);
                return _XmlBase;
            }
        }

        // Comparison
        internal static bool Equals(string keyword1, string keyword2)
        {
            return (object)keyword1 == (object)keyword2;
        }
    }
}
