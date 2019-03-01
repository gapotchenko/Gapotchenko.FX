# Gapotchenko.FX
`Gapotchenko.FX` is the main module of Gapotchenko.FX framework. Coincidently, it has the very same name.



The module was started by creating its first building block: `ArrayEqualityComparer`.

Sure enough, .NET provides a similar `Enumerable.SequenceEqual` method (`System.Linq`) that allows to check two sequences for equality, however it is very limited:
* It is slow, and puts a pressure on GC by allocating iterator objects
* It does not treat `null` arguments well
* It does not provide an implementation of `IEqualityComparer<T>` interface.
Good luck trying to make something like `Dictionary<byte[], string>`.

Many years had passed until it became clear that original platform maintainer is not going to solve that.

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

And what about `Dictionary<byte[], string>`? Here it is:

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
so it's important to keep that in mind when we study its main `Gapotchenko.FX` module.

Some concepts may seem a bit odd at first look.
However, they allow to reap the great benefits. Let's see how and why that happens.

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

To make this scenario work as designed, we should use an `Optional<T>` value provided by Gapotchenko.FX. Like so:

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
Due to the fact that underlying primitives are well-written, the combined outcome also tends to be excellent.

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

We can do better with Gapotchenko.FX:

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
We used the `Empty.Nullify` primitive, combined it with `Optional<T>` primitive and got an excellent result.

