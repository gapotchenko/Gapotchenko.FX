using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS1718 // Comparison made to same variable

namespace Gapotchenko.FX.Tests;

[TestClass]
public class OptionalTests
{
    [TestMethod]
    public void Optional_AV1()
    {
        var optional = Optional<int>.None;
        Assert.IsFalse(optional.HasValue);
    }

    [TestMethod]
    public void Optional_AV2()
    {
        var optional = Optional<int>.None;
        Assert.ThrowsException<InvalidOperationException>(() => optional.Value);
    }

    [TestMethod]
    public void Optional_AV3()
    {
        var optional = Optional.Some(10);
        Assert.IsTrue(optional.HasValue);
        Assert.AreEqual(10, optional.Value);
    }

    [TestMethod]
    public void Optional_AV4()
    {
        var optional = new Optional<int>();
        Assert.IsFalse(optional.HasValue);
    }

    [TestMethod]
    public void Optional_AV5()
    {
        var optional = new Optional<int>();
        Assert.ThrowsException<InvalidOperationException>(() => optional.Value);
    }

    [TestMethod]
    public void Optional_AV6()
    {
        var optional = new Optional<int>(100);
        Assert.IsTrue(optional.HasValue);
        Assert.AreEqual(100, optional.Value);
    }

    [TestMethod]
    public void Optional_AR1()
    {
        var optional = Optional<string>.None;
        Assert.IsFalse(optional.HasValue);
    }

    [TestMethod]
    public void Optional_AR2()
    {
        var optional = Optional<string>.None;
        Assert.ThrowsException<InvalidOperationException>(() => optional.Value);
    }

    [TestMethod]
    public void Optional_AR3()
    {
        var optional = Optional.Some("ABC");
        Assert.IsTrue(optional.HasValue);
        Assert.AreEqual("ABC", optional.Value);
    }

    [TestMethod]
    public void Optional_AR4()
    {
        var optional = new Optional<string>();
        Assert.IsFalse(optional.HasValue);
    }

    [TestMethod]
    public void Optional_AR5()
    {
        var optional = new Optional<string>();
        Assert.ThrowsException<InvalidOperationException>(() => optional.Value);
    }

    [TestMethod]
    public void Optional_AR6()
    {
        var optional = new Optional<string>("ABCDEF");
        Assert.IsTrue(optional.HasValue);
        Assert.AreEqual("ABCDEF", optional.Value);
    }

    [TestMethod]
    public void Optional_AR7()
    {
        var optional = new Optional<string?>(null);
        Assert.IsTrue(optional.HasValue);
        Assert.IsNull(optional.Value);
    }

    [TestMethod]
    public void Optional_BV1()
    {
        var optional = Optional<int>.None;
        Assert.IsFalse(optional.Equals(0));
        Assert.IsFalse(optional.Equals(null));
    }

    [TestMethod]
    public void Optional_BV2()
    {
        var optional = Optional<int>.None;
        Assert.IsFalse(optional.Equals((object)0));
    }

    [TestMethod]
    public void Optional_BV3()
    {
        var optional = Optional<int>.None;
        Assert.IsFalse(optional.Equals(Optional.Some(0)));

        Assert.IsTrue(optional.Equals(Optional<int>.None));
        Assert.IsFalse(optional.Equals(Optional<long>.None));

        Assert.IsTrue(optional.Equals((object)Optional<int>.None));
        Assert.IsFalse(optional.Equals((object)Optional<long>.None));
    }

    [TestMethod]
    public void Optional_BV4()
    {
        var optional = Optional.Some(10);
        Assert.IsTrue(optional.Equals(10));
        Assert.IsFalse(optional.Equals(11));
    }

    [TestMethod]
    public void Optional_BV5()
    {
        var optional = Optional.Some(10);
        Assert.IsTrue(optional.Equals((object)10));
        Assert.IsFalse(optional.Equals((object)11));
    }

    [TestMethod]
    public void Optional_BV6()
    {
        var optional = Optional.Some(10);
        Assert.IsTrue(optional.Equals(Optional.Some(10)));
        Assert.IsFalse(optional.Equals(Optional.Some(11)));
    }

    [TestMethod]
    public void Optional_BR1()
    {
        var optional = Optional<string>.None;
        Assert.IsFalse(optional.Equals(null));
    }

    [TestMethod]
    public void Optional_BR2()
    {
        var optional = Optional<string>.None;
        Assert.IsFalse(optional.Equals((object?)null));
    }

