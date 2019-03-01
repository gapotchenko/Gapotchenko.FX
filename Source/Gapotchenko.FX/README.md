# Gapotchenko.FX
`Gapotchenko.FX` is the main module of Gapotchenko.FX framework. Coincidently, it has the very same name.



The module was started by creating its first building block: `ArrayEqualityComparer`.

Sure enough, .NET provides a similar `Enumerable.SequenceEqual` method (`System.Linq`) that allows to check two sequences for equality, however it is very limited:
* It is slow, and puts a pressure on GC by allocating iterator objects
* It does not treat `null` arguments well
* It does not provide an implementation of `IEqualityComparer<T>` interface.
Good luck trying to make something like `Dictionary<byte[], string>`.

Many years had passed until it became clear that original platform maintainer is not going to solve that.
Don't mind millions of people* are suffering.
The world can wait.

(* 8 million is approximate number of .NET developers in the world)

As you can imagine, this whole situation gave an initial spark to Gapotchenko.FX project.

What if we have an `ArrayEqualityComparer` that does the job out of the box?
What if it does the job in the fastest possible way by leveraging the properties of host CPU and platform?

No problem. Now we have it:

``` csharp
using Gapotchenko.FX;
using System;

var a1 = new byte[] { 1, 2, 3 };
var a2 = new byte[] { 1, 2, 3 };
bool f = ArrayEqualityComparer.Equals(a1, a2);
Console.WriteLine(f);
```

And what about `Dictionary<byte[], string>`? Here you go:

``` csharp
var map = new Dictionary<byte[], string>(ArrayEqualityComparer<byte>.Default);

var key1 = new byte[] { 1, 2, 3 };
var key2 = new byte[] { 110, 230, 36 };

map[key1] = "Easy";
map[key2] = "Complex";

Console.WriteLine(map[key1]);
```
