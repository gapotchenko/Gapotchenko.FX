using Gapotchenko.FX.Data.Xml.Pointer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;

namespace Gapotchenko.FX.Data.Xml.Test
{
    [TestClass]
    public class XPointerTests
    {
        [TestMethod]
        public void XPointer_Test1()
        {
            string xml = "<dogbreeds><dog breed='Rottweiler' id='Rottweiler'><history>The Rottweiler's ancestors were probably Roman drover dogs.....</history></dog></dogbreeds>";

            var xdoc = new XmlDocument();
            xdoc.LoadXml(xml);

            var xptr = XPointer.Parse("xpointer(id('Rottweiler'))");

            var list = xptr.Evaluate(xdoc);
        }

        [TestMethod]
        public void XPointer_Test2()
        {
            var xdoc = new XmlDocument();

            using (var stream = Snippet.GetStream("JORF.xml"))
                xdoc.Load(stream);

            var xptr = XPointer.Parse("auth1");

            var list = xptr.Evaluate(xdoc);
        }
    }
}
