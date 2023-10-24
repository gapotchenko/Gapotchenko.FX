// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Tests;

[TestClass]
public class UriExTests
{
    [TestMethod]
    public void UriEx_Combine_A1()
    {
        Assert.AreEqual("http://example.com/abc", UriEx.Combine("http://example.com/", "abc"));
        Assert.AreEqual("http://example.com/abc/", UriEx.Combine("http://example.com/", "abc/"));
        Assert.AreEqual("http://example.com/abc", UriEx.Combine("http://example.com/", "/abc"));
        Assert.AreEqual("http://example.com/abc/", UriEx.Combine("http://example.com/", "/abc/"));
    }

    [TestMethod]
    public void UriEx_Combine_A2()
    {
        Assert.AreEqual("http://example.com/test/abc", UriEx.Combine("http://example.com/test", "abc"));
        Assert.AreEqual("http://example.com/test/abc", UriEx.Combine("http://example.com/test/", "abc"));
        Assert.AreEqual("http://example.com/test/abc/", UriEx.Combine("http://example.com/test", "abc/"));
        Assert.AreEqual("http://example.com/test/abc/", UriEx.Combine("http://example.com/test/", "abc/"));
        Assert.AreEqual("http://example.com/abc", UriEx.Combine("http://example.com/test", "/abc"));
        Assert.AreEqual("http://example.com/abc", UriEx.Combine("http://example.com/test/", "/abc"));
        Assert.AreEqual("http://example.com/abc/", UriEx.Combine("http://example.com/test", "/abc/"));
        Assert.AreEqual("http://example.com/abc/", UriEx.Combine("http://example.com/test/", "/abc/"));
    }

    [TestMethod]
    public void UriEx_Combine_A3()
    {
        Assert.AreEqual("http://example.com/abc?p=100", UriEx.Combine("http://example.com/", "abc?p=100"));
        Assert.AreEqual("http://example.com/abc/?p=100", UriEx.Combine("http://example.com/", "abc/?p=100"));
        Assert.AreEqual("http://example.com/abc?p=100", UriEx.Combine("http://example.com/", "/abc?p=100"));
        Assert.AreEqual("http://example.com/abc/?p=100", UriEx.Combine("http://example.com/", "/abc/?p=100"));
    }

    [TestMethod]
    public void UriEx_Combine_A4()
    {
        Assert.AreEqual("http://example.com/test/abc?p=100", UriEx.Combine("http://example.com/test", "abc?p=100"));
        Assert.AreEqual("http://example.com/test/abc?p=100", UriEx.Combine("http://example.com/test/", "abc?p=100"));
        Assert.AreEqual("http://example.com/test/abc/?p=100", UriEx.Combine("http://example.com/test", "abc/?p=100"));
        Assert.AreEqual("http://example.com/test/abc/?p=100", UriEx.Combine("http://example.com/test/", "abc/?p=100"));
        Assert.AreEqual("http://example.com/abc?p=100", UriEx.Combine("http://example.com/test", "/abc?p=100"));
        Assert.AreEqual("http://example.com/abc?p=100", UriEx.Combine("http://example.com/test/", "/abc?p=100"));
        Assert.AreEqual("http://example.com/abc/?p=100", UriEx.Combine("http://example.com/test", "/abc/?p=100"));
        Assert.AreEqual("http://example.com/abc/?p=100", UriEx.Combine("http://example.com/test/", "/abc/?p=100"));
    }

    [TestMethod]
    public void UriEx_Combine_A5()
    {
        Assert.AreEqual("http://example.com/abc?q=50&p=100", UriEx.Combine("http://example.com/?q=50", "abc?p=100"));
        Assert.AreEqual("http://example.com/abc/?q=50&p=100", UriEx.Combine("http://example.com/?q=50", "abc/?p=100"));
        Assert.AreEqual("http://example.com/abc?q=50&p=100", UriEx.Combine("http://example.com/?q=50", "/abc?p=100"));
        Assert.AreEqual("http://example.com/abc/?q=50&p=100", UriEx.Combine("http://example.com/?q=50", "/abc/?p=100"));

        Assert.AreEqual("http://example.com/abc?p=100", UriEx.Combine("http://example.com/?q=50", "http://example.com/abc?p=100"));
        Assert.AreEqual("https://example.org/abc?p=100", UriEx.Combine("https://example.com/?q=50", "//example.org/abc?p=100"));
        Assert.AreEqual("https://example.org/abc?p=100#anchor", UriEx.Combine("https://example.com/?q=50", "//example.org/abc?p=100#anchor"));
    }

