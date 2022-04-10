# Gapotchenko.FX.Data.Integrity.Checksum.Crc8
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Integrity.Checksum.Crc8.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum.Crc8)

The module provides the implementation of checksum algorithms belonging to CRC-8 family.

## Quick Start

Use the following code to calculate a CRC-8 checksum for the specified data, be it `System.Byte[]`, `System.ReadOnlySpan<Byte>` or `System.IO.Stream`:

``` c#
using Gapotchenko.FX.Data.Integrity.Checksum;

var checksum = Crc8.Standard.ComputeChecksum(data);
```

If you need to calculate a CRC-8 checksum iteratively then the following approach becomes handy:

```csharp
var iterator = Crc8.Standard.CreateIterator();

iterator.ComputeBlock(...); // block 1
// ...
iterator.ComputeBlock(...); // block N

// Compute the final checksum:
var checksum = iterator.ComputeFinal();
```

## Available CRC-8 Algorithms

CRC-8 family of cyclic redundancy checks consists of several attested checksum algorithms with predefined parameters:

| Algorithm | Aliases | Implementation | Parameters: poly | init | refin | refout | xorout | check |
| --------- | ------- | -------- | ---- | ---- | ----- | ------ | ------ | ----- |
| CRC-8 (standard, recommended) | CRC-8/SMBUS | `Crc8.Standard` | 0x07 | 0x00 | false | false | 0x00 | 0xf4 |
| CRC-8/TECH-3250 | CRC-8/AES, CRC-8/EBU | `Crc8.Attested.Tech3250` | 0x1d | 0xff | true | true | 0x00 | 0x97 |
| CRC-8/SAE-J1850 | | `Crc8.Attested.SaeJ1850` | 0x1d | 0xff | false | false | 0xff | 0x4b |
| CRC-8/OPENSAFETY | | `Crc8.Attested.OpenSafety` | 0x2f | 0x00 | false | false | 0x00 | 0x3e |
| CRC-8/NRSC-5 | | `Crc8.Attested.Nrsc5` | 0x31 | 0xff | false | false | 0x00 | 0xf7 |
| CRC-8/MIFARE-MAD | | `Crc8.Attested.MifareMad` | 0x1d | 0xc7 | false | false | 0x00 | 0x99 |
| CRC-8/MAXIM | CRC-8/MAXIM-DOW, DOW-CRC | `Crc8.Attested.Maxim` | 0x31 | 0x00 | true | true | 0x00 | 0xa1 |
| CRC-8/I-CODE | | `Crc8.Attested.ICode` | 0x1d | 0xfd | false | false | 0x00 | 0x7e |
| CRC-8/HITAG | | `Crc8.Attested.Hitag` | 0x1d | 0xff | false | false | 0x00 | 0xb4 |
| CRC-8/DARC | | `Crc8.Attested.Darc` | 0x39 | 0x00 | true | true | 0x00 | 0x15 |
| CRC-8/BLUETOOTH | | `Crc8.Attested.Bluetooth` | 0xa7 | 0x00 | true | true | 0x00 | 0x26 |
| CRC-8/AUTOSAR | | `Crc8.Attested.Autosar` | 0x2f | 0xff | false | false | 0xff | 0xdf |

The `check` parameter shows what checksum value an algorithm should produce for `"123456789"` input string interpreted as an ASCII data:

``` c#
// Get the byte representation of the ASCII string.
var data = Encoding.ASCII.GetBytes("123456789");

// Compute the checksum.
var checksum = Crc8.Standard.ComputeChecksum(data);

// Print out the result (will print "Checksum = 0xf4").
Console.WriteLine("Checksum = 0x{0:x}", checksum);
```

## Recommended CRC-8 Algorithm

Among all other posibilities, it is recommended to use the standard CRC-8 algorithm which comes under CRC-8, CRC-8/SMBUS aliases and is available via `Crc8.Standard` property.

All other predefined algorithms are available via the properties of `Crc8.Attested` class.

## Custom CRC-8 Algorithms

Once in a while, you may encounter a custom CRC-8 algorithm that is neither widely known nor characterized.
In that case, you can instantiate a custom checksum algorithm with the desired parameters by hand:

``` c#
var checksumAlgorithm = new CustomCrc8(poly, init, refin, refout, xorout);
```

If you want to formalize a custom algorithm even further, you may opt-in to creating a separate class for it with a convenient accessor property:

``` c#
/// <summary>
/// Defines a custom CRC-8 algorithm.
/// </summary>
sealed class FooCrc8 : CustomCrc8
{
    FooCrc8() :
        base(poly, init, refin, refout, xorout)
    {
    }

    public static FooCrc8 Instance { get; } = new FooCrc8();
}
```

That will allow you to use the algorithm effortlessly from several places in the codebase later:

``` c#
var checksum = FooCrc8.Instance.ComputeChecksum(...);
```

## Usage

`Gapotchenko.FX.Data.Integrity.Checksum.Crc8` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum.Crc8):

```
PM> Install-Package Gapotchenko.FX.Data.Integrity.Checksum.Crc8
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../../../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../../../../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../../../../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../../../../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data.Integrity.Checksum](../Gapotchenko.FX.Data.Integrity.Checksum)
  - &#x27B4; [Gapotchenko.FX.Data.Integrity.Checksum.Crc8](../Gapotchenko.FX.Data.Integrity.Checksum.Crc8)
  - [Gapotchenko.FX.Data.Integrity.Checksum.Crc16](../Gapotchenko.FX.Data.Integrity.Checksum.Crc16)
  - [Gapotchenko.FX.Data.Integrity.Checksum.Crc32](../Gapotchenko.FX.Data.Integrity.Checksum.Crc32)
- [Gapotchenko.FX.Diagnostics](../../../../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../../../../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../../../../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../../../../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../../../../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Security.Cryptography](../../../../Security/Cryptography/Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../../../../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../../../../Gapotchenko.FX.Threading)

Or look at the [full list of modules](../../../..#available-modules).
