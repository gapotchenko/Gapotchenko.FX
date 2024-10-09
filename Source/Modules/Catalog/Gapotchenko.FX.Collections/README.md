# Gapotchenko.FX.Collections

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Collections.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Collections)

The module was started by borrowing an implementation of `ConcurrentHashSet<T>` from [Mr. Bar Arnon](https://github.com/i3arnon).

Other than that, the module provides polyfills for missing functionality in .NET.

## Collections

### ConcurrentHashSet&lt;T&gt;

`ConcurrentHashSet<T>` provided by `Gapotchenko.FX.Collections` is a thread-safe implementation of `HashSet<T>`.

### AssociativeArray&lt;TKey, TValue&gt;

`AssociativeArray<TKey, TValue>` provided by `Gapotchenko.FX.Collections` is a drop-in replacement for `Dictionary<TKey, TValue>` that can handle `null` keys.

`Dictionary<TKey, TValue>` cannot work with `null` keys and throws an exception whenever a `null` key is encountered.
`AssociativeArray<TKey, TValue>` resolves that by supporting a full space of keys without opinionated exclusions.

### Deque&lt;T&gt;

`Deque<T>` provided by `Gapotchenko.FX.Collections` is a linear collection that supports element insertion and removal at both ends with O(1) algorithmic complexity.

`Deque<T>` can be seen as a `List<T>`, but in contrast to the `List<T>`, both ends of the collection support efficient addition and removal of elements.

## Polyfills

### AddRange&lt;T&gt;(IEnumerable&lt;T&gt;) for Collections

`AddRange` is a highly demanded operation that allows to add a sequence of elements to the end of a collection.
Like this:

``` csharp
using Gapotchenko.FX.Collections.Generic;

var collection = new Collection<int>();
…
collection.AddRange(numbers.Where(x => x % 2 == 0)); // add even numbers
```

<details>
  <summary>Other polyfills</summary>

### KeyValuePair Polyfill

.NET provides a versatile `KeyValuePair<TKey, TValue>` struct and suggests a default way for its instantiation:

``` csharp
new KeyValuePair<TKey, TValue>(key, value)
```

Which is, well, not handy as it often comes to this:

``` csharp
new KeyValuePair<BindingManagerDataErrorEventHandler, ICom2PropertyPageDisplayService>(key, value)
```

`Gapotchenko.FX.Collections` provides a better way to instantiate a `KeyValuePair<TKey, TValue>` struct:

``` csharp
using Gapotchenko.FX.Collections.Generic;

KeyValuePair.Create(key, value)
```

It leverages the automatic type inference provided by some .NET languages like C#.

#### Deconstruction

`Gapotchenko.FX.Collections` module comes with a function for `KeyValuePair<TKey, TValue>` deconstruction, so you can write this:

``` csharp
using Gapotchenko.FX.Collections.Generic;

void ProcessMap(IDictionary<string, int> map)
{
    foreach (var (key, value) in map)
    {
        …
    }
}
```

instead of a more verbose variant:

``` csharp
void ProcessMap(IDictionary<string, int> map)
{
    foreach (var i in map)
    {
        var key = i.Key;
        var value = i.Value;
        …
    }
}
```

A little detail, but sometimes it matters a lot when you are amid the heat of the code.

### PriorityQueue Polyfill

`PriorityQueue<TElement, TPriority>` provided by `Gapotchenko.FX.Collections` module is an implementation of the prioritized queue available in .NET 6.0+.
The polyfill makes it available to all other supported .NET versions.

</details>

## Usage

`Gapotchenko.FX.Collections` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Collections):

```
PM> Install-Package Gapotchenko.FX.Collections
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../Gapotchenko.FX.AppModel.Information)
- &#x27B4; [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data](../Data/Encoding/Gapotchenko.FX.Data.Encoding)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Security.Cryptography](../Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.ValueTuple](../Gapotchenko.FX.ValueTuple)

Or look at the [full list of modules](..#available-modules).
