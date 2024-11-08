# Gapotchenko.FX.Data.Encoding.Base32
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Encoding.Base16.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Encoding.Base32)

The module provides the implementation of binary-to-text encoding algorithms belonging to the Base32 family.

## Quick Start

Use the following code to get the textual representation of the specified data in Base32 encoding:

``` C#
using Gapotchenko.FX.Data.Encoding;

var encodedText = Base32.GetString(data);
```

To get the data back from the textual representation, use the following method:

``` C#
var decodedData = Base32.GetBytes(encodedText);
```

## Static Methods vs Interface

Base32 encoding classes provide a set of convenient static methods that represent the most commonly used operations.
For a more general and full use, however, the classes implement `ITextDataEncoding` interface which can be accessed via the `Instance` property of the corresponding encoding class.

## Iterative Data Processing

If you need to encode the data iteratively then the following approach becomes handy:

``` C#
var encoding = Base32.Instance;

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

``` C#
var encoding = Base32.Instance;

// Create a streaming decoder that iteratively reads the encoded text
// from the specified text reader and decodes the data on the fly.
var stream = encoding.CreateDecoder(textReader);

// Iteratively stream the decoded data.
stream.Read(...); // block 1
// ...
stream.Read(...); // block N
```

The streaming encoder and decoder can be used to [iteratively transcode the data from one encoding to another](../Gapotchenko.FX.Data.Encoding#transcoding-between-various-binary-to-text-encodings).

## Available Base32 Algorithms

Base32 family of binary-to-text data encodings consists of several attested algorithms with predefined parameters:

| Algorithm | Gapotchenko.FX Implementation | Alphabet | Case-Sensitive | Data Encoding Efficiency* |
| --------- | -------- | -------- | -------- | -------- | 
| Base32 (standard, recommended) | `Base32` | ABCDEFGHIJKLMNOPQRSTUVWXYZ234567 | No | 0.625 |
| base32-hex | `Base32Hex` | 0123456789ABCDEFGHIJKLMNOPQRSTUV | No | 0.625 |
| Crockford Base 32 | `CrockfordBase32` | 0123456789ABCDEFGHJKMNPQRSTVWXYZ*~$=U | No | 0.625 |
| z-base-32 | `ZBase32` | ybndrfg8ejkmcpqxot1uwisza345h769 | No | 0.625 |

\* Data encoding efficiency is the ratio between the amount of original data and its encoded representation.

## Recommended Base32 Algorithm

Among all other possibilities, it is recommended to use the standard Base32 algorithm which is provided by `Base32` class.

All other predefined algorithms are provided by the corresponding classes of `Gapotchenko.FX.Data.Encoding.Base32` module.

## Custom Base32 Algorithms

Once in a while, you may encounter a custom Base32 algorithm that is neither widely known nor characterized.
In that case, you can instantiate a custom data encoding algorithm with the desired parameters by hand. For example:

``` C#
var encoding = new CustomBase32("ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ01234567");
```

If you want to formalize the custom algorithm even further, you may opt in to create a separate class for it with a convenient accessor property:

``` C#
/// <summary>
/// Defines a custom Base32 data encoding algorithm.
/// </summary>
sealed class FooBase32 : CustomBase32
{
    FooBase32() :
        base("ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ01234567")
    {
    }

    public static FooBase32 Instance { get; } = new();
}
```

That will allow you to use the algorithm effortlessly from several places in the codebase later:

``` C#
var encodedText = FooBase32.Instance.GetString(...);
```

## Usage

`Gapotchenko.FX.Data.Encoding.Base32` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Encoding.Base32):

```
PM> Install-Package Gapotchenko.FX.Data.Encoding.Base32
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../Gapotchenko.FX.Data.Encoding#readme)
  - [Gapotchenko.FX.Data.Encoding](../Gapotchenko.FX.Data.Encoding#readme)
    - [Gapotchenko.FX.Data.Encoding.Base16](../Gapotchenko.FX.Data.Encoding.Base16#readme)
    - [Gapotchenko.FX.Data.Encoding.Base24](../Gapotchenko.FX.Data.Encoding.Base24#readme)
    - &#x27B4; [Gapotchenko.FX.Data.Encoding.Base32](../Gapotchenko.FX.Data.Encoding.Base32#readme)
    - [Gapotchenko.FX.Data.Encoding.Base64](../Gapotchenko.FX.Data.Encoding.Base64#readme)
  - [Gapotchenko.FX.Data.Integrity.Checksum](../../Integrity/Checksum/Gapotchenko.FX.Data.Integrity.Checksum#readme)
- [Gapotchenko.FX.Diagnostics](../../../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../../../Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../../../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../../../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../../Gapotchenko.FX.Tuples#readme)

Or look at the [full list of modules](../../../../..#readme).
