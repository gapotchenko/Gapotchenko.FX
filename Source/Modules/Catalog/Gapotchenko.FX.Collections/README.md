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

`Dictionary<TKey, TValue>` cannot work with `null` keys and throws `ArgumentNullException` whenever a `null` key is encountered.
`AssociativeArray<TKey, TValue>` solves that by supporting a full space of keys without opinionated exclusions.

### Deque&lt;T&gt;

`Deque<T>` provided by `Gapotchenko.FX.Collections` is a linear collection that supports element insertion and removal at both ends with O(1) algorithmic complexity.

`Deque<T>` can be seen as a `List<T>`, but in contrast to the `List<T>`, both ends of the collection support efficient addition and removal of elements.

## Collection Construction Kits

A concept of a construction kit provided by `Gapotchenko.FX.Collections` module allows you to quickly and reliably build customized collection primitives.

<details>
  <summary>More information</summary>

### ISet&lt;T&gt; Construction Kit 

For example, let's imagine that we need to build a custom implementation of `System.Collections.Generic.ISet<T>` collection.
In order to do that, we need to implement a plethora of methods such as `UnionWith`, `IntersectWith`, `ExceptWith` and others just to begin with.
It gets complicated and nuanced quickly, while all we want is to build a simple custom `ISet<T>` implementation.

This is where the concept of a construction kit starts to shine.
In our case, instead of implementing `ISet<T>` interface directly, we just derive our implementation from the one provided by the corresponding construction kit:

``` C#
using Gapotchenko.FX.Collections.Generic.Kits;
using Gapotchenko.FX.Linq;
using System.Collections;

class MyBitSet(int capacity) : SetKit<int>
{
    public override bool Contains(int item) => m_Bits[item];

    public override bool Add(int item) => ChangeBit(item, true);

    public override bool Remove(int item) => ChangeBit(item, false);

    bool ChangeBit(int item, bool value)
    {
        if (m_Bits[item] != value)
        {
            m_Bits[item] = value;

            if (value)
                ++m_CachedCount;
            else
                --m_CachedCount;

            return true;
        }
        else
        {
            return false;
        }
    }

    public override void Clear()
    {
        m_Bits.SetAll(false);
        m_CachedCount = 0;
    }

    public override int Count => m_CachedCount ??= this.Stream().Count();

    int? m_CachedCount = 0;

    public override IEnumerator<int> GetEnumerator()
    {
        for (int i = 0, n = m_Bits.Count; i < n; ++i)
            if (m_Bits[i])
                yield return i;
    }

    protected BitArray Bits
    {
        get => m_Bits;
        set { m_Bits = value; m_CachedCount = null; }
    }

    BitArray m_Bits = new(capacity);
}
```

We implemented just several abstract methods and got a fully functional and compliant `ISet<T>` collection.
All the remaining implementation details are covered by the construction kit our class is derived from.

Mind you, a generic implementation does not mean inefficient.
If we have a more optimized way to do some operations, we just override the corresponding methods:

``` C#
class MyHWAcceleratedBitSet(int capacity) : MyBitSet(capacity)
{
    public override bool Overlaps(IEnumerable<int> other)
    {
        if (other is MyBitSet bitSet)
            return Bits.And(bitSet.Bits).HasAnySet();
        else
            return base.Overlaps(other);
    }

    public override void IntersectWith(IEnumerable<int> other)
    {
        if (other is MyBitSet bitSet)
            Bits = Bits.And(bitSet.Bits);
        else
            base.IntersectWith(other);
    }

    public override void UnionWith(IEnumerable<int> other)
    {
        if (other is MyBitSet bitSet)
            Bits = Bits.Or(bitSet.Bits);
        else
            base.UnionWith(other);
    }

    public override void ExceptWith(IEnumerable<int> other)
    {
        if (other is MyBitSet bitSet)
            Bits = Bits.And(bitSet.Bits.Not());
        else
            base.ExceptWith(other);
    }

    public override void SymmetricExceptWith(IEnumerable<int> other)
    {
        if (other is MyBitSet bitSet)
            Bits = Bits.Xor(bitSet.Bits);
        else
            base.SymmetricExceptWith(other);
    }

    public override bool SetEquals(IEnumerable<int> other)
    {
        if (other is MyBitSet bitSet)
            return bitSet.Bits.Xor(bitSet.Bits).HasAnySet();
        else
            return base.SetEquals(other);
    }
}
```

Given that `BitArray` operations are hardware-accelerated in all modern .NET versions,
it quickly boils from a generic `ISet<T>` implementation down to a highly-optimized one, leveraging AVX and SSE vector instructions provided by CPU.
What a ride just within a screen of code.

</details>

## Polyfills

### AddRange&lt;T&gt;(IEnumerable&lt;T&gt;) for Collections

`AddRange` is a frequently used operation that allows you to add a sequence of elements to the end of a collection.
Like this:

``` C#
using Gapotchenko.FX.Collections.Generic;

var collection = new Collection<int>();
…
collection.AddRange(numbers.Where(x => x % 2 == 0)); // add even numbers
```

### PriorityQueue Polyfill

`PriorityQueue<TElement, TPriority>` provided by `Gapotchenko.FX.Collections` module is an implementation of the prioritized queue available since .NET 6.0.
The polyfill makes it available to all other supported .NET versions.

<details>
  <summary>Other polyfills</summary>

### KeyValuePair Polyfill

.NET provides a versatile `KeyValuePair<TKey, TValue>` struct and suggests a default way for its instantiation:

``` C#
new KeyValuePair<TKey, TValue>(key, value)
```

Which is, well, not handy as it often comes to this:

``` C#
new KeyValuePair<BindingManagerDataErrorEventHandler, ICom2PropertyPageDisplayService>(key, value)
```

`Gapotchenko.FX.Collections` provides a better way to instantiate a `KeyValuePair<TKey, TValue>` struct:

``` C#
using Gapotchenko.FX.Collections.Generic;

KeyValuePair.Create(key, value)
```

It leverages the automatic type inference provided by some .NET languages like C#.

#### Deconstruction

`Gapotchenko.FX.Collections` module comes with a function for `KeyValuePair<TKey, TValue>` deconstruction, so you can write this:

``` C#
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

``` C#
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

</details>

## Usage

`Gapotchenko.FX.Collections` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Collections):

```
PM> Install-Package Gapotchenko.FX.Collections
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- &#x27B4; [Gapotchenko.FX.Collections](.#readme)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../Data/Encoding/Gapotchenko.FX.Data.Encoding#readme)
- [Gapotchenko.FX.Diagnostics](../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../Gapotchenko.FX.Tuples#readme)

Or look at the [full list of modules](../..#readme).
