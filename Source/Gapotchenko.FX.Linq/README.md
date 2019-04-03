# Gapotchenko.FX.Linq

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Linq.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Linq)

The module provides primitives for functional processing of data sequences.

## Memoization

Memoization is a pillar of functional data processing.
You already met it quite often albeit in somewhat masked forms.

`Gapotchenko.FX.Linq` module provides the `Memoize()` extension method for `IEnumerable<T>` types.
You can use it like this:

``` csharp
using Gapotchenko.FX.Linq;

var files = Directory.EnumerateFiles(".", "*.txt").Select(Path.GetFileName).Memoize();

Console.WriteLine("A text file with an upper case letter is present: {0}", files.Any(x => x.Any(char.IsUpper)));
Console.WriteLine("A text file with a name longer than 12 letters is present: {0}", files.Any(x => x.Length > 12));
```

`Memoize()` caches the already retrieved elements of a sequence, and does it lazily.

.NET developers often use `ToList()` and `ToArray()` methods for the very same purpose.
But those methods are eager, as they retrieve _all_ elements of a sequence in one shot.
This often leads to suboptimal performance of an otherwise sound functional algorithm.

`Memoize()` solves that. This is the method you are going to use the most for LINQ caching.

## ScalarOrDefault()

The second most popular primitive provided by `Gapotchenko.FX.Linq` module is `ScalarOrDefault()` method.
It is similar to `SingleOrDefault()` from conventional .NET but with one big difference: it does not throw an exception when there are multiple elements in a sequence.

### _SingleOrDefault()_ Semantics

``` csharp
using System.Linq;

new string[0].SingleOrDefault(); // returns null
new[] { "A" }.SingleOrDefault(); // returns "A"
new[] { "A", "B" }.SingleOrDefault(); // throws an exception 😞
```

### _ScalarOrDefault()_ Semantics

``` csharp
using Gapotchenko.FX.Linq;

new string[0].ScalarOrDefault(); // returns null
new[] { "A" }.ScalarOrDefault(); // returns "A"
new[] { "A", "B" }.ScalarOrDefault(); // returns null 👍
```

In practice, `ScalarOrDefault()` semantics is a big win as it allows to _safely_ determine whether a given query converged to a scalar result.

## Usage

`Gapotchenko.FX.Linq` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Linq):

```
PM> Install-Package Gapotchenko.FX.Linq
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- &#x27B4; [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
  - [Gapotchenko.FX.Linq.Expressions](../Gapotchenko.FX.Linq.Expressions)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
