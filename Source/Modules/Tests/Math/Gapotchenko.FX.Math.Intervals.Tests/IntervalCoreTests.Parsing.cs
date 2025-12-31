// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Globalization;
using System.Reflection;

namespace Gapotchenko.FX.Math.Intervals.Tests;

partial class IntervalCoreTests
{
    protected abstract IInterval<T> Parse<T>(ReadOnlySpan<char> input, IFormatProvider? provider = null)
        where T : IComparable<T>, IEquatable<T>;

    protected abstract IInterval<T>? TryParse<T>(ReadOnlySpan<char> input, IFormatProvider? provider = null)
        where T : IComparable<T>, IEquatable<T>;

    [TestMethod]
    [DynamicData(nameof(Interval_Core_Parse_ValidInputs))]
    [DynamicData(nameof(Interval_Core_Parse_ValidAltInputs))]
    public void Interval_Core_Parse_ValidInput(string input, IInterval expectedInterval, IFormatProvider? provider)
    {
        var type = expectedInterval.GetType().GetGenericArguments()[0];

        var actualInterval = CallTestTryParse(type, input, provider);
        if (!expectedInterval.IntervalEquals(actualInterval))
            Assert.Fail($"Expected interval is {expectedInterval}, but the actual is {actualInterval}.");
    }

    [TestMethod]
    [DynamicData(nameof(Interval_Core_Parse_InvalidInputs))]
    public void Interval_Core_Parse_InvalidInput(string input, Type type, IFormatProvider? provider)
    {
        var actualInterval = CallTestTryParse(type, input, provider);
        Assert.IsNull(actualInterval, $"'{input}' string should not represent a valid interval.");
    }

    IInterval? CallTestTryParse(Type type, string input, IFormatProvider? provider)
    {
        var methodInfo = typeof(IntervalCoreTests).GetMethod(nameof(TestTryParse), BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(methodInfo);

        var specializedMethodInfo = methodInfo.MakeGenericMethod(type);
        return (IInterval?)specializedMethodInfo.Invoke(this, [input, provider]);
    }

    IInterval<T>? TestTryParse<T>(string input, IFormatProvider? provider)
        where T : IComparable<T>, IEquatable<T>
    {
        var interval = TryParse<T>(input, provider);

        if (interval is null)
        {
            Assert.ThrowsExactly<FormatException>(() => Parse<T>(input, provider));
        }
        else
        {
            var other = Parse<T>(input, provider);
            Assert.AreEqual(interval, other, IntervalEqualityComparer.Default<T>());
        }

        return interval;
    }

    static IEnumerable<(string, IInterval, IFormatProvider?)> Interval_Core_Parse_ValidInputs
    {
        get
        {
            // Empty intervals
            yield return ("∅", Interval.Empty<int>(), null);
            yield return ("{}", Interval.Empty<int>(), null);

            // Degenerate intervals
            yield return ("{42}", Interval.Degenerate(42), null);
            yield return ("[42,42]", Interval.Degenerate(42), null);

            // Infinite intervals
            yield return ("(-∞,∞)", Interval.Infinite<int>(), null);
            yield return ("(-∞,+∞)", Interval.Infinite<int>(), null);
            yield return ("(-inf,inf)", Interval.Infinite<int>(), null);
            yield return ("(-inf,+inf)", Interval.Infinite<int>(), null);

            yield return ("[A,Z]", Interval.Inclusive('A', 'Z'), null);
            yield return ("(A,Z)", Interval.Exclusive('A', 'Z'), null);
            yield return ("[A,Z)", Interval.InclusiveExclusive('A', 'Z'), null);
            yield return ("(A,Z]", Interval.ExclusiveInclusive('A', 'Z'), null);

            yield return ("[42.5,43.7]", Interval.Inclusive(42.5m, 43.7m), NumberFormatInfo.InvariantInfo);
        }
    }

    static IEnumerable<(string, IInterval, IFormatProvider?)> Interval_Core_Parse_ValidAltInputs
    {
        get
        {
            yield return ("{ }", Interval.Empty<int>(), null);

            yield return ("{ 42 }", Interval.Degenerate(42), null);

            yield return ("[42, 43]", Interval.Inclusive(42, 43), null);
            yield return ("[42; 43]", Interval.Inclusive(42, 43), null);
            yield return ("[ 42 , 43 ]", Interval.Inclusive(42, 43), null);
            yield return ("[ 42 ; 43 ]", Interval.Inclusive(42, 43), null);
        }
    }

    static IEnumerable<(string, Type, IFormatProvider?)> Interval_Core_Parse_InvalidInputs
    {
        get
        {
            // Empty or whitespace-only strings
            yield return ("", typeof(int), null);
            yield return (" ", typeof(int), null);
            yield return ("  ", typeof(int), null);
            yield return ("\t", typeof(int), null);
            yield return ("\n", typeof(int), null);

            // Missing brackets
            yield return ("42", typeof(int), null);
            yield return ("42,43", typeof(int), null);
            yield return ("42;43", typeof(int), null);
            yield return ("10,20", typeof(int), null);

            // Empty brackets
            yield return ("[]", typeof(int), null);
            yield return ("()", typeof(int), null);
            yield return ("[)", typeof(int), null);
            yield return ("(]", typeof(int), null);

            // Missing separators
            yield return ("[42]", typeof(int), null);
            yield return ("(42)", typeof(int), null);
            yield return ("{42,43}", typeof(int), null);

            // Missing values
            yield return ("[42,]", typeof(int), null);
            yield return ("[,42]", typeof(int), null);
            yield return ("[,]", typeof(int), null);
            yield return ("[;]", typeof(int), null);
            yield return ("[42;]", typeof(int), null);
            yield return ("[;42]", typeof(int), null);

            // Invalid infinity representations
            yield return ("(∞,∞)", typeof(int), null);
            yield return ("(-∞,-∞)", typeof(int), null);
            yield return ("(∞,-∞)", typeof(int), null);
            yield return ("{∞}", typeof(int), null);
            yield return ("(infinity,infinity)", typeof(int), null);
            yield return ("(-∞∞,∞∞)", typeof(int), null);
            yield return ("[infinity]", typeof(int), null);
        }
    }
}
