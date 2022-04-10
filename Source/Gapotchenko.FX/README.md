# Gapotchenko.FX

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.svg)](https://www.nuget.org/packages/Gapotchenko.FX)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.svg)](https://www.nuget.org/packages/Gapotchenko.FX)

`Gapotchenko.FX` is the main module of Gapotchenko.FX toolkit. Coincidently, they have identical names.

The module was started by creating its first building block `ArrayEqualityComparer` back in 2014.

Sure enough, .NET provided a similar `Enumerable.SequenceEqual` method (`System.Linq`) that allowed to check two sequences for equality, however it was (and is) very limited:
* It is slow, and puts a pressure on GC by allocating iterator objects
* It does not treat `null` arguments well
* It does not provide an implementation of `IEqualityComparer<T>` interface.
Good luck trying to make something like `Dictionary<byte[], string>`.

Many years had passed until it became clear that original platform maintainer is not going to solve that.

As you can imagine, this whole situation gave an initial spark to Gapotchenko.FX project.

What if we have an `ArrayEqualityComparer` that does the job out of the box?
What if it does the job in the fastest possible way by leveraging the properties of host CPU and platform?

`Gapotchenko.FX` provides the exact solution:

``` csharp
using Gapotchenko.FX;
using System;

var a1 = new byte[] { 1, 2, 3 };
var a2 = new byte[] { 1, 2, 3 };
bool f = ArrayEqualityComparer.Equals(a1, a2);
Console.WriteLine(f);
```

And what about `Dictionary<byte[], string>` scenario? It's easy now:

``` csharp
var map = new Dictionary<byte[], string>(ArrayEqualityComparer<byte>.Default);

var key1 = new byte[] { 1, 2, 3 };
var key2 = new byte[] { 110, 230, 36 };

map[key1] = "Easy";
map[key2] = "Complex";

Console.WriteLine(map[key1]);
```

## Functional Influence

Gapotchenko.FX has a strong influence from functional languages and paradigms,
so it's important to keep that in mind when studying `Gapotchenko.FX` module.

Some concepts may seem a bit odd at first look.
However, they allow to reap the _great_ benefits. Let's see how and why that happens.

## Optional Values

.NET provides a notion of nullable values. For example, a nullable `int` value:

``` csharp
int? x = null;
if (!x.HasValue)
	x = 10;
Console.WriteLine(x);
```

But what if we want to do that with a reference type like `string`? Actually, we can:

``` csharp
string s = null;
if (s == null)
	s = "Test";
Console.WriteLine(s);
```

Unfortunately, the scheme breaks at the following example:

``` csharp
class Deployment
{
	string m_CachedHomeDir;

	public string HomeDir
	{
		get
		{
			if (m_CachedHomeDir == null)
				m_CachedHomeDir = Environment.GetEnvironmentVariable("PRODUCT_HOME");
			return m_CachedHomeDir;
		}
	}
}
```

If `PRODUCT_HOME` environment variable is not set (e.g. its value is `null`), then `GetEnvironmentVariable` method will be called again and again
diminishing the value of provided caching.

To make this scenario work as designed, we should use an `Optional<T>` value provided by `Gapotchenko.FX`. Like so:

``` csharp
using Gapotchenko.FX;

class Deployment
{
	Optional<string> m_CachedHomeDir;

	public string HomeDir
	{
		get
		{
			if (!m_CachedHomeDir.HasValue)
				m_CachedHomeDir = Environment.GetEnvironmentVariable("PRODUCT_HOME");
			return m_CachedHomeDir.Value;
		}
	}
}
```

Optional values are pretty common in functional languages. And they are simple enough to grasp.
But let's move to a more advanced topic - a notion of emptiness.

## Notion of Emptiness

Functional style is very similar to Unix philosophy.
There are tools, they do their job and they do it well.
Those Unix tools can be easily combined into more complex pipelines by redirecting inputs and outputs to form a chain.

Functional programming is no different.
There are primitives, and they can be combined to quickly achieve the goal.
Due to the fact that underlying primitives are well-written and have no side effects, the combined outcome also tends to be excellent.

Let's take a look at the notion of emptiness provided by the `Empty` class from `Gapotchenko.FX` assembly.

The basic thing it does is nullifying. Say, we have the following code:

``` csharp
using Gapotchenko.FX;

class Deployment
{
	Optional<string> m_CachedHomeDir;

	public string HomeDir
	{
		get
		{
			if (!m_CachedHomeDir.HasValue)
				m_CachedHomeDir = Environment.GetEnvironmentVariable("PRODUCT_HOME");
			return m_CachedHomeDir.Value;
		}
	}
}
```

It's all good, but in real world the `PRODUCT_HOME` environment variable may be set to an empty string `""`
on a machine of some customer.

Let's improve the code to handle that condition:

``` csharp
using Gapotchenko.FX;

class Deployment
{
	Optional<string> m_CachedHomeDir;

	public string HomeDir
	{
		get
		{
			if (!m_CachedHomeDir.HasValue)
			{
				string s = Environment.GetEnvironmentVariable("PRODUCT_HOME");
				if (string.IsNullOrEmpty(s))
				{
					// Treat an empty string as null. The value is absent.
					s = null;
				}
				m_CachedHomeDir = s;
			}
			return m_CachedHomeDir.Value;
		}
	}
}
```

It does the job but that's a lot of thought and code.

We can do better with `Empty.Nullify` primitive:

``` csharp
using Gapotchenko.FX;

class Deployment
{
	Optional<string> m_CachedHomeDir;

	public string HomeDir
	{
		get
		{
			if (!m_CachedHomeDir.HasValue)
				m_CachedHomeDir = Empty.Nullify(Environment.GetEnvironmentVariable("PRODUCT_HOME"));
			return m_CachedHomeDir.Value;
		}
	}
}
```

A simple one-liner.
We combine `Empty.Nullify` and `Optional<T>` primitives in order to get a quick, sound result.

## Lazy Evaluation

Most .NET languages employ eager evaluation model. But sometimes it may be beneficial to perform lazy evaluation.

.NET comes pre-equipped with `Lazy<T>` primitive that does a decent job.
However, during the years of extensive `Lazy<T>` usage it became evident that there are a few widespread usage scenarios where it becomes an overkill.

First of all, `Lazy<T>` is a class, even in cases where it might be a struct.
That means an additional pressure on GC.
Secondly, `Lazy<T>` employs a sophisticated concurrency model where you can select the desired thread-safety level.
That means an additional bookkeeping of state and storage, and thus fewer inlining opportunities for JIT.

`Gapotchenko.FX` extends the .NET lazy evaluation model by providing the new `LazyEvaluation<T>` primitive.
`LazyEvaluation<T>` is a struct, so it has no memory allocation burden.
It does not provide thread safety; though a thread-safe variant does exist in a form of `EvaluateOnce<T>` primitive.

The sample below demonstrates a typical usage scenario for `LazyEvaluation<T>`:

``` csharp
using Gapotchenko.FX;

class Deployment
{
	LazyEvaluation<string> m_HomeDir = LazyEvaluation.Create(
		() => Empty.Nullify(Environment.GetEnvironmentVariable("PRODUCT_HOME")));

	public string HomeDir => m_HomeDir.Value;
}
```

Or as a local variable:

``` csharp
using Gapotchenko.FX;

class Program
{
	public static void Main()
	{	
		var homeDir = LazyEvaluation.Create(
			() => Empty.Nullify(Environment.GetEnvironmentVariable("PRODUCT_HOME")));
		// ...
		// Use 'homeDir' value somewhere in the code.
	}
}
```

On a side note, just look how small our `Deployment` class became now.
Using the functional composition of primitives, we were able to achieve a tiny, reliable and maintainable implementation.

## Polyfills

A tagline of Gapotchenko.FX project says "A .NET Polyfill to the Future".
But what does it really mean?
A couple of things.

First of all, Gapotchenko.FX closes the gaps in original .NET design by providing the missing functionality.

### Lazy Polyfill

For example, `Lazy<T>` class has to be constructed with a `new` keyword, like so: `new Lazy<string>(() => ...)`.
It's a no-brainer for simple types like `string`. But for custom types it quickly gets clunky:

``` csharp
new Lazy<ICom2PropertyPageDisplayService>(() => ...)
```

The good news is Gapotchenko.FX allows you to do a better job here:

``` csharp
using Gapotchenko.FX;

Lazy.Create(() => ...)
```

`Gapotchenko.FX` provides a static `Lazy` class that contains a bunch of methods for `Lazy<T>` instantiation.
It allows to leverage the type inference mechanism provided by some .NET languages like C#.
It immediately translates into less typing for you on a daily basis.

Secondly, Gapotchenko.FX provides some implementations from future versions of .NET.

### HashCode Polyfill

For example, `HashCode` struct first appeared in .NET Core 2.1.
It allows to quickly combine various hash code sources into the final value with a minimal probability of collisions.
A very decent thing that was _never_ backported to conventional .NET Framework.

`Gapotchenko.FX` provides `HashCode` so you can use it in your projects right now.
It even goes further than that by providing extension methods that are likely to appear in .NET in the future (yes, we own a Cassandra's magic ball):
- `SequenceCombine<T>(IEnumerable<T> source)`
- `AddRange<T>(IEnumerable<T> source)`

### Ex Classes

Some Gapotchenko.FX polyfill functionality cannot be packed into existing .NET structures because they don't provide enough extensibility points.
In that case, Gapotchenko.FX provides so called "Ex" classes, where "Ex" is an abbreviation of "Extended". For example:
 - `HashCodeEx`
 - `LazyInitializerEx`

## Summary

As you can see, there are many little things that shape the productive environment into one you can immediately employ, and reap the benefit in no-time.

It was the original idea of .NET when it was new.
However, it got lost somewhere along the road.
.NET is a pretty bleak set of segregated platforms nowadays (2019), mostly focused on micro-optimizations and breaking the compatibility,
sowing the wrong seeds at people's hearts.

Gapotchenko.FX was created to change that.
Its mission is to bring joy of a Rapid Application Development (RAD) back to the people.
It's a polyfill to the future, after all.

![Gapotchenko FX Ark](../../Documentation/Assets/gapotchenko-fx-ark-2019.png?raw=true ".NET is a technology for everyone.
Nobody can take it away or undermine its progress.
Gapotchenko.FX brings RAD back to the people 💪")

<div align="right">
    <a href="../../../../wiki/Manifesto">Project Manifesto</a>
    |
    <a href="../../../../wiki/RAD" title="Learn more about Rapid Application Development and why it matters">More on RAD</a>
</div>

## Supported Platforms

- .NET 5.0+
- .NET Standard 2.0+
- .NET Core 2.0+
- .NET Framework 4.6+

## Usage

The main Gapotchenko.FX module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX):

```
PM> Install-Package Gapotchenko.FX
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- &#x27B4; [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data.Integrity.Checksum](../Data/Integrity/Checksum/Gapotchenko.FX.Data.Integrity.Checksum)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Security.Cryptography](../Security/Cryptography/Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)

Or look at the [full list of modules](..#available-modules).
