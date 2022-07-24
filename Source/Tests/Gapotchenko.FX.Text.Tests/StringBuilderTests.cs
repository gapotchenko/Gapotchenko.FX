using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Gapotchenko.FX.Text.Tests;

[TestClass]
public class StringBuilderTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringBuilder_AJ_E1()
    {
        StringBuilderExtensions.AppendJoin(null!, ", ", Enumerable.Empty<int>());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringBuilder_AJ_E2()
    {
        var sb = new StringBuilder();
        StringBuilderExtensions.AppendJoin(sb, ", ", (IEnumerable<int>)null!);
    }

    [TestMethod]
    public void StringBuilder_AJ_E3()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ", ", Enumerable.Empty<int>());
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_E4()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ", ", (IEnumerable<int>)new[] { 1 });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X1", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_E5()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ", ", (IEnumerable<int>)new[] { 1, 2, 3 });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X1, 2, 3", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_E6()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, null, (IEnumerable<int>)new[] { 1, 2, 3 });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X123", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_E7()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ",", (IEnumerable<string?>)new[] { "A", null, "C" });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("XA,,C", sb.ToString());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringBuilder_AJ_A1()
    {
        StringBuilderExtensions.AppendJoin(null!, ", ", new object[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringBuilder_AJ_A2()
    {
        var sb = new StringBuilder();
        StringBuilderExtensions.AppendJoin(sb, ", ", (object[])null!);
    }

    [TestMethod]
    public void StringBuilder_AJ_A3()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ", ", new object[0]);
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_A4()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ", ", new object[] { 1 });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X1", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_A5()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ", ", new object[] { 1, 2, 3 });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X1, 2, 3", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_A6()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, null, new object[] { 1, 2, 3 });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X123", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_A7()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ",", new object?[] { "A", null, "C" });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("XA,,C", sb.ToString());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringBuilder_AJ_SA1()
    {
        StringBuilderExtensions.AppendJoin(null!, ", ", new string[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringBuilder_AJ_SA2()
    {
        var sb = new StringBuilder();
        StringBuilderExtensions.AppendJoin(sb, ", ", (string[]?)null!);
    }

    [TestMethod]
    public void StringBuilder_AJ_SA3()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ", ", new string[0]);
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_SA4()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ", ", new string[] { "1" });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X1", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_SA5()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ", ", new string[] { "1", "2", "3" });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X1, 2, 3", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_SA6()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, null, new string[] { "1", "2", "3" });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X123", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_SA7()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ",", new string?[] { "A", null, "C" });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("XA,,C", sb.ToString());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringBuilder_AJ_CS_E1()
    {
        StringBuilderExtensions.AppendJoin(null!, ';', Enumerable.Empty<int>());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringBuilder_AJ_CS_E2()
    {
        var sb = new StringBuilder();
        StringBuilderExtensions.AppendJoin(sb, ';', (IEnumerable<int>?)null!);
    }

    [TestMethod]
    public void StringBuilder_AJ_CS_E3()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ';', Enumerable.Empty<int>());
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_CS_E4()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ';', (IEnumerable<int>)new[] { 1 });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X1", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_CS_E5()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ';', (IEnumerable<int>)new[] { 1, 2, 3 });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X1;2;3", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_CS_E6()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ';', (IEnumerable<string?>)new[] { "A", null, "C" });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("XA;;C", sb.ToString());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringBuilder_AJ_CS_E7()
    {
        StringBuilderExtensions.AppendJoin(null!, ';', new object[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringBuilder_AJ_CS_A1()
    {
        var sb = new StringBuilder();
        StringBuilderExtensions.AppendJoin(sb, ';', (object[]?)null!);
    }

    [TestMethod]
    public void StringBuilder_AJ_CS_A2()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ';', new object[0]);
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_CS_A3()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ';', new object[] { 1 });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X1", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_CS_A4()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ';', new object[] { 1, 2, 3 });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X1;2;3", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_CS_A5()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ';', new object?[] { "A", null, "C" });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("XA;;C", sb.ToString());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringBuilder_AJ_CS_SA1()
    {
        StringBuilderExtensions.AppendJoin(null!, ';', new string[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringBuilder_AJ_CS_SA2()
    {
        var sb = new StringBuilder();
        StringBuilderExtensions.AppendJoin(sb, ';', (string[]?)null!);
    }

    [TestMethod]
    public void StringBuilder_AJ_CS_SA3()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ';', new string[0]);
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_CS_SA4()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ';', new string[] { "1" });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X1", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_CS_SA5()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ';', new string[] { "1", "2", "3" });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("X1;2;3", sb.ToString());
    }

    [TestMethod]
    public void StringBuilder_AJ_CS_SA6()
    {
        var sb = new StringBuilder("X");
        var returnedSB = StringBuilderExtensions.AppendJoin(sb, ';', new string?[] { "A", null, "C" });
        Assert.AreSame(sb, returnedSB);
        Assert.AreEqual("XA;;C", sb.ToString());
    }
}
