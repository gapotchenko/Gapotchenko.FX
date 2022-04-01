# Gapotchenko.FX.Data.Integrity.Checksum.Crc16
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Integrity.Checksum.Crc16.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum.Crc16)

The module provides the implementation of checksum algorithms belonging to CRC-16 family.

## Quick Start

Use the following code to calculate a CRC-16 checksum for the specified data:

``` c#
using Gapotchenko.FX.Data.Integrity.Checksum;

var checksum = Crc16.Standard.ComputeChecksum(data);
```

If you need to calculate a CRC-16 checksum iteratively then the following approach becomes handy:

```csharp
var iterator = Crc16.Standard.CreateIterator();

iterator.ComputeBlock(...); // block 1
// ...
iterator.ComputeBlock(...); // block N

// Compute the final checksum:
var checksum = iterator.ComputeFinal();
```

## Available CRC-16 Algorithms

CRC-16 family of cyclic redundancy checks consists of several attested checksum algorithms with predefined parameters:

| Algorithm | Aliases | Implementation | Parameters: poly | init | refin | refout | xorout | check |
| --------- | ------- | -------- | ---- | ---- | ----- | ------ | ------ | ----- |
| CRC-16 (standard, recommended) | CRC-16/ARC, CRC-IBM, CRC-16/LHA | `Crc16.Standard` | 0x8005 | 0x0000 | true | true | 0x0000 | 0xbb3d |
| CRC-16/CCITT | CRC-16/KERMIT, CRC-16/CCITT-TRUE, CRC-16/V-41-LSB, CRC-CCITT, KERMIT | `Crc16.Attested.Ccitt` | 0x1021 | 0x0000 | true | true | 0x0000 | 0x2189 |
| CRC-16/ISO-IEC-14443-3-A | CRC-A | `Crc16.Attested.IsoIec14443_3_A` | 0x1021 | 0xc6c6 | true | true | 0x0000 | 0xbf05 |
| CRC-16/ISO-IEC-14443-3-B | CRC-B, CRC-16/IBM-SDLC, CRC-16/ISO-HDLC, CRC-16/X-25, X-25 | `Crc16.Attested.IsoIec14443_3_B` | 0x1021 | 0xffff | true | true | 0xffff | 0x906e |

The `check` parameter shows what checksum value an algorithm should produce for `"123456789"` input string interpreted as an ASCII data:

``` c#
// Get the byte representation of the ASCII string:
var data = Encoding.ASCII.GetBytes("123456789");

// Compute checksum:
var checksum = Crc16.Standard.ComputeChecksum(data);

// Print out the result (will print "Checksum = 0xbb3d"):
Console.WriteLine("Checksum = 0x{0:x}", checksum);
```

## Recommended CRC-16 Algorithm

Among all other posibilities, it is recommended to use the standard CRC-16 algorithm which comes under CRC-16, CRC-16/ARC, CRC-IBM, CRC-16/LHA aliases and is available via `Crc16.Standard` property.

All other predefined algorithms are available as the properties of `Crc16.Attested` class.

## Custom CRC-16 Algorithms

Once in a while you may encounter a custom CRC-16 algorithm that is neither widely known nor characterized.
In that case, you can instantiate a custom checksum algorithm with the desired parameters:

``` c#
var checksumAlgorithm = new CustomCrc16(...);
```

If you want to formalize a custom algorithm even further, you may opt-in to creating a separate class for it with a convenient accessor property:

``` c#
/// <summary>
/// Defines a custom CRC-16 algorithm.
/// </summary>
sealed class FooCrc : CustomCrc16
{
    FooCrc() :
        base(...) // <- custom algorithm parameters go here
    {
    }

    public static FooCrc Instance { get; } = new FooCrc();
}
```

That would allow to use the algorithm effortlessly from several places in the codebase:

``` c#
var checksum = FooCrc.Instance.ComputeChecksum(...);
```

## Usage

`Gapotchenko.FX.Data.Integrity.Checksum.Crc16` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum.Crc16):

```
PM> Install-Package Gapotchenko.FX.Data.Integrity.Checksum.Crc16
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
  - [Gapotchenko.FX.Diagnostics.CommandLine](../Gapotchenko.FX.Diagnostics.CommandLine)
  - &#x27B4; [Gapotchenko.FX.Diagnostics.Process](../Gapotchenko.FX.Diagnostics.Process)
  - [Gapotchenko.FX.Diagnostics.WebBrowser](../Gapotchenko.FX.Diagnostics.WebBrowser)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)

Or look at the [full list of modules](../../../..#available-modules).
