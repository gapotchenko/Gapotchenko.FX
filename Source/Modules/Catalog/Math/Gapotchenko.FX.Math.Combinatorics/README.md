# Gapotchenko.FX.Math.Combinatorics

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Math.Combinatorics.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math.Combinatorics)

The module provides math operations for combinatorics.

## Permutations

Permutation is a widely used operation that returns all possible permutations of elements in a sequence.

Let's take a look at the sample code:

``` C#
using Gapotchenko.FX.Math.Combinatorics;

var seq = new[] { "A", "B", "C" };

foreach (var i in Permutations.Of(seq))
    Console.WriteLine(string.Join(" ", i));
```

The code produces the following output:

```
A B C
A C B
B A C
B C A
C A B
C B A
```

### Unique Permutations

By default, the generated permutations are positional,
e.g. if the input sequence contains some duplicates, there will be duplicate permutations in the output.

Let's see an example of non-unique permutations:

``` C#
var seq = new[] { 1, 2, 1 };

foreach (var i in Permutations.Of(seq))
    Console.WriteLine(string.Join(" ", i.Select(x => x.ToString())));
```

The output contains some duplicates, which is expected:

```
1 2 1
1 1 2
2 1 1
2 1 1
1 1 2
1 2 1
```

But if you want to get the *unique permutations*, there exists a straightforward way (note the `Distinct` operation):

``` C#
var seq = new[] { 1, 2, 1 };

foreach (var i in Permutations.Of(seq).Distinct())
    Console.WriteLine(string.Join(" ", i.Select(x => x.ToString())));
```

which will produce the following output:

```
1 1 2
1 2 1
2 1 1
```

An experienced engineer will definitely spot that while that approach produces the correct result, it is not the most efficient way of achieving it.

It all comes to the number of elements processed by the `Distinct` operation.
The number of resulting permutations is `n!` where `n` is the size of the input sequence.
So if we take care to perform `Distinct` on the input sequence instead, we are going to achieve considerable savings in the number of operations and amount of used memory.
Like so:

``` C#
foreach (var i in Permutations.Of(seq.Distinct()))
    Console.WriteLine(string.Join(" ", i.Select(x => x.ToString())));
```

(Note that `Distinct` operation is now applied to the input sequence, supposedly making the whole algorithm more efficient while producing the same results)

This whole way of thinking stands true but `Gapotchenko.FX.Math.Combinatorics` goes ahead of that and provides the out-of-the-box support for such natural idiosyncrasies.

Whatever syntax is used: `Permutations.Of(seq.Distinct())` or `Permutations.Of(seq).Distinct()`,
the algorithm complexity stays at bay thanks to the built-in optimizer that chooses the best execution plan of a query <ins>automatically</ins>.

### Permutations in LINQ

`Permutations.Of(...)` is an explicit form of producing the permutations, but the LINQ shorthand `Permute()` is also available as a part of `Gapotchenko.FX.Math.Combinatorics` fluent API:

``` C#
using Gapotchenko.FX.Math.Combinatorics;

foreach (var i in new[] { "A", "B", "C" }.Permute())
    // ...
```

## Cartesian Product

Another widespread combinatorial primitive is a Cartesian product:

``` C#
using Gapotchenko.FX.Math.Combinatorics;

var seq1 = new[] { "1", "2" };
var seq2 = new[] { "A", "B", "C" };

foreach (var i in CartesianProduct.Of(seq1, seq2))
    Console.WriteLine(string.Join(" ", i));
```

The output:

```
1 A
2 A
1 B
2 B
1 C
2 C
```

### Cartesian Product in LINQ

`CartesianProduct.Of(...)` is an explicit form of producing the Cartesian product, but the LINQ shorthand `CrossJoin(...)` is also available as a part of `Gapotchenko.FX.Math.Combinatorics` fluent API:

``` C#
using Gapotchenko.FX.Math.Combinatorics;

foreach (var i in new[] { "1", "2" }.CrossJoin(new[] { "A", "B", "C" }))
    // ...
```

Note the naming difference between `CartesianProduct` and `CrossJoin`.
LINQ is historically built around data access,
and data access, in turn, historically uses the term `cross join` for Cartesian product.

## Usage

`Gapotchenko.FX.Math.Combinatorics` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Math.Combinatorics):

```
PM> Install-Package Gapotchenko.FX.Math.Combinatorics
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../Data/Encoding/Gapotchenko.FX.Data.Encoding#readme)
- [Gapotchenko.FX.Diagnostics](../../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../../Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math#readme)
  - &#x27B4; [Gapotchenko.FX.Math.Combinatorics](.#readme)
  - [Gapotchenko.FX.Math.Graphs](../Gapotchenko.FX.Math.Graphs#readme)
  - [Gapotchenko.FX.Math.Intervals](../Gapotchenko.FX.Math.Intervals#readme)
  - [Gapotchenko.FX.Math.Metrics](../Gapotchenko.FX.Math.Metrics#readme)
- [Gapotchenko.FX.Memory](../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../Gapotchenko.FX.Tuples#readme)

Or look at the [full list of modules](../../..#readme).
