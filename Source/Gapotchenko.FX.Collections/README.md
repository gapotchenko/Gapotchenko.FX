# Gapotchenko.FX.Collections

The module was started by borrowing an implementation of `ConcurrentHashSet<T>` from [Mr. Bar Arnon](https://github.com/i3arnon).

Other than that, the module provides polyfills for missing functionality in .NET.

## ConcurrentHashSet&lt;T&gt;

`ConcurrentHashSet<T>` provided by `Gapotchenko.FX.Collections` is a thread-safe implementation of `HashSet<T>`.

## AddRange&lt;T&gt;(IEnumerable&lt;T&gt;) for Collections

`AddRange` is a highly demanded operation that allows to add a sequence of elements to the end of a collection.
Like this:

``` csharp
using Gapotchenko.FX.Collections.Generic;

var collection = new Collection<int>();
…
collection.AddRange(numbers.Where(x => x % 2 == 0)); // add even numbers
```


## IsNullOrEmpty() for Containers

`System.String` class provides a convenient `IsNullOrEmpty` primitive for strings.

`Gapotchenko.FX.Collections` extends availability of the primitive by providing `IsNullOrEmpty()` extension method for other container types.
Here is an example:

``` csharp
using Gapotchenko.FX.Collections;

class Method
{
    List<Argument> m_Arguments;

    public bool HasArguments => !m_Arguments.IsNullOrEmpty();

    …
}
```

The use of an extension method for a null check may look controversial at first,
but since the method clearly starts with `IsNull…` prefix,
it immediately communicates its purpose and function.
Other benefits include automatic type inference and semantic locality. 

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

## Other Modules

Let's continue with a look to some other modules provided by Gapotchenko.FX platform:

- [Gapotchenko.FX](../Gapotchenko.FX)
- &#x27B4; [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.Diagnostics.Process](../Gapotchenko.FX.Diagnostics.Process)
