using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace Gapotchenko.FX.Data.Xml.Include
{
    /// <summary>
    /// Custom <c>XmlReader</c>, handler for parse="text" case.	
    /// </summary>
    /// <remarks>
    /// Allows to read specified resource as a text node.
    /// </remarks>
    class XIncludeTextReader : XmlReader
    {
        private string _encoding;
        private ReadState _state;
        private string _value;
        private Uri _includeLocation;
        private string _accept, _acceptCharset, _acceptLanguage;
        private string _href;

        public XIncludeTextReader(Uri includeLocation, string encoding,
            string accept, string acceptCharset, string acceptLanguage)
        {
            _includeLocation = includeLocation;
            _href = includeLocation.AbsoluteUri;
            _encoding = encoding;
            _state = ReadState.Initial;
            _accept = accept;
            _acceptCharset = acceptCharset;
            _acceptLanguage = acceptLanguage;
        }

        public override int AttributeCount
        {
            get { return 0; }
        }

        public override string BaseURI
        {
            get { return _href; }
        }

        public override int Depth
        {
            get { return _state == ReadState.Interactive ? 1 : 0; }
        }

        public override bool EOF
        {
            get { return _state == ReadState.EndOfFile ? true : false; }
        }

        public override bool HasValue
        {
            get { return _state == ReadState.Interactive ? true : false; }
        }

        public override bool IsDefault
        {
            get { return false; }
        }

        public override bool IsEmptyElement
        {
            get { return false; }
        }

        public override string this[int index]
        {
            get { return String.Empty; }
        }

        public override string this[string qname]
        {
            get { return String.Empty; }
        }

        public override string this[string localname, string nsuri]
        {
            get { return String.Empty; }
        }

        public override string LocalName
        {
            get { return String.Empty; }
        }

        public override string Name
        {
            get { return String.Empty; }
        }

        public override string NamespaceURI
        {
            get { return String.Empty; }
        }

        public override XmlNameTable NameTable
        {
            get { return null; }
        }

        public override XmlNodeType NodeType
        {
            get { return _state == ReadState.Interactive ? XmlNodeType.Text : XmlNodeType.None; }
        }

        public override string Prefix
        {
            get { return String.Empty; }
        }

        public override char QuoteChar
        {
            get { return '"'; }
        }

        public override ReadState ReadState
        {
            get { return _state; }
        }

        public override string Value
        {
            get { return _state == ReadState.Interactive ? _value : String.Empty; }
        }

        public override string XmlLang
        {
            get { return String.Empty; }
        }

        public override XmlSpace XmlSpace
        {
            get { return XmlSpace.None; }
        }

        public override void Close()
        {
            _state = ReadState.Closed;
        }

        public override string GetAttribute(int index)
        {
            throw new ArgumentOutOfRangeException("index", index, "No attributes exposed");
        }

        public override string GetAttribute(string qname)
        {
            return null;
        }

        public override string GetAttribute(string localname, string nsuri)
        {
            return null;
        }

        public override string LookupNamespace(string prefix)
        {
            return null;
        }

        public override void MoveToAttribute(int index) { }

        public override bool MoveToAttribute(string qname)
        {
            return false;
        }

        public override bool MoveToAttribute(string localname, string nsuri)
        {
            return false;
        }

        public override bool MoveToElement()
        {
            return false;
        }

        public override bool MoveToFirstAttribute()
        {
            return false;
        }

        public override bool MoveToNextAttribute()
        {
            return false;
        }

        public override bool ReadAttributeValue()
        {
            return false;
        }

        public override string ReadInnerXml()
        {
            return _state == ReadState.Interactive ? _value : String.Empty;
        }

        public override string ReadOuterXml()
        {
            return _state == ReadState.Interactive ? _value : String.Empty;
        }

        public override string ReadString()
        {
            return _state == ReadState.Interactive ? _value : String.Empty;
        }

        public override void ResolveEntity() { }

        public override bool Read()
        {
            switch (_state)
            {
                case ReadState.Initial:
                    WebResponse wRes;
                    Stream stream = XIncludeReader.GetResource(_includeLocation.AbsoluteUri,
                        _includeLocation, _accept, _acceptCharset, _acceptLanguage, out wRes);
                    StreamReader reader;
                    /* According to the spec, encoding should be determined as follows:
						* external encoding information, if available, otherwise
						* if the media type of the resource is text/xml, application/xml, 
						  or matches the conventions text/*+xml or application/*+xml as 
						  described in XML Media Types [IETF RFC 3023], the encoding is 
						  recognized as specified in XML 1.0, otherwise
						* the value of the encoding attribute if one exists, otherwise  
						* UTF-8.
					*/
                    try
                    {
                        //TODO: try to get "content-encoding" from wRes.Headers collection?
                        //If mime type is xml-aware, get resource encoding as per XML 1.0
                        string contentType = wRes.ContentType.ToLower();
                        if (contentType == "text/xml" ||
                            contentType == "application/xml" ||
                            contentType.StartsWith("text/") && contentType.EndsWith("+xml") ||
                            contentType.StartsWith("application/") && contentType.EndsWith("+xml"))
                        {
                            //Yes, that's xml, let's read encoding from the xml declaration                    
                            reader = new StreamReader(stream, GetEncodingFromXMLDecl(_href));
                        }
                        else if (_encoding != null)
                        {
                            //Try to use user-specified encoding
                            Encoding enc;
                            try
                            {
                                enc = Encoding.GetEncoding(_encoding);
                            }
                            catch (Exception e)
                            {
                                throw new ResourceException("Not supported encoding '" + _encoding + "'", e);
                            }
                            reader = new StreamReader(stream, enc);
                        }
                        else
                            //Fallback to UTF-8
                            reader = new StreamReader(stream, Encoding.UTF8);
                        _value = XmlConvert.VerifyXmlChars(reader.ReadToEnd());
                    }
                    catch (ResourceException re)
                    {
                        throw re;
                    }
                    catch (OutOfMemoryException oome)
                    {
                        //Crazy include - memory is out - what about reading by chunks?
                        throw new ResourceException("Out of memory while reading resource '" + _href + "'", oome);
                    }
                    catch (IOException ioe)
                    {
                        throw new ResourceException("I/O error while fetching '" + _href + "'", ioe);
                    }
                    _state = ReadState.Interactive;
                    return true;
                case ReadState.Interactive:
                    //No more input
                    _state = ReadState.EndOfFile;
                    return false;
                default:
                    return false;
            }
        } // Read()

        /// <summary>
        /// Reads encoding from the XML declarartion.
        /// </summary>
        /// <param name="href">URI reference indicating the location 
        /// of the resource to inlclude.</param>		
        /// <returns>The document encoding as per XML declaration.</returns>
        /// <exception cref="ResourceException">Resource error.</exception>
        internal Encoding GetEncodingFromXMLDecl(string href)
        {
            var tmpReader = new XmlTextReader(href);
            tmpReader.WhitespaceHandling = WhitespaceHandling.None;
            while (tmpReader.Read())
            {
                switch (tmpReader.NodeType)
                {
                    case XmlNodeType.XmlDeclaration:
                        if (tmpReader.MoveToAttribute("encoding"))
                        {
                            string encValue = tmpReader.Value;
                            try
                            {
                                return Encoding.GetEncoding(encValue);
                            }
                            catch (Exception e)
                            {
                                throw new ResourceException("Not supported encoding '" + encValue + "'", e);
                            }
                        }
                        else
                            //No encoding in XML declaration - UTF-8 by default
                            return Encoding.UTF8;
                    default:
                        //No XML declaration - UTF-8 by default
                        return Encoding.UTF8;
                }
            }
            //Hmmm, empty file? Anyway
            return Encoding.UTF8;
        }
    }
}
