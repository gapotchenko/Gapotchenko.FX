using Gapotchenko.FX.Data.Xml.Pointer.Dom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Gapotchenko.FX.Data.Xml.Test.Pointer
{
    [TestClass]
    public class XPointerDomTests
    {
        [TestMethod]
        public void XPointer_Dom_Xmlns_A1()
        {
            var part = new XPointerXmlnsPart();

            Assert.AreEqual(XName.Get("xmlns"), part.SchemeName);
        }

        [TestMethod]
        public void XPointer_Dom_Xmlns_A2()
        {
            var part = new XPointerXmlnsPart("t", XNamespace.Get("http://example.com/schemas/test"));

            string data = part.SchemeData;

            Assert.AreEqual("t=http://example.com/schemas/test", data);
        }

        [TestMethod]
        public void XPointer_Dom_Xmlns_A3()
        {
            var part = new XPointerXmlnsPart();

            part.SchemeData = "t=http://example.com/schemas/test";

            Assert.AreEqual(part.Prefix, "t");
            Assert.AreEqual("http://example.com/schemas/test", part.Namespace.NamespaceName);
        }
    }
}
