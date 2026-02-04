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
            // Empty set notation
            yield return ("∅", Interval.Empty<int>(), null);
            yield return ("{}", Interval.Empty<int>(), null);

            // Mathematically empty intervals
            yield return ("(10,10)", Interval.Empty<int>(), null);
            yield return ("[10,10)", Interval.Empty<int>(), null);
            yield return ("(10,10]", Interval.Empty<int>(), null);
            yield return ("(11,10)", Interval.Empty<int>(), null);

            // Degenerate intervals
            yield return ("{42}", Interval.Degenerate(42), null);
            yield return ("[42,42]", Interval.Degenerate(42), null);

            // Infinite intervals
            yield return ("(-∞,∞)", Interval.Infinite<int>(), null);
            yield return ("(-∞,+∞)", Interval.Infinite<int>(), null);
            yield return ("(-inf,inf)", Interval.Infinite<int>(), null);
            yield return ("(-inf,+inf)", Interval.Infinite<int>(), null);

            // Half-bounded intervals (left unbounded)
            yield return ("(-∞,10]", Interval.ToInclusive(10), null);
            yield return ("(-∞,10)", Interval.ToExclusive(10), null);
            yield return ("(-inf,42]", Interval.ToInclusive(42), null);

            // Half-bounded intervals (right unbounded)
            yield return ("[10,∞)", Interval.FromInclusive(10), null);
            yield return ("(10,∞)", Interval.FromExclusive(10), null);
            yield return ("[42,+∞)", Interval.FromInclusive(42), null);
            yield return ("[42,+inf)", Interval.FromInclusive(42), null);

            // Character intervals
            yield return ("[A,Z]", Interval.Inclusive('A', 'Z'), null);
            yield return ("(A,Z)", Interval.Exclusive('A', 'Z'), null);
            yield return ("[A,Z)", Interval.InclusiveExclusive('A', 'Z'), null);
            yield return ("(A,Z]", Interval.ExclusiveInclusive('A', 'Z'), null);
            yield return ("[a,z]", Interval.Inclusive('a', 'z'), null);
            yield return ("[0,9]", Interval.Inclusive('0', '9'), null);

            // Integer intervals with negative numbers
            yield return ("[-10,10]", Interval.Inclusive(-10, 10), null);
            yield return ("(-10,10)", Interval.Exclusive(-10, 10), null);
            yield return ("[-10,0]", Interval.Inclusive(-10, 0), null);
            yield return ("[0,10]", Interval.Inclusive(0, 10), null);

            // Decimal intervals
            yield return ("[42.5,43.7]", Interval.Inclusive(42.5m, 43.7m), NumberFormatInfo.InvariantInfo);
            yield return ("[-10.5,10.5]", Interval.Inclusive(-10.5m, 10.5m), NumberFormatInfo.InvariantInfo);
            yield return ("[0.0,1.0]", Interval.Inclusive(0.0m, 1.0m), NumberFormatInfo.InvariantInfo);
            var fr = new CultureInfo("fr-FR");
            yield return ("[1,5;2,5]", Interval.Inclusive(1.5m, 2.5m), fr);

            // Double intervals
            yield return ("[1.5,2.5]", Interval.Inclusive(1.5, 2.5), NumberFormatInfo.InvariantInfo);
            yield return ("[-3.14,3.14]", Interval.Inclusive(-3.14, 3.14), NumberFormatInfo.InvariantInfo);

            // Long intervals
            yield return ("[1000000,2000000]", Interval.Inclusive(1000000L, 2000000L), null);
            yield return ("[-9223372036854775808,9223372036854775807]", Interval.Inclusive(long.MinValue, long.MaxValue), null);
            yield return ($"[{long.MinValue},{long.MaxValue}]", Interval.Inclusive(long.MinValue, long.MaxValue), null);
        }
    }

    static IEnumerable<(string, IInterval, IFormatProvider?)> Interval_Core_Parse_ValidAltInputs
    {
        get
        {
            // Empty intervals with whitespace
            yield return ("{ }", Interval.Empty<int>(), null);
            yield return ("{  }", Interval.Empty<int>(), null);

            // Degenerate with whitespace
            yield return ("{ 42 }", Interval.Degenerate(42), null);
            yield return ("{  42  }", Interval.Degenerate(42), null);
            yield return ("{ -10 }", Interval.Degenerate(-10), null);

            // Infinity (case-insensitive)
            yield return ("(-Inf,iNf)", Interval.Infinite<int>(), null);

            // Intervals with whitespace and comma separator
            yield return ("[42, 43]", Interval.Inclusive(42, 43), null);
            yield return ("[ 42 , 43 ]", Interval.Inclusive(42, 43), null);
            yield return ("[ 42,43 ]", Interval.Inclusive(42, 43), null);
            yield return ("[42 , 43]", Interval.Inclusive(42, 43), null);
            yield return ("[ -10 , 10 ]", Interval.Inclusive(-10, 10), null);

            // Intervals with whitespace and semicolon separator
            yield return ("[42; 43]", Interval.Inclusive(42, 43), null);
            yield return ("[ 42 ; 43 ]", Interval.Inclusive(42, 43), null);
            yield return ("[ 42;43 ]", Interval.Inclusive(42, 43), null);
            yield return ("[42 ; 43]", Interval.Inclusive(42, 43), null);

            // Intervals with whitespace and different boundary types
            yield return ("( 10 , 20 )", Interval.Exclusive(10, 20), null);
            yield return ("[ 10 , 20 )", Interval.InclusiveExclusive(10, 20), null);
            yield return ("( 10 , 20 ]", Interval.ExclusiveInclusive(10, 20), null);

            // Intervals with whitespace and infinity
            yield return ("( -∞ , ∞ )", Interval.Infinite<int>(), null);
            yield return ("[ 10 , ∞ )", Interval.FromInclusive(10), null);
            yield return ("( -∞ , 10 ]", Interval.ToInclusive(10), null);

            // Character intervals with whitespace
            yield return ("[ A , Z ]", Interval.Inclusive('A', 'Z'), null);
            yield return ("( A ; Z )", Interval.Exclusive('A', 'Z'), null);

            // European regional format
            yield return ("]-inf,inf[", Interval.Infinite<int>(), null);
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

            // Multiple separators
            yield return ("[42,,43]", typeof(int), null);
            yield return ("[42,;43]", typeof(int), null);
            yield return ("[42;;43]", typeof(int), null);
            yield return ("[42,43,44]", typeof(int), null);
            yield return ("[42;43;44]", typeof(int), null);

            // Missing values
            yield return ("[42,]", typeof(int), null);
            yield return ("[,42]", typeof(int), null);
            yield return ("[,]", typeof(int), null);
            yield return ("[;]", typeof(int), null);
            yield return ("[42;]", typeof(int), null);
            yield return ("[;42]", typeof(int), null);

            // Invalid infinity representations
            yield return ("[-∞,∞)", typeof(int), null);
            yield return ("(-∞,∞]", typeof(int), null);
            yield return ("[-∞,∞]", typeof(int), null);
            yield return ("(∞,∞)", typeof(int), null);
            yield return ("(-∞,-∞)", typeof(int), null);
            yield return ("(∞,-∞)", typeof(int), null);
            yield return ("{∞}", typeof(int), null);
            yield return ("(infinity,infinity)", typeof(int), null);
            yield return ("(-∞∞,∞∞)", typeof(int), null);
            yield return ("[infinity]", typeof(int), null);

            // Invalid infinity boundary positions
            yield return ("(∞,10]", typeof(int), null); // Invalid: positive infinity as left boundary
            yield return ("[-10,-∞)", typeof(int), null); // Invalid: negative infinity as right boundary

            // Invalid set notation
            yield return ("{42,43}", typeof(int), null); // Multiple values in set
            yield return ("{42,43,44}", typeof(int), null);
            yield return ("{", typeof(int), null);
            yield return ("}", typeof(int), null);
            yield return ("{42", typeof(int), null);
            yield return ("42}", typeof(int), null);

            // Invalid whitespace in critical positions
            yield return ("[ 42, 43", typeof(int), null); // Missing closing bracket
            yield return ("42, 43 ]", typeof(int), null); // Missing opening bracket
            yield return ("[42 , 43", typeof(int), null); // Missing closing bracket

            // Extra characters that shouldn't be valid
            yield return ("[42,43]extra", typeof(int), null);
            yield return ("extra[42,43]", typeof(int), null);

            // Type-specific invalid inputs
            yield return ("[42.5,43.7]", typeof(int), null); // Decimal for int
            yield return ("[A,Z]", typeof(int), null); // Character for int
        }
    }
}
