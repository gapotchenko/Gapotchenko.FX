using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Tests;

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

    [TestMethod]
    public void UriQueryBuilder_B1()
    {
        var uri = UriQueryBuilder.AppendParameter("https://example.com/?p=1#test", "p", "2");

        Assert.AreEqual("https://example.com/?p=1&p=2#test", uri);
    }

    [TestMethod]
    public void UriQueryBuilder_B2()
    {
        string uri = "https://example.com/#test";
        uri = UriQueryBuilder.CombineWithUri(uri, "p=1");

        Assert.AreEqual("https://example.com/?p=1#test", uri);
    }

    [TestMethod]
    public void UriQueryBuilder_B3()
    {
        string uri = "https://example.com/#test";
        uri = UriQueryBuilder.CombineWithUri(uri, "p=1");
        uri = UriQueryBuilder.CombineWithUri(uri, "p=2");

        Assert.AreEqual("https://example.com/?p=1&p=2#test", uri);
    }

    [TestMethod]
    public void UriQueryBuilder_B4()
    {
        string uri = "https://example.com/#test";
        uri = UriQueryBuilder.AppendParameter(uri, "p", "a b c");

        Assert.AreEqual("https://example.com/?p=a%20b%20c#test", uri);
    }

    [TestMethod]
    public void UriQueryBuilder_B5()
    {
        string uri = "https://example.com/#should-test-be-used?";
        uri = UriQueryBuilder.CombineWithUri(uri, "answer=yes");

        Assert.AreEqual("https://example.com/?answer=yes#should-test-be-used?", uri);
    }

    [TestMethod]
    public void UriQueryBuilder_B6()
    {
        string uri = "https://example.com/?action=ask#should-test-be-used?";
        uri = UriQueryBuilder.CombineWithUri(uri, "answer=yes");

        Assert.AreEqual("https://example.com/?action=ask&answer=yes#should-test-be-used?", uri);
    }

    [TestMethod]
    public void UriQueryBuilder_B7()
    {
        string uri = "https://example.com/?#should-test-be-used?";
        uri = UriQueryBuilder.CombineWithUri(uri, "answer=yes");

        Assert.AreEqual("https://example.com/?answer=yes#should-test-be-used?", uri);
    }

    [TestMethod]
    public void UriQueryBuilder_B8()
    {
        string uri = "#should-test-be-used?";
        uri = UriQueryBuilder.CombineWithUri(uri, "answer=yes");

        Assert.AreEqual("answer=yes#should-test-be-used?", uri);
    }

    [TestMethod]
    public void UriQueryBuilder_C1()
    {
        Assert.AreEqual("p=1", UriQueryBuilder.CombineWithUri("", "p=1"));
        Assert.AreEqual("p=1", UriQueryBuilder.CombineWithUri((string?)null, "p=1"));
    }

    [TestMethod]
    public void UriQueryBuilder_C2()
    {
        string uri = "https://example.com/";

        Assert.AreEqual(uri, UriQueryBuilder.CombineWithUri(uri, ""));
        Assert.AreEqual(uri, UriQueryBuilder.CombineWithUri(uri, null));
    }

    [TestMethod]
    public void UriQueryBuilder_C3()
    {
        Assert.AreEqual("", UriQueryBuilder.CombineWithUri("", null));
    }
}
