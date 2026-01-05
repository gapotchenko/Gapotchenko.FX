using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS1718 // Comparison made to same variable

namespace Gapotchenko.FX.Tests;

[TestClass]
public class OptionalTests
{
    [TestMethod]
    public void Optional_AV1()
    {
        var optional = Optional.None<int>();
        Assert.IsFalse(optional.HasValue);
    }

    [TestMethod]
    public void Optional_AV2()
    {
        var optional = Optional.None<int>();
        Assert.ThrowsExactly<InvalidOperationException>(() => optional.Value);
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
        Assert.ThrowsExactly<InvalidOperationException>(() => optional.Value);
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
        var optional = Optional.None<string>();
        Assert.IsFalse(optional.HasValue);
    }

    [TestMethod]
    public void Optional_AR2()
    {
        var optional = Optional.None<string>();
        Assert.ThrowsExactly<InvalidOperationException>(() => optional.Value);
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
        Assert.ThrowsExactly<InvalidOperationException>(() => optional.Value);
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
        var optional = Optional.None<int>();
        Assert.IsFalse(optional.Equals(0));
        Assert.IsFalse(optional.Equals(null));
    }

    [TestMethod]
    public void Optional_BV2()
    {
        var optional = Optional.None<int>();
        Assert.IsFalse(optional.Equals((object)0));
    }

    [TestMethod]
    public void Optional_BV3()
    {
        var optional = Optional.None<int>();
        Assert.IsFalse(optional.Equals(Optional.Some(0)));

        Assert.IsTrue(optional.Equals(Optional.None<int>()));
        Assert.IsFalse(optional.Equals(Optional.None<long>()));

        Assert.IsTrue(optional.Equals((object)Optional.None<int>()));
        Assert.IsFalse(optional.Equals((object)Optional.None<long>()));
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
        var optional = Optional.None<string>();
        Assert.IsFalse(optional.Equals(null));
    }

    [TestMethod]
    public void Optional_BR2()
    {
        var optional = Optional.None<string>();
        Assert.IsFalse(optional.Equals((object?)null));
    }

    [TestMethod]
    public void Optional_BR3()
    {
        var optional = Optional.None<string?>();
        Assert.IsFalse(optional.Equals(Optional.Some<string?>(null)));

        Assert.IsTrue(optional.Equals(Optional.None<string?>()));
        Assert.IsFalse(optional.Equals(Optional.None<Version>()));

        Assert.IsTrue(optional.Equals((object)Optional.None<string>()));
        Assert.IsFalse(optional.Equals(Optional.None<Version>()));
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
        var optional = Optional.None<int>();
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
        var optional = Optional.None<string>();
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
        Assert.AreEqual(Optional.None<int>(), Optional.Discriminate(0));
        Assert.AreEqual(Optional.Some(1), Optional.Discriminate(1));
        Assert.AreEqual(Optional.Some(2), Optional.Discriminate(2));
    }

    [TestMethod]
    public void Optional_DA2()
    {
        Assert.AreEqual(Optional.Some(-1), Optional.Discriminate(-1, 1));
        Assert.AreEqual(Optional.Some(0), Optional.Discriminate(0, 1));
        Assert.AreEqual(Optional.None<int>(), Optional.Discriminate(1, 1));
        Assert.AreEqual(Optional.Some(2), Optional.Discriminate(2, 1));
    }

    [TestMethod]
    public void Optional_DA3()
    {
        Assert.AreEqual(Optional.None<string?>(), Optional.Discriminate<string?>(null, string.IsNullOrEmpty));
        Assert.AreEqual(Optional.None<string>(), Optional.Discriminate("", string.IsNullOrEmpty));
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
        var a = Optional.None<StringWriter>();
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
            optional.HasValue ? Optional.Some(optional.Value) : Optional.None<object>();
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

            var none = Optional.None<T?>();
            var some = Optional.Some(value);

            Assert.AreEqual(0, comparer.Compare(none, none));
            Assert.IsGreaterThan(0, comparer.Compare(some, none));
            Assert.IsLessThan(0, comparer.Compare(none, some));
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
        Assert.IsFalse(a > Optional.None<int>());
        Assert.IsFalse(Optional.None<int>() > b);
        Assert.IsFalse(Optional.None<int>() > Optional.None<int>());
        Assert.IsFalse(a > a);
        Assert.IsFalse(b > b);

        var comparer = Comparer<Optional<int>>.Default;
        Assert.IsGreaterThan(0, comparer.Compare(a, b));
    }

    [TestMethod]
    public void Optional_Comparability_LessThan()
    {
        Optional<int> a = -10;
        Optional<int> b = 10;

        Assert.IsTrue(a < b);
        Assert.IsFalse(b < a);
        Assert.IsFalse(a < Optional.None<int>());
        Assert.IsFalse(Optional.None<int>() < b);
        Assert.IsFalse(Optional.None<int>() < Optional.None<int>());
        Assert.IsFalse(a < a);
        Assert.IsFalse(b < b);

        var comparer = Comparer<Optional<int>>.Default;
        Assert.IsLessThan(0, comparer.Compare(a, b));
    }

    [TestMethod]
    public void Optional_Comparability_GreaterThanOrEqualTo()
    {
        Optional<int> a = 10;
        Optional<int> b = -10;

        Assert.IsTrue(a >= b);
        Assert.IsFalse(b >= a);
        Assert.IsFalse(a >= Optional.None<int>());
        Assert.IsFalse(Optional.None<int>() >= b);
        Assert.IsFalse(Optional.None<int>() >= Optional.None<int>());
        Assert.IsTrue(a >= a);
        Assert.IsTrue(b >= b);

        var comparer = Comparer<Optional<int>>.Default;
        Assert.IsGreaterThanOrEqualTo(0, comparer.Compare(a, b));
    }

    [TestMethod]
    public void Optional_Comparability_LessThanOrEqualTo()
    {
        Optional<int> a = -10;
        Optional<int> b = 10;

        Assert.IsTrue(a <= b);
        Assert.IsFalse(b <= a);
        Assert.IsFalse(a <= Optional.None<int>());
        Assert.IsFalse(Optional.None<int>() <= b);
        Assert.IsFalse(Optional.None<int>() <= Optional.None<int>());
        Assert.IsTrue(a <= a);
        Assert.IsTrue(b <= b);

        var comparer = Comparer<Optional<int>>.Default;
        Assert.IsLessThanOrEqualTo(0, comparer.Compare(a, b));
    }

    #endregion
}