    [TestMethod]
    public void UriEx_Combine_A6()
    {
        Assert.AreEqual("http://example.com/test/abc?q=50&p=100", UriEx.Combine("http://example.com/test?q=50", "abc?p=100"));
        Assert.AreEqual("http://example.com/test/abc?q=50&p=100", UriEx.Combine("http://example.com/test/?q=50", "abc?p=100"));
        Assert.AreEqual("http://example.com/test/abc/?q=50&p=100", UriEx.Combine("http://example.com/test?q=50", "abc/?p=100"));
        Assert.AreEqual("http://example.com/test/abc/?q=50&p=100", UriEx.Combine("http://example.com/test/?q=50", "abc/?p=100"));
        Assert.AreEqual("http://example.com/abc?q=50&p=100", UriEx.Combine("http://example.com/test?q=50", "/abc?p=100"));
        Assert.AreEqual("http://example.com/abc?q=50&p=100", UriEx.Combine("http://example.com/test/?q=50", "/abc?p=100"));
        Assert.AreEqual("http://example.com/abc/?q=50&p=100", UriEx.Combine("http://example.com/test?q=50", "/abc/?p=100"));
        Assert.AreEqual("http://example.com/abc/?q=50&p=100", UriEx.Combine("http://example.com/test/?q=50", "/abc/?p=100"));

        Assert.AreEqual("http://example.com/abc?p=100", UriEx.Combine("http://example.com/test?q=50", "http://example.com/abc?p=100"));
        Assert.AreEqual("https://example.org/abc?p=100", UriEx.Combine("https://example.com/test?q=50", "//example.org/abc?p=100"));
        Assert.AreEqual("https://example.org/abc?p=100#anchor", UriEx.Combine("https://example.com/test/?q=50", "//example.org/abc?p=100#anchor"));
    }

    [TestMethod]
    public void UriEx_Combine_R1()
    {
        Assert.AreEqual("test/abc", UriEx.Combine("test", "abc"));
        Assert.AreEqual("test/abc", UriEx.Combine("test/", "abc"));
        Assert.AreEqual("/test/abc", UriEx.Combine(@"/test", "abc"));
        Assert.AreEqual("/test/abc", UriEx.Combine("/test/", "abc"));

        Assert.AreEqual("test/abc/", UriEx.Combine("test", "abc/"));
        Assert.AreEqual("test/abc/", UriEx.Combine("test/", "abc/"));
        Assert.AreEqual("/test/abc/", UriEx.Combine("/test", "abc/"));
        Assert.AreEqual("/test/abc/", UriEx.Combine("/test/", "abc/"));

        Assert.AreEqual("/abc", UriEx.Combine("test", "/abc"));
        Assert.AreEqual("/abc", UriEx.Combine("test/", "/abc"));
        Assert.AreEqual("/abc", UriEx.Combine("/test", "/abc"));
        Assert.AreEqual("/abc", UriEx.Combine("/test/", "/abc"));
        Assert.AreEqual("/abc/", UriEx.Combine("test", "/abc/"));
        Assert.AreEqual("/abc/", UriEx.Combine("test/", "/abc/"));
        Assert.AreEqual("/abc/", UriEx.Combine("/test", "/abc/"));
        Assert.AreEqual("/abc/", UriEx.Combine("/test/", "/abc/"));
    }

    [TestMethod]
    public void UriEx_Combine_R2()
    {
        Assert.AreEqual("test/abc?a=100&b=200", UriEx.Combine("test?a=100", "abc?b=200"));
    }

    [TestMethod]
    public void UriEx_Combine_I1()
    {
        Assert.AreEqual("abc", UriEx.Combine("abc", ""));
        Assert.AreEqual("abc", UriEx.Combine("", "abc"));
    }
}
