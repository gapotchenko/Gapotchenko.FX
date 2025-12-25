# Gapotchenko.FX.Data.Integrity.Checksum.Crc8
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Integrity.Checksum.Crc8.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum.Crc8)

The module provides the implementation of checksum algorithms belonging to the CRC-8 family.

## Quick Start

Use the following code to calculate a CRC-8 checksum for the specified data, be it `System.Byte[]`, `System.ReadOnlySpan<Byte>` or `System.IO.Stream`:

``` C#
using Gapotchenko.FX.Data.Integrity.Checksum;

var checksum = Crc8.Standard.ComputeChecksum(data);
```

If you need to calculate a CRC-8 checksum iteratively then the following approach becomes handy:

``` C#
var iterator = Crc8.Standard.CreateIterator();

iterator.ComputeBlock(...); // block 1
// ...
iterator.ComputeBlock(...); // block N

// Compute the final checksum:
var checksum = iterator.ComputeFinal();
```

## Available CRC-8 Algorithms

CRC-8 family of cyclic redundancy checks consists of several attested checksum algorithms with predefined parameters:

| Algorithm | Aliases | Gapotchenko.FX Implementation | Parameters: poly | init | refin | refout | xorout | check |
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

``` C#
// Get the byte representation of the ASCII string.
var data = Encoding.ASCII.GetBytes("123456789");

// Compute the checksum.
var checksum = Crc8.Standard.ComputeChecksum(data);

// Print out the result ("Checksum=0xf4" for standard CRC-8).
Console.WriteLine("Checksum=0x{0:x}", checksum);
```

## Recommended CRC-8 Algorithm

Among all other possibilities, it is recommended to use the standard CRC-8 algorithm which comes under CRC-8, CRC-8/SMBUS aliases and is available via `Crc8.Standard` property.

All other predefined algorithms are available via the corresponding properties of `Crc8.Attested` class.

## Custom CRC-8 Algorithms

Once in a while, you may encounter a custom CRC-8 algorithm that is neither widely known nor characterized.
In that case, you can instantiate a custom checksum algorithm with the desired parameters by hand:

``` C#
var checksumAlgorithm = new CustomCrc8(poly, init, refin, refout, xorout);
```

If you want to formalize the custom algorithm even further, you may opt in to create a separate class for it with a convenient accessor property:

``` C#
/// <summary>
/// Defines a custom CRC-8 algorithm.
/// </summary>
sealed class FooCrc8 : CustomCrc8
{
    FooCrc8() :
        base(poly, init, refin, refout, xorout)
    {
    }

    public static FooCrc8 Instance { get; } = new();
}
```

That will allow you to use the algorithm effortlessly from several places in the codebase later:

``` C#
var checksum = FooCrc8.Instance.ComputeChecksum(...);
```

## Usage

`Gapotchenko.FX.Data.Integrity.Checksum.Crc8` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum.Crc8):

```
dotnet package add Gapotchenko.FX.Data.Integrity.Checksum.Crc8
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../../Archives/Gapotchenko.FX.Data.Archives#readme)
  - [Gapotchenko.FX.Data.Archives](../../../Archives/Gapotchenko.FX.Data.Archives#readme)
  - [Gapotchenko.FX.Data.Encoding](../../../Encoding/Gapotchenko.FX.Data.Encoding#readme)
  - [Gapotchenko.FX.Data.Integrity.Checksum](../Gapotchenko.FX.Data.Integrity.Checksum#readme)
    - &#x27B4; [Gapotchenko.FX.Data.Integrity.Checksum.Crc8](.#readme)
    - [Gapotchenko.FX.Data.Integrity.Checksum.Crc16](../Gapotchenko.FX.Data.Integrity.Checksum.Crc16#readme)
    - [Gapotchenko.FX.Data.Integrity.Checksum.Crc32](../Gapotchenko.FX.Data.Integrity.Checksum.Crc32#readme)
- [Gapotchenko.FX.Diagnostics](../../../../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../../../../IO/Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../../../../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../../../../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../../../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../../../Gapotchenko.FX.Tuples#readme)
- [Gapotchenko.FX.Versioning](../../../../Versioning/Gapotchenko.FX.Versioning#readme)

Or look at the [full list of modules](../../../../..#readme).
