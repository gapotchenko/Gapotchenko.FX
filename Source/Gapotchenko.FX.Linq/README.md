# Gapotchenko.FX.Linq

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

It caches the already retrieved elements of a sequence, and does it lazily.

.NET developers often use `ToList()` and `ToArray()` methods for the very same purpose.
But those methods are eager, as they retrieve all elements of a sequence in one shot.
This often leads to suboptimal performance of an otherwise sound functional algorithm.

`Memoize()` solves that. This is the method you are going to use the most for LINQ caching.

## Other Modules

Let's continue with a look to some other modules provided by Gapotchenko.FX framework:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- &#x27B4; [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.Diagnostics.Process](../Gapotchenko.FX.Diagnostics.Process)
