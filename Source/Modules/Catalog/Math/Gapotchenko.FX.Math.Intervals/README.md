# Gapotchenko.FX.Math.Intervals

The module provides data structures and primitives for working with [intervals](https://en.wikipedia.org/wiki/Interval_(mathematics)).

## Interval&lt;T&gt;

Interval is a mathematical structure that defines a range of values in a unified and formalized way.
`Interval<T>` type represents a continuous interval of values.
For example, a teenager interval can be defined as:

``` C#
using Gapotchenko.FX.Math.Intervals;

// Define an inclusive interval of years ranging from 13 to 19.
// Inclusive interval has both its ends included in the interval.
// The formal interval representation is [13,19],
// where '[' denotes the start of the interval (inclusive),
// and ']' denotes the end of the interval (inclusive).
var teenagers = Interval.Inclusive(13, 19);
```

Given that range, it is now possible to do various operations:

```
Console.Write("Enter your age: ");
var age = int.Parse(Console.ReadLine());

// Everyone of an age between 13 and 19 years (inclusive) is a teenager.
// Using the interval notation, this can be stated as: age ∈ [13,19],
// where '∈' symbol denotes the "is an element of" operation,
if (teenagers.Contains(age))
    Console.WriteLine("Congrats, you are a teenager.");
else
    Console.WriteLine("Congrats, you are not a teenager.");
```

In this simple example, the value is just a number but it can be any comparable type.
For example, it can be a `System.Version`, a `System.DateTime`, etc.

## Interval Operations

The real power of intervals comes when you need to perform certain operations on intervals:

``` C#
// Adults are people of 18 years or older
// (the exact age of adulthood depends on a jurisdiction but we use 18 for simplicity).
// Using the interval notation, this is [18,∞),
// where ')' denotes the end of the interval (non-inclusive),
var adults = Interval.FromInclusive(18);

Console.Write(
    "Can teenagers be adults? The answer is {0}."
    teenagers.Overlaps(adults) ? "yes" : "no");

Console.WriteLine(
    "Adult teenagers have an age of {0}",
    teenagers.Intersect(adults));
```

The program produces the following output:

```
Can teenagers be adults? The answer is yes.
Adult teenagers have an age of [18,19].
```

## ValueInterval&lt;T&gt;

`ValueInterval<T>` type provides a similar functionality as `Interval<T>` but it is a structure in terms of .NET type system, while `Interval<T>` is a class.
The difference is that `ValueInterval<T>` can be allocated on stack without involving expensive GC memory allocations, also it has tinier memory footprint.

All in all, `ValueInterval<T>` is the preferred interval type to use.
Being totally transparent and interchangeable with `Interval<T>`, it comes with certain restrictions.
For example, `ValueInterval<T>` cannot use a custom `System.IComparer<T>`, and thus it requires `T` type to implement `System.IComparable<T>` interface.
This is not an obstacle for most specializing types, but still this is a formal restriction that may affect your choice in favor of `Interval<T>`.

Another scenario where you may prefer `Interval<T>` type better is when you need to pass it as a reference to many places in code.
This may save some CPU time and memory in cases where `T` type is sufficiently large because passing by reference avoids copying.
