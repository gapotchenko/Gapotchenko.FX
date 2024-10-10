# Gapotchenko.FX.Data.Encoding.Base16
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Encoding.Base16.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Encoding.Base16)

The module provides the implementation of binary-to-text encoding algorithms belonging to the Base16 family.

## Quick Start

Use the following code to get the textual representation of the specified data in Base16 encoding:

``` c#
using Gapotchenko.FX.Data.Encoding;

var encodedText = Base16.GetString(data);
```

To get the data back from the textual representation, use the following method:

``` c#
var decodedData = Base16.GetBytes(encodedText);
```

## Static Methods vs Interface

Base16 encoding classes provide a set of convenient static methods that represent the most commonly used operations.
For a more general and full use, however, the classes implement `ITextDataEncoding` interface which can be accessed via the `Instance` property of the corresponding encoding class.

## Iterative Data Processing

If you need to encode the data iteratively then the following approach becomes handy:

``` c#
var encoding = Base16.Instance;

// Create a streaming encoder that iteratively encodes the data and
// writes the encoded text to the specified text writer.
var stream = encoding.CreateEncoder(textWriter);

// Iteratively stream the data to encode.
stream.Write(...); // block 1
// ...
stream.Write(...); // block N

// Flush the data to the underlying text writer.
stream.Flush();
```

The same approach is valid for the decoding operation which is the opposite of the encoding:

``` c#
var encoding = Base16.Instance;

// Create a streaming decoder that iteratively reads the encoded text
// from the specified text reader and decodes the data on the fly.
var stream = encoding.CreateDecoder(textReader);

// Iteratively stream the decoded data.
stream.Read(...); // block 1
// ...
stream.Read(...); // block N
```

## Available Base16 Algorithms

Base16 family of binary-to-text data encodings consists of the following algorithms with predefined parameters:

| Algorithm | Gapotchenko.FX Implementation | Alphabet | Case-Sensitive | Data Encoding Efficiency* |
| ---------  | -------- | -------- | -------- | -------- | 
| Base16 | `Base16` | 0123456789ABCDEF | No | 0.5 |

\* Data encoding efficiency is the ratio between the amount of original data and its encoded representation.

## Custom Base16 Algorithms

Once in a while, you may encounter a custom Base16 algorithm that is neither widely known nor characterized.
In that case, you can instantiate a custom data encoding algorithm with the desired parameters by hand.
For example:

``` c#
var encoding = new CustomBase16("ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠ");
```

If you want to formalize a custom algorithm even further, you may opt in to create a separate class for it with a convenient accessor property:

``` c#
/// <summary>
/// Defines a custom Base16 data encoding algorithm.
/// </summary>
sealed class FooBase16 : CustomBase16
{
    FooBase16() :
        base("ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠ")
    {
    }

    public static FooBase16 Instance { get; } = new FooBase16();
}
```

That will allow you to use the algorithm effortlessly from several places in the codebase later:

``` c#
var encodedText = FooBase16.Instance.GetString(...);
```

## Usage

`Gapotchenko.FX.Data.Encoding.Base16` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Encoding.Base16):

```
PM> Install-Package Gapotchenko.FX.Data.Encoding.Base16
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../../../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../../../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../../../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data](../Gapotchenko.FX.Data.Encoding)
  - [Gapotchenko.FX.Data.Encoding](../Gapotchenko.FX.Data.Encoding)
    - &#x27B4; [Gapotchenko.FX.Data.Encoding.Base16](../Gapotchenko.FX.Data.Encoding.Base16)
    - [Gapotchenko.FX.Data.Encoding.Base24](../Gapotchenko.FX.Data.Encoding.Base24)
    - [Gapotchenko.FX.Data.Encoding.Base32](../Gapotchenko.FX.Data.Encoding.Base32)
    - [Gapotchenko.FX.Data.Encoding.Base64](../Gapotchenko.FX.Data.Encoding.Base64)
  - [Gapotchenko.FX.Data.Integrity.Checksum](../../Integrity/Checksum/Gapotchenko.FX.Data.Integrity.Checksum)
- [Gapotchenko.FX.Diagnostics](../../../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../../../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../../../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../../../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../../../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Security.Cryptography](../../../Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../../../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../../../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.ValueTuple](../../../Gapotchenko.FX.ValueTuple)

Or look at the [full list of modules](../../..#available-modules).
