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
    public void UriEx_Combine_B1()
    {
        Assert.AreEqual("test/abc", UriEx.Combine("test", "abc"));
        Assert.AreEqual("test/abc/", UriEx.Combine("test", "abc/"));
        Assert.AreEqual("/abc", UriEx.Combine("test", "/abc"));
        Assert.AreEqual("/abc/", UriEx.Combine("test", "/abc/"));
    }
}
