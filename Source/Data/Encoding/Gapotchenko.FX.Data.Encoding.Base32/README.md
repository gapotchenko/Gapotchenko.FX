# Gapotchenko.FX.Data.Encoding.Base32
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Encoding.Base16.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Encoding.Base32)

The module provides the implementation of binary-to-text encoding algorithms belonging to Base32 family.

## Quick Start

Use the following code to get the textual representation of the specified data in Base32 encoding:

``` c#
using Gapotchenko.FX.Data.Encoding;

var encodedText = Base32.GetString(data);
```

To get the data back from the textual representation, use the following method:

``` c#
var decodedData = Base32.GetBytes(encodedText);
```

## Iterative Data Processing

If you need to encode the data iteratively then the following approach becomes handy:

``` c#
var encoding = Base32.Instance;

// Create a streaming encoder that iteratively encodes the data and
// writes the encoded text to the specified text writer.
var stream = encoding.CreateEncoder(textWriter);

// Stream the data to encode.
stream.Write(...); // block 1
// ...
stream.Write(...); // block N

// Flush the data to the underlying text writer.
stream.Flush();
```

The same approach is valid for the decoding operation which is the opposite of the encoding:

``` c#
var encoding = Base32.Instance;

// Create a streaming decoder that iteratively reads the encoded text
// from the specified text reader and decodes the data on the fly.
var stream = encoding.CreateDecoder(textReader);

// Stream the decoded data.
stream.Read(...); // block 1
// ...
stream.Read(...); // block N
```

## Available Base32 Algorithms

Base32 family of binary-to-text data encodings consists of several attested algorithms with predefined parameters:

| Algorithm | Gapotchenko.FX Implementation | Alphabet | Case-Sensitive | Data Encoding Efficiency ** |
| --------- | -------- | -------- | -------- | -------- | 
| Base32 | `Base32` | ABCDEFGHIJKLMNOPQRSTUVWXYZ234567 | No | 0.625 |
| base32-hex | `Base32Hex` | 0123456789ABCDEFGHIJKLMNOPQRSTUV | No | 0.625 |
| Crockford Base 32 | `CrockfordBase32` | 0123456789ABCDEFGHJKMNPQRSTVWXYZ*~$=U | No | 0.625 |
| z-base-32 | `ZBase32` | ybndrfg8ejkmcpqxot1uwisza345h769 | No | 0.625 |

\* Data encoding efficiency is the ratio between the amount of original data and its encoded representation.

## Usage

`Gapotchenko.FX.Data.Encoding.Base32` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Encoding.Base32):

```
PM> Install-Package Gapotchenko.FX.Data.Encoding.Base32
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
- [Gapotchenko.FX.Security.Cryptography](../../../Security/Cryptography/Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../../../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../../../Gapotchenko.FX.Threading)

Or look at the [full list of modules](../../..#available-modules).
