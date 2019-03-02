# Gapotchenko.FX.Collections

The module was started by borrowing an implementation of `ConcurrentHashSet` from [Mr. Bar Arnon](https://github.com/i3arnon).

Other than that, the module does not offer much yet, mostly polyfills for missing functionality in .NET BCL.

## ConcurrentHashSet&lt;T&gt;

`ConcurrentHashSet<T>` provided by `Gapotchenko.FX.Collections` is a thread-safe implementation of `HashSet<T>`.

For some strange reason, such an important primitive is missing in baseline .NET.
But, of course, you have it out of the box when you use Gapotchenko.FX.

## Polyfills

### KeyValuePair Polyfill

.NET provides a versatile `KeyValuePair<TKey, TValue>` struct and suggests a default way for its instantiation:

``` csharp
new KeyValuePair<TKey, TValue>(key, value)
```

Which is, well, not handy as it often comes to this:

``` csharp
new KeyValuePair<BindingManagerDataErrorEventHandler, ICom2PropertyPageDisplayService>(key, value)
```

`Gapotchenko.FX.Collections` provides a better way to instantiate a `KeyValuePair` struct:

``` csharp
using Gapotchenko.FX.Collections.Generic;

KeyValuePair.Create(key, value)
```

It leverages the automatic type inference provided by some .NET languages like C#.

Gapotchenko.FX does not stop there and also provides a function for automatic `KeyValuePair<TKey, TValue>` deconstruction, so you can write this:

``` csharp
using Gapotchenko.FX.Collections.Generic;

void ProcessMap(IDictionary<string, int> map)
{
    foreach (var (key, value) in map)
    {
        // ...
    }
}
```

instead of a more clunky variant:

``` csharp
void ProcessMap(IDictionary<string, int> map)
{
    foreach (var i in map)
    {
        var key = i.Key;
        var value = i.Value;
        // ...
    }
}
```

A little detail, but sometimes it matters a lot when you are amid the heat deep inside the code.

## Other Modules

Let's continue with a look to some other modules provided by Gapotchenko.FX framework:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.Diagnostics.Process](../Gapotchenko.FX.Diagnostics.Process)
