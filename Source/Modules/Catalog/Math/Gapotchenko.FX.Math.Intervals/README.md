# Gapotchenko.FX.Math.Intervals

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Math.Intervals.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math.Intervals)

The module provides data structures and primitives for working with [intervals](https://en.wikipedia.org/wiki/Interval_(mathematics)).

## Interval&lt;T&gt;

Interval is a mathematical structure that defines a range of values in a unified and formalized way.
`Interval<T>` type represents a continuous interval of values.
For example, a human age interval can be defined as:

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

``` C#
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

The real power of intervals comes when you need to perform certain operations on them.

### Overlap Detection

`Interval<T>.Overlaps` function returns a Boolean value indicating whether a specified interval overlaps with another:

``` C#
var teenagers = Interval.Inclusive(13, 19);

// Adults are people of 18 years or older
// (the exact age of adulthood depends on a jurisdiction but we use 18 for simplicity).
// Using the interval notation, this is [18,∞),
// where ')' denotes the end of the interval (non-inclusive),
var adults = Interval.FromInclusive(18);

Console.Write(
    "Can teenagers be adults? The answer is {0}."
    teenagers.Overlaps(adults) ? "yes" : "no");
```

The snippet produces the following output:

```
Can teenagers be adults? The answer is yes.
```

### Intersection

The intersection of two intervals returns an interval which has a range shared by both of them:

``` C#
var teenagers = Interval.Inclusive(13, 19);
var adults = Interval.FromInclusive(18);

Console.WriteLine(
    "Adult teenagers have an age of {0}",
    teenagers.Intersect(adults));
```

The snippet produces the following output:

```
Adult teenagers have an age of [18,19].
```

### Union

The union of two continuous intervals has the range that covers both of them:

``` C#
var teenagers = Interval.Inclusive(13, 19);
var adults = Interval.FromInclusive(18);

Console.WriteLine(
    "Adults and teenagers have an age of {0}",
    teenagers.Union(adults));
```

The snippet produces the following output:

```
Adults and teenagers have an age of [13,inf).
```

Note the `[13,inf)` interval string in the output.
This is the ASCII variant of a formal `[13,∞)` notation.
The ASCII notation is produced by `Interval<T>.ToString()` method by default.

## Interval Construction

To define an interval, you can use a set of predefined methods provided by the static `Interval` type:

``` C#
// [10,20]
interval = Interval.Inclusive(10, 20);

// (10,20)
interval = Interval.Exclusive(10, 20);

// [10,20)
interval = Interval.InclusiveExclusive(10, 20);

// (10,20]
interval = Interval.ExclusiveInclusive(10, 20);

// [10,∞)
interval = Interval.FromInclusive(10);

// (10,∞)
interval = Interval.FromExclusive(10);

// (-∞,10]
interval = Interval.ToInclusive(10);

// (-∞,10)
interval = Interval.ToExclusive(10);
```

Or you can explicitly construct an interval by using an `Interval<T>` constructor and the notion of boundaries:

``` C#
// [10,20)
interval = new Interval<int>(IntervalBoundary.Inclusive(10), IntervalBoundary.Exclusive(20));

// (10,∞)
interval = new Interval<int>(IntervalBoundary.Exclusive(10), IntervalBoundary.PositiveInfinity<int>());
```

## Special Intervals

There are a few special intervals readily available for use:

``` C#
// An empty interval ∅
interval = Intrval.Empty<int>();

// An infinite interval (-∞,∞)
interval = Intrval.Infinite<int>();
```

## ValueInterval&lt;T&gt;

`ValueInterval<T>` type provides a similar functionality as `Interval<T>` but it is a structure in terms of .NET type system, while `Interval<T>` is a class.
The difference is that `ValueInterval<T>` can be allocated on stack without involving expensive GC memory allocations, also it has tinier memory footprint.

All in all, `ValueInterval<T>` is the preferred interval type to use.
Being totally transparent and interchangeable with `Interval<T>`, it comes with certain restrictions.
For example, `ValueInterval<T>` cannot use a custom `System.IComparer<T>`, and thus it requires `T` type to implement `System.IComparable<T>` interface.
This is not an obstacle for most specializing types, but still this is a formal restriction that may affect your choice in favor of `Interval<T>`.

Another scenario where you may prefer `Interval<T>` type better is when you need to pass it as a reference to many places in code.
This may save some CPU time and memory in cases where `T` type is sufficiently large because passing the interval by reference avoids copying.

## Usage

`Gapotchenko.FX.Math.Intervals` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Math.Intervals):

```
PM> Install-Package Gapotchenko.FX.Math.Intervals
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data](../Data/Encoding/Gapotchenko.FX.Data.Encoding)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
  - [Gapotchenko.FX.Math.Combinatorics](../Gapotchenko.FX.Math.Combinatorics)
  - [Gapotchenko.FX.Math.Geometry](../Gapotchenko.FX.Math.Geometry)
  - &#x27B4; [Gapotchenko.FX.Math.Topology](../Gapotchenko.FX.Math.Topology)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Security.Cryptography](../Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.ValueTuple](../Gapotchenko.FX.ValueTuple)

Or look at the [full list of modules](..#available-modules).
