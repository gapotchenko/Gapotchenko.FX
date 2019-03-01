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

To make this scenario work as designed, we should use an `Optional<T>` value provided by Gapotchenko FX. Like so:

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