    [TestMethod]
    public void Optional_BR3()
    {
        var optional = Optional<string?>.None;
        Assert.IsFalse(optional.Equals(Optional.Some<string?>(null)));

        Assert.IsTrue(optional.Equals(Optional<string?>.None));
        Assert.IsFalse(optional.Equals(Optional<Version>.None));

        Assert.IsTrue(optional.Equals((object)Optional<string>.None));
        Assert.IsFalse(optional.Equals(Optional<Version>.None));
    }

    [TestMethod]
    public void Optional_BR4()
    {
        var optional = Optional.Some("10");
        Assert.IsTrue(optional.Equals("10"));
        Assert.IsFalse(optional.Equals("11"));
    }

    [TestMethod]
    public void Optional_BR5()
    {
        var optional = Optional.Some("10");
        Assert.IsTrue(optional.Equals((object)"10"));
        Assert.IsFalse(optional.Equals((object)"11"));
    }

    [TestMethod]
    public void Optional_BR6()
    {
        var optional = Optional.Some("10");
        Assert.IsTrue(optional.Equals(Optional.Some("10")));
        Assert.IsFalse(optional.Equals(Optional.Some("11")));
    }

    [TestMethod]
    public void Optional_CV1()
    {
        var optional = Optional<int>.None;
        Assert.AreEqual(0, optional.GetHashCode());
    }

    [TestMethod]
    public void Optional_CV2()
    {
        var optional = Optional.Some(10);
        Assert.AreEqual(optional.Value.GetHashCode(), optional.GetHashCode());
    }

    [TestMethod]
    public void Optional_CR1()
    {
        var optional = Optional<string>.None;
        Assert.AreEqual(0, optional.GetHashCode());
    }

    [TestMethod]
    public void Optional_CR2()
    {
        var optional = Optional.Some("10");
        Assert.AreEqual(optional.Value.GetHashCode(), optional.GetHashCode());
    }

    [TestMethod]
    public void Optional_CR3()
    {
        var optional = Optional.Some<string?>(null);
        Assert.AreEqual(0, optional.GetHashCode());
    }

    [TestMethod]
    public void Optional_DA1()
    {
        Assert.AreEqual(Optional.Some(-1), Optional.Discriminate(-1));
        Assert.AreEqual(Optional<int>.None, Optional.Discriminate(0));
        Assert.AreEqual(Optional.Some(1), Optional.Discriminate(1));
        Assert.AreEqual(Optional.Some(2), Optional.Discriminate(2));
    }

    [TestMethod]
    public void Optional_DA2()
    {
        Assert.AreEqual(Optional.Some(-1), Optional.Discriminate(-1, 1));
        Assert.AreEqual(Optional.Some(0), Optional.Discriminate(0, 1));
        Assert.AreEqual(Optional<int>.None, Optional.Discriminate(1, 1));
        Assert.AreEqual(Optional.Some(2), Optional.Discriminate(2, 1));
    }

    [TestMethod]
    public void Optional_DA3()
    {
        Assert.AreEqual(Optional<string?>.None, Optional.Discriminate<string?>(null, string.IsNullOrEmpty));
        Assert.AreEqual(Optional<string>.None, Optional.Discriminate("", string.IsNullOrEmpty));
        Assert.AreEqual("A", Optional.Discriminate("A", string.IsNullOrEmpty));
    }

    [TestMethod]
    public void Optional_E1_S()
    {
        Optional<string> a = "test";
        Optional<object> b = a;
        Assert.IsInstanceOfType<string>(b.Value);
        Assert.IsTrue(a.Equals(b));
        Assert.IsTrue(b == a);

        Optional<int> c = 10;
        b = c;
        Assert.IsInstanceOfType<int>(b.Value);
        Assert.IsTrue(c.Equals(b));
        Assert.IsTrue(b == c);
    }

    [TestMethod]
    public void Optional_E1_N()
    {
        Optional<string> a = default;
        Optional<object> b = a;
        Assert.IsTrue(a.Equals(b));
        Assert.IsTrue(b == a);

        Optional<int> c = default;
        b = c;
        Assert.IsTrue(c.Equals(b));
        Assert.IsTrue(b == c);
    }

    [TestMethod]
    public void Optional_E2_S()
    {
        var a = Optional.Some(new StringWriter());
        var b = a.Cast<IDisposable>();
        Assert.IsInstanceOfType<StringWriter>(b.Value);
        Assert.IsTrue(a.Equals(b));
        Assert.IsTrue(a == b);

        Optional<int> c = 10;
        var d = c.Cast<IComparable>();
        Assert.IsInstanceOfType<int>(d.Value);
        Assert.IsTrue(c.Equals(d));
        Assert.IsTrue(c == d);
    }

