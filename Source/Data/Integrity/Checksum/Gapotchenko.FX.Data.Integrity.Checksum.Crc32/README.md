# Gapotchenko.FX.Data.Integrity.Checksum.Crc32
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Integrity.Checksum.Crc32.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum.Crc32)

The module provides the implementation of checksum algorithms belonging to the CRC-32 family.

## Quick Start

Use the following code to calculate a CRC-32 checksum for the specified data, be it `System.Byte[]`, `System.ReadOnlySpan<Byte>` or `System.IO.Stream`:

``` c#
using Gapotchenko.FX.Data.Integrity.Checksum;

var checksum = Crc32.Standard.ComputeChecksum(data);
```

If you need to calculate a CRC-32 checksum iteratively then the following approach becomes handy:

```csharp
var iterator = Crc32.Standard.CreateIterator();

iterator.ComputeBlock(...); // block 1
// ...
iterator.ComputeBlock(...); // block N

// Compute the final checksum:
var checksum = iterator.ComputeFinal();
```

## Available CRC-32 Algorithms

CRC-32 family of cyclic redundancy checks consists of several attested checksum algorithms with predefined parameters:

| Algorithm | Aliases | Gapotchenko.FX Implementation | Parameters: poly | init | refin | refout | xorout | check |
| --------- | ------- | -------- | ---- | ---- | ----- | ------ | ------ | ----- |
| CRC-32 (standard, recommended) | CRC-32/ISO-HDLC, CRC-32/ADCCP, CRC-32/V-42, CRC-32/XZ, PKZIP | `Crc32.Standard` | 0x04c11db7 | 0xffffffff | true | true | 0xffffffff | 0xcbf43926 |
| CRC-32C | CRC-32/BASE91-C, CRC-32/CASTAGNOLI, CRC-32/INTERLAKEN, CRC-32/ISCSI | `Crc32.Attested.C` | 0x1edc6f41 | 0xffffffff | true | true | 0xffffffff | 0xe3069283 |
| CRC-32Q | CRC-32/AIXM | `Crc32.Attested.Q` | 0x814141ab | 0x00000000 | false | false | 0x00000000 | 0x3010bf7f |
| CRC-32/AUTOSAR | | `Crc32.Attested.Autosar` | 0xf4acfb13 | 0xffffffff | true | true | 0xffffffff | 0x1697d06a |
| CRC-32/POSIX | CRC-32/CKSUM, CKSUM | `Crc32.Attested.Posix` | 0x04c11db7 | 0x00000000 | false | false | 0xffffffff | 0x765e7680 |
| CRC-32/BZIP2 | CRC-32/DECT-B, CRC-32/AAL5, B-CRC-32 | `Crc32.Attested.BZip2` | 0x04c11db7 | 0xffffffff | false | false | 0xffffffff | 0xfc891918 |
| CRC-32/MEF | | `Crc32.Attested.Mef` | 0x741b8cd7 | 0xffffffff | true | true | 0x00000000 | 0xd2c22f51 |
| CRC-32/MPEG-2 | | `Crc32.Attested.Mpeg2` | 0x04c11db7 | 0xffffffff | false | false | 0x00000000 | 0x0376e6e7 |

The `check` parameter shows what checksum value an algorithm should produce for `"123456789"` input string interpreted as an ASCII data:

``` c#
// Get the byte representation of the ASCII string.
var data = Encoding.ASCII.GetBytes("123456789");

// Compute the checksum.
var checksum = Crc32.Standard.ComputeChecksum(data);

// Print out the result ("Checksum = 0xcbf43926" for standard CRC-32).
Console.WriteLine("Checksum = 0x{0:x}", checksum);
```

## Recommended CRC-32 Algorithm

Among all other posibilities, it is recommended to use the standard CRC-32 algorithm which comes under CRC-32, CRC-32/ISO-HDLC, CRC-32/ADCCP, CRC-32/V-42, CRC-32/XZ, PKZIP aliases and is available via `Crc32.Standard` property.

All other predefined algorithms are available via the corresponding properties of `Crc32.Attested` class.

## Custom CRC-32 Algorithms

Once in a while, you may encounter a custom CRC-32 algorithm that is neither widely known nor characterized.
In that case, you can instantiate a custom checksum algorithm with the desired parameters by hand:

``` c#
var checksumAlgorithm = new CustomCrc32(poly, init, refin, refout, xorout);
```

If you want to formalize a custom algorithm even further, you may opt-in to creating a separate class for it with a convenient accessor property:

``` c#
/// <summary>
/// Defines a custom CRC-32 algorithm.
/// </summary>
sealed class FooCrc32 : CustomCrc32
{
    FooCrc32() :
        base(poly, init, refin, refout, xorout)
    {
    }

    public static FooCrc32 Instance { get; } = new FooCrc32();
}
```

That will allow you to use the algorithm effortlessly from several places in the codebase later:

``` c#
var checksum = FooCrc32.Instance.ComputeChecksum(...);
```

## Usage

`Gapotchenko.FX.Data.Integrity.Checksum.Crc32` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum.Crc32):

```
PM> Install-Package Gapotchenko.FX.Data.Integrity.Checksum.Crc32
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../../../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../../../../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../../../../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../../../../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data](../../../Encoding/Gapotchenko.FX.Data.Encoding)
  - [Gapotchenko.FX.Data.Encoding](../../../Encoding/Gapotchenko.FX.Data.Encoding)
  - [Gapotchenko.FX.Data.Integrity.Checksum](../Gapotchenko.FX.Data.Integrity.Checksum)
    - [Gapotchenko.FX.Data.Integrity.Checksum.Crc8](../Gapotchenko.FX.Data.Integrity.Checksum.Crc8)
    - [Gapotchenko.FX.Data.Integrity.Checksum.Crc16](../Gapotchenko.FX.Data.Integrity.Checksum.Crc16)
    - &#x27B4; [Gapotchenko.FX.Data.Integrity.Checksum.Crc32](../Gapotchenko.FX.Data.Integrity.Checksum.Crc32)
- [Gapotchenko.FX.Diagnostics](../../../../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../../../../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../../../../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../../../../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../../../../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Security.Cryptography](../../../../Security/Cryptography/Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../../../../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../../../../Gapotchenko.FX.Threading)

Or look at the [full list of modules](../../../..#available-modules).
