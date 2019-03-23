using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Test
{
    [TestClass]
    public class UriQueryBuilderTests
    {
        [TestMethod]
        public void UriQueryBuilder_A1()
        {
            var uqb = new UriQueryBuilder();

            Assert.AreEqual("", uqb.ToString());
            Assert.AreEqual(0, uqb.Length);
        }

        [TestMethod]
        public void UriQueryBuilder_A2()
        {
            var uqb = new UriQueryBuilder();
            uqb.AppendParameter("a", "1");

            Assert.AreEqual("a=1", uqb.ToString());
            Assert.AreEqual(3, uqb.Length);
        }

        [TestMethod]
        public void UriQueryBuilder_A3()
        {
            var uqb = new UriQueryBuilder();
            uqb
                .AppendParameter("a", "1")
                .AppendParameter("b", "2");

            Assert.AreEqual("a=1&b=2", uqb.ToString());
            Assert.AreEqual(7, uqb.Length);
        }

        [TestMethod]
        public void UriQueryBuilder_A4()
        {
            var uqb = new UriQueryBuilder("a=1");
            uqb.AppendParameter("b", "2");

            Assert.AreEqual("a=1&b=2", uqb.ToString());
        }

        [TestMethod]
        public void UriQueryBuilder_A5()
        {
            var uqb = new UriQueryBuilder(null);

            Assert.AreEqual("", uqb.ToString());
        }

        [TestMethod]
        public void UriQueryBuilder_A6()
        {
            var uqb = new UriQueryBuilder("?");

            Assert.AreEqual("", uqb.ToString());
        }

        [TestMethod]
        public void UriQueryBuilder_A7()
        {
            var uqb = new UriQueryBuilder("?a=1");
            uqb.AppendParameter("b", "2");

            Assert.AreEqual("a=1&b=2", uqb.ToString());
        }

        [TestMethod]
        public void UriQueryBuilder_A8()
        {
            var ub = new UriBuilder("https://example.com/?key=abc");

            var uqb = new UriQueryBuilder(ub.Query)
                .AppendParameter("mode", "1")
                .AppendParameter("complexity", "easy");

            ub.Query = uqb.ToString();

            Assert.AreEqual("https://example.com/?key=abc&mode=1&complexity=easy", ub.Uri.ToString());
        }

        [TestMethod]
        public void UriQueryBuilder_A9()
        {
            var uri = UriQueryBuilder.AppendParameter("https://example.com/?key=abc", "say", "hello");

            Assert.AreEqual("https://example.com/?key=abc&say=hello", uri);
        }
    }
}
