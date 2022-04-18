# Gapotchenko.FX.UriQueryBuilder

`UriQueryBuilder` class from `Gapotchenko.FX` module provides a custom constructor for the query part of uniform resource identifiers (URIs).
It can be used separately or in conjunction with [`System.UriBuilder`](https://docs.microsoft.com/en-us/dotnet/api/system.uribuilder).

## Standalone Usage

The standalone usage of `UriQueryBuilder` class is similar to a widely used [`System.StringBuilder`](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder) pattern:

```csharp
using Gapotchenko.FX;
using System;

// Create a UriQueryBuilder.
var uqb = new UriQueryBuilder();

// Append the first parameter.
uqb.AppendParameter("a", "1");

// Append the second parameter.
uqb.AppendParameter("b", "2");

// Display the query.
Console.WriteLine(uqb.ToString());
```

This code produces the following output:

```
a=1&b=2
```

In this way, the code can dynamically build standards-compliant URI queries according to the program logic.
`UriQueryBuilder` takes care to properly handle all the isolation and escaping rules:

```csharp
using Gapotchenko.FX;
using System;

var uqb = new UriQueryBuilder();

// Add the parameter that requires escaping.
uqb.AppendParameter("p", "a b c");

// Display the query.
Console.WriteLine(uqb.ToString());
```

This code produces the following result:

```
p=a%20b%20c
```


Once the query is built, it can be combined with a URI:

```csharp
using Gapotchenko.FX;
using System;

var uqb = new UriQueryBuilder()
	.AppendParameter("a", "1")
	.AppendParameter("rad", "productivity");

// Combine the query with a URI.
var uri = uqb.CombineWithUri("https://example.com/");

// Display the result.
Console.WriteLine(uri);
```

The code produces the following output:

```
https://example.com/?a=1&rad=productivity
```

## Usage in Conjunction With `System.UriBuilder`

`UriQueryBuilder` can be used in conjunction with [`System.UriBuilder`](https://docs.microsoft.com/en-us/dotnet/api/system.uribuilder) to build the query part of a URI:

```csharp
using Gapotchenko.FX;
using System;

// Create a UriBuilder.
// Initialize the UriBuilder with "https://example.com/?key=abc".
var ub = new UriBuilder("https://example.com/?key=abc");

// Create a UriQueryBuilder.
// Initialize the UriQueryBuilder with an existing query from URI builder.
// Append query parameters.
var uqb = new UriQueryBuilder(ub.Query)
    .AppendParameter("mode", "flow")
    .AppendParameter("complexity", "easy");

// Set the query part of a URI builder.
ub.Query = uqb.ToString();

// Display the URI.
Console.WriteLine(ub.Uri);
```

The code produces the following output:

```
https://example.com/?key=abc&mode=flow&complexity=easy
```

## See Also

- [Gapotchenko.FX module](../Gapotchenko.FX)
