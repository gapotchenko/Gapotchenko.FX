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
    public void Interval_Core_Parse_ValidInput(string input, object expectedInterval, IFormatProvider? provider)
    {
        var type = expectedInterval.GetType().GetGenericArguments()[0];
        var actualInterval = CallTestTryParse(type, input, provider);
        if (!CallIntervalEquals(expectedInterval, actualInterval))
            Assert.Fail($"Expected interval is {expectedInterval}, but the actual is {actualInterval}.");
    }

    [TestMethod]
    [DynamicData(nameof(Interval_Core_Parse_InvalidInputs))]
    public void Interval_Core_Parse_InvalidInput(string input, Type type, IFormatProvider? provider)
    {
        var actualInterval = CallTestTryParse(type, input, provider);
        Assert.IsNull(actualInterval, $"'{input}' string should not represent a valid interval.");
    }

    object? CallTestTryParse(Type type, string input, IFormatProvider? provider)
    {
        var methodInfo = typeof(IntervalCoreTests).GetMethod(nameof(TestTryParse), BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(methodInfo);

        var specializedMethodInfo = methodInfo.MakeGenericMethod(type);
        return specializedMethodInfo.Invoke(this, [input, provider]);
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

    static bool CallIntervalEquals(object? interval, object? other)
    {
        if (ReferenceEquals(interval, other))
            return true;
        if (interval is null || other is null)
            return false;

        return (bool)interval.GetType().InvokeMember(
            "IntervalEquals",
            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
            null,
            interval,
            [other])!;
    }

    static IEnumerable<(string, object, IFormatProvider?)> Interval_Core_Parse_ValidInputs
    {
        get
        {
            yield return ("∅", Interval.Empty<int>(), null);
            yield return ("{}", Interval.Empty<int>(), null);

            yield return ("{42}", Interval.Degenerate(42), null);
            yield return ("[42,42]", Interval.Degenerate(42), null);

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

    static IEnumerable<(string, object, IFormatProvider?)> Interval_Core_Parse_ValidAltInputs
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
            yield return ("[]", typeof(int), null);
            yield return ("42", typeof(int), null);
            yield return ("[42,]", typeof(int), null);
            yield return ("[,42]", typeof(int), null);
        }
    }
}
