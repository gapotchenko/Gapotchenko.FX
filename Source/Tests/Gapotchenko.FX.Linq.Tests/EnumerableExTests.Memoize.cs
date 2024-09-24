using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace Gapotchenko.FX.Linq.Tests;

partial class EnumerableExTests
{
    static IEnumerable<int> _LazyGen(TextWriter tw)
    {
        tw.Write("(");
        try
        {
            tw.Write('A');
            yield return 1;

            tw.Write('B');
            yield return 2;

            tw.Write('C');
            yield return 3;

            tw.Write('D');
            yield return 4;

            tw.Write('E');
            yield return 5;

            tw.Write('.');
        }
        finally
        {
            tw.Write(")");
        }
    }

    [TestMethod]
    public void Linq_Memoize_A1()
    {
        var sw = new StringWriter();
        var seq = _LazyGen(sw).Memoize();
        foreach (var i in seq.Take(3))
            sw.Write(i.ToString(NumberFormatInfo.InvariantInfo));
        Assert.AreEqual("(A1B2C3", sw.ToString());
    }

    [TestMethod]
    public void Linq_Memoize_A2()
    {
        var sw = new StringWriter();
        var seq = _LazyGen(sw).Memoize();
        foreach (var i in seq.Take(3))
            sw.Write(i.ToString(NumberFormatInfo.InvariantInfo));
        foreach (var i in seq.Take(4))
            sw.Write(i.ToString(NumberFormatInfo.InvariantInfo));
        Assert.AreEqual("(A1B2C3123D4", sw.ToString());
    }

    [TestMethod]
    public void Linq_Memoize_A3()
    {
        var sw = new StringWriter();
        var seq = _LazyGen(sw).Memoize();
        foreach (var i in seq)
            sw.Write(i.ToString(NumberFormatInfo.InvariantInfo));
        foreach (var i in seq)
            sw.Write(i.ToString(NumberFormatInfo.InvariantInfo));
        Assert.AreEqual("(A1B2C3D4E5.)12345", sw.ToString());
    }

    static volatile int _V;

    [TestMethod]
    public void Linq_Memoize_A4()
    {
        var sw = new StringWriter();
        var seq = _LazyGen(sw).Memoize();
        var e = seq.GetEnumerator();

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(1, e.Current);

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(2, e.Current);

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(3, e.Current);

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(4, e.Current);

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(5, e.Current);

        Assert.IsFalse(e.MoveNext());

        bool exceptionThrown = false;
        try
        {
            _V = e.Current;
        }
        catch (InvalidOperationException)
        {
            exceptionThrown = true;
        }
        Assert.IsTrue(exceptionThrown);

        Assert.AreEqual("(ABCDE.)", sw.ToString());

        Assert.IsFalse(e.MoveNext());

        e.Reset();

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(1, e.Current);

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(2, e.Current);

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(3, e.Current);

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(4, e.Current);

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(5, e.Current);

        Assert.IsFalse(e.MoveNext());

        exceptionThrown = false;
        try
        {
            _V = e.Current;
        }
        catch (InvalidOperationException)
        {
            exceptionThrown = true;
        }
        Assert.IsTrue(exceptionThrown);

        Assert.IsFalse(e.MoveNext());

        Assert.AreEqual("(ABCDE.)", sw.ToString());
    }

    [TestMethod]
    public void Linq_Memoize_A5()
    {
        var sw = new StringWriter();
        var seq = _LazyGen(sw).Memoize();
        var e = seq.GetEnumerator();

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(1, e.Current);

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(2, e.Current);

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(3, e.Current);

        e.Dispose();

        Assert.IsFalse(e.MoveNext());

        bool exceptionThrown = false;
        try
        {
            _V = e.Current;
        }
        catch (InvalidOperationException)
        {
            exceptionThrown = true;
        }
        Assert.IsTrue(exceptionThrown);

        Assert.AreEqual("(ABC", sw.ToString());

        e.Reset();

        Assert.IsFalse(e.MoveNext());

        exceptionThrown = false;
        try
        {
            _V = e.Current;
        }
        catch (InvalidOperationException)
        {
            exceptionThrown = true;
        }
        Assert.IsTrue(exceptionThrown);

        Assert.AreEqual("(ABC", sw.ToString());
    }

    [TestMethod]
    public void Linq_Memoize_A6()
    {
        var sw = new StringWriter();
        var seq = _LazyGen(sw).Memoize();
        var e = seq.GetEnumerator();

        bool exceptionThrown = false;
        try
        {
            _V = e.Current;
        }
        catch (InvalidOperationException)
        {
            exceptionThrown = true;
        }
        Assert.IsTrue(exceptionThrown);

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(1, e.Current);

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(2, e.Current);

        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual(3, e.Current);

        Assert.AreEqual("(ABC", sw.ToString());

        e.Dispose();
        Assert.IsFalse(e.MoveNext());

        e.Dispose();
        Assert.IsFalse(e.MoveNext());

        exceptionThrown = false;
        try
        {
            _V = e.Current;
        }
        catch (InvalidOperationException)
        {
            exceptionThrown = true;
        }
        Assert.IsTrue(exceptionThrown);

        Assert.AreEqual("(ABC", sw.ToString());
    }

    [TestMethod]
    public void Linq_Memoize_A7()
    {
        var te = new TracedEnumerable<int>([1, 2, 3]);
        Assert.IsFalse(te.EnumeratorRetrieved);

        var me = te.Memoize();
        Assert.IsFalse(te.EnumeratorRetrieved);

        var mee = me.GetEnumerator();
        Assert.IsFalse(te.EnumeratorRetrieved);

        Assert.IsTrue(mee.MoveNext());
        Assert.IsTrue(te.EnumeratorRetrieved);
        Assert.AreEqual(1, mee.Current);
    }
}