    [TestMethod]
    public void Optional_E2_N()
    {
        var a = Optional<StringWriter>.None;
        var b = a.Cast<IDisposable>();
        Assert.IsTrue(a.Equals(b));
        Assert.IsTrue(a == b);

        Optional<int> c = default;
        var d = c.Cast<IComparable>();
        Assert.IsTrue(c.Equals(d));
        Assert.IsTrue(c == d);
    }

    [TestMethod]
    public void Optional_E3()
    {
        const string s = "abc";

        var optional = F(s);
        Assert.IsInstanceOfType<string>(optional.Value);
        Assert.AreEqual(s, optional.Value);

        static Optional<object> F(Optional<string> optional) =>
            optional.HasValue ? Optional.Some(optional.Value) : Optional<object>.None;
    }

    [TestMethod]
    public void Optional_E4()
    {
        const string s = "abc";

        Optional<Optional<string>> optional;
        optional = new(s);
        Assert.AreEqual(s, optional.Value.Value);
    }

    #region Comparability

    [TestMethod]
    public void Optional_Comparability_CompareWithNone()
    {
        Test<int>();
        Test(-10);
        Test(10);

        Test<string>();
        Test(string.Empty);

        static void Test<T>(T? value = default)
        {
            var comparer = Comparer<Optional<T?>>.Default;

            var none = Optional<T?>.None;
            var some = Optional.Some(value);

            Assert.AreEqual(0, comparer.Compare(none, none));
            Assert.IsTrue(comparer.Compare(some, none) > 0);
            Assert.IsTrue(comparer.Compare(none, some) < 0);
            Assert.AreEqual(0, comparer.Compare(some, some));
        }
    }

    [TestMethod]
    public void Optional_Comparability_GreaterThan()
    {
        Optional<int> a = 10;
        Optional<int> b = -10;

        Assert.IsTrue(a > b);
        Assert.IsFalse(b > a);
        Assert.IsFalse(a > Optional<int>.None);
        Assert.IsFalse(Optional<int>.None > b);
        Assert.IsFalse(Optional<int>.None > Optional<int>.None);
        Assert.IsFalse(a > a);
        Assert.IsFalse(b > b);

        var comparer = Comparer<Optional<int>>.Default;
        Assert.IsTrue(comparer.Compare(a, b) > 0);
    }

    [TestMethod]
    public void Optional_Comparability_LessThan()
    {
        Optional<int> a = -10;
        Optional<int> b = 10;

        Assert.IsTrue(a < b);
        Assert.IsFalse(b < a);
        Assert.IsFalse(a < Optional<int>.None);
        Assert.IsFalse(Optional<int>.None < b);
        Assert.IsFalse(Optional<int>.None < Optional<int>.None);
        Assert.IsFalse(a < a);
        Assert.IsFalse(b < b);

        var comparer = Comparer<Optional<int>>.Default;
        Assert.IsTrue(comparer.Compare(a, b) < 0);
    }

    [TestMethod]
    public void Optional_Comparability_GreaterThanOrEqualTo()
    {
        Optional<int> a = 10;
        Optional<int> b = -10;

        Assert.IsTrue(a >= b);
        Assert.IsFalse(b >= a);
        Assert.IsFalse(a >= Optional<int>.None);
        Assert.IsFalse(Optional<int>.None >= b);
        Assert.IsFalse(Optional<int>.None >= Optional<int>.None);
        Assert.IsTrue(a >= a);
        Assert.IsTrue(b >= b);

        var comparer = Comparer<Optional<int>>.Default;
        Assert.IsTrue(comparer.Compare(a, b) >= 0);
    }

    [TestMethod]
    public void Optional_Comparability_LessThanOrEqualTo()
    {
        Optional<int> a = -10;
        Optional<int> b = 10;

        Assert.IsTrue(a <= b);
        Assert.IsFalse(b <= a);
        Assert.IsFalse(a <= Optional<int>.None);
        Assert.IsFalse(Optional<int>.None <= b);
        Assert.IsFalse(Optional<int>.None <= Optional<int>.None);
        Assert.IsTrue(a <= a);
        Assert.IsTrue(b <= b);

        var comparer = Comparer<Optional<int>>.Default;
        Assert.IsTrue(comparer.Compare(a, b) <= 0);
    }

    #endregion
}
