# Gapotchenko.FX.Unit

`Unit` is a class defined in `Gapotchenko.FX` module.

`Unit` class cannot be instantiated and thus can hold no information.
Its notion comes from the area of mathematical logic and computer science known as [type theory](https://en.wikipedia.org/wiki/Type_theory).

The `Unit` class can only have a single possible value (`null`):

``` csharp
using Gapotchenko.FX;

Unit val;

val = null;        // OK: null is the only possible value for Unit
val = new Unit();  // ERROR: wouldn't compile
```

In this way, the unit type is the terminal object in the category of types and typed functions.
It should not be confused with the zero or bottom type (`System.Void`), which allows no values and is the initial object in this category.
Similarly, the `System.Boolean` is the type with two values.

A visual placement of the `Unit` type in .NET type system:

- `Void` (allows *zero* possible values)
- **`Unit`** (allows *one* possible value: `null`)
- `Boolean` (allows *two* possible values: `true` | `false`)
- …

## Usage

Some time ago, .NET had no `HashSet<T>` class.
So whenever a developer needed a set, he used a dictionary which was readily available:

``` csharp
var set = new Dictionary<string, object>();
```

The values in these dictionaries were always `null`, and only the keys represented the useful payload.

What most developers did not know then, is that they were essentially using a Unit type in disguise.
So a better way to write this would be:

``` csharp
var set = new Dictionary<string, Unit>();
```

so that nobody could perform an erroneous operation like `stringSet["contoso"] = DateTime.UtcNow` which is not intended for a set.

Now you see this. 
The `Unit` type can be used as a parameter in generic specializations whenever your intent is to "void" a specific parameter because you do not need it.

Actually, it would be cool if we could write something like:

- `Func<void>`
- `Dictionary<string, void>`

But .NET type system disallows `System.Void` as a generic parameter type.
Instead, we have the second best alternative out there: the `Unit` type from `Gapotchenko.FX` module.

## The History Tends to Repeat Itself

You think that the whole story with `Dictionary` as a `HashSet` is the thing of the past?

Surprise, surprise, meet a `ConcurrentHashSet` which is… still absent in conventional .NET.
So you know what to do now:

```
var set = new ConcurrentDictionary<string, Unit>();
```

But there is a better way.
You can just use the readily available [`ConcurrentHashSet`](../Gapotchenko.FX.Collections/Concurrent/ConcurrentHashSet.cs) from [`Gapotchenko.FX.Collections`](../Gapotchenko.FX.Collections#gapotchenkofxcollections) module.

## See Also

- [Gapotchenko.FX module](../Gapotchenko.FX)
