# Gapotchenko.FX.Data.Integrity.Checksum.Crc16
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Integrity.Checksum.Crc16.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum.Crc16)

The module provides the implementation of checksum algorithms belonging to the CRC-16 family.

## Quick Start

Use the following code to calculate a CRC-16 checksum for the specified data, be it `System.Byte[]`, `System.ReadOnlySpan<Byte>` or `System.IO.Stream`:

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

| Algorithm | Aliases | Gapotchenko.FX Implementation | Parameters: poly | init | refin | refout | xorout | check |
| --------- | ------- | -------- | ---- | ---- | ----- | ------ | ------ | ----- |
| CRC-16 (standard, recommended) | CRC-16/ARC, CRC-IBM, CRC-16/LHA | `Crc16.Standard` | 0x8005 | 0x0000 | true | true | 0x0000 | 0xbb3d |
| CRC-16/CCITT | CRC-16/KERMIT, CRC-16/CCITT-TRUE, CRC-16/V-41-LSB, CRC-CCITT, KERMIT | `Crc16.Attested.Ccitt` | 0x1021 | 0x0000 | true | true | 0x0000 | 0x2189 |
| CRC-16/ISO-IEC-14443-3-A | CRC-A | `Crc16.Attested.IsoIec14443_3_A` | 0x1021 | 0xc6c6 | true | true | 0x0000 | 0xbf05 |
| CRC-16/ISO-IEC-14443-3-B | CRC-B, CRC-16/IBM-SDLC, CRC-16/ISO-HDLC, CRC-16/X-25, X-25 | `Crc16.Attested.IsoIec14443_3_B` | 0x1021 | 0xffff | true | true | 0xffff | 0x906e |
| CRC-16/NRSC-5 | | `Crc16.Attested.Nrsc5` | 0x080b | 0xffff | true | true | 0x0000 | 0xa066 |
| CRC-16/MAXIM | CRC-16/MAXIM-DOW | `Crc16.Attested.Maxim` | 0x8005 | 0x0000 | true | true | 0xffff | 0x44c2 |
| CRC-16/SPI-FUJITSU | CRC-16/AUG-CCITT | `Crc16.Attested.SpiFujitsu` | 0x1021 | 0x1d0f | false | false | 0x0000 | 0xe5cc |
| CRC-16/UMTS | CRC-16/VERIFONE, CRC-16/BUYPASS | `Crc16.Attested.Umts` | 0x8005 | 0x0000 | false | false | 0x0000 | 0xfee8 |
| CRC-16/USB | | `Crc16.Attested.Usb` | 0x8005 | 0xffff | true | true | 0xffff | 0xb4c8 |
| CRC-16/XMODEM | CRC-16/ACORN, CRC-16/LTE, CRC-16/V-41-MSB, XMODEM, ZMODEM | `Crc16.Attested.XModem` | 0x1021 | 0x0000 | false | false | 0x0000 | 0x31c3 |
| CRC-16/PROFIBUS | CRC-16/IEC-61158-2 | `Crc16.Attested.Profibus` | 0x1dcf | 0xffff | false | false | 0xffff | 0xa819 |
| CRC-16/MODBUS | MODBUS | `Crc16.Attested.Modbus` | 0x8005 | 0xffff | true | true | 0x0000 | 0x4b37 |
| CRC-16/GENIBUS | CRC-16/DARC, CRC-16/EPC, CRC-16/EPC-C1G2, CRC-16/I-CODE | `Crc16.Attested.Genibus` | 0x1021 | 0xffff | false | false | 0xffff | 0xd64e |
| CRC-16/GSM | | `Crc16.Attested.Gsm` | 0x1021 | 0x0000 | false | false | 0xffff | 0xce3c |
| CRC-16/OPENSAFETY-A | | `Crc16.Attested.OpenSafetyA` | 0x5935 | 0x0000 | false | false | 0x0000 | 0x5d38 |
| CRC-16/OPENSAFETY-B | | `Crc16.Attested.OpenSafetyB` | 0x755b | 0x0000 | false | false | 0x0000 | 0x20fe |
| CRC-16/TMS37157 | | `Crc16.Attested.TMS37157` | 0x1021 | 0x89ec | true | true | 0x0000 | 0x26b1 |
| CRC-16/MCRF4XX | | `Crc16.Attested.MCRF4xx` | 0x1021 | 0xffff | true | true | 0x0000 | 0x6f91 |
| CRC-16/DECT-R | R-CRC-16 | `Crc16.Attested.DectR` | 0x0589 | 0x0000 | false | false | 0x0001 | 0x007e |
| CRC-16/DECT-X | X-CRC-16 | `Crc16.Attested.DectX` | 0x0589 | 0x0000 | false | false | 0x0000 | 0x007f |
| CRC-16/DDS-110 | | `Crc16.Attested.Dds110` | 0x8005 | 0x800d | false | false | 0x0000 | 0x9ecf |
| CRC-16/CCITT-FALSE | CRC-16/AUTOSAR, CRC-16/IBM-3740 | `Crc16.Attested.CcittFalse` | 0x1021 | 0xffff | false | false | 0x0000 | 0x29b1 |

The `check` parameter shows what checksum value an algorithm should produce for `"123456789"` input string interpreted as an ASCII data:

``` c#
// Get the byte representation of the ASCII string.
var data = Encoding.ASCII.GetBytes("123456789");

// Compute the checksum.
var checksum = Crc16.Standard.ComputeChecksum(data);

// Print out the result ("Checksum=0xbb3d" for standard CRC-16).
Console.WriteLine("Checksum=0x{0:x}", checksum);
```

## Recommended CRC-16 Algorithm

Among all other possibilities, it is recommended to use the standard CRC-16 algorithm which comes under CRC-16, CRC-16/ARC, CRC-IBM, CRC-16/LHA aliases and is available via `Crc16.Standard` property.

All other predefined algorithms are available via the corresponding properties of `Crc16.Attested` class.

## Custom CRC-16 Algorithms

Once in a while, you may encounter a custom CRC-16 algorithm that is neither widely known nor characterized.
In that case, you can instantiate a custom checksum algorithm with the desired parameters by hand:

``` c#
var checksumAlgorithm = new CustomCrc16(poly, init, refin, refout, xorout);
```

If you want to formalize a custom algorithm even further, you may opt-in to creating a separate class for it with a convenient accessor property:

``` c#
/// <summary>
/// Defines a custom CRC-16 algorithm.
/// </summary>
sealed class FooCrc16 : CustomCrc16
{
    FooCrc16() :
        base(poly, init, refin, refout, xorout)
    {
    }

    public static FooCrc16 Instance { get; } = new FooCrc16();
}
```

That will allow you to use the algorithm effortlessly from several places in the codebase later:

``` c#
var checksum = FooCrc16.Instance.ComputeChecksum(...);
```

## Usage

`Gapotchenko.FX.Data.Integrity.Checksum.Crc16` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum.Crc16):

```
PM> Install-Package Gapotchenko.FX.Data.Integrity.Checksum.Crc16
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
    - &#x27B4; [Gapotchenko.FX.Data.Integrity.Checksum.Crc16](../Gapotchenko.FX.Data.Integrity.Checksum.Crc16)
    - [Gapotchenko.FX.Data.Integrity.Checksum.Crc32](../Gapotchenko.FX.Data.Integrity.Checksum.Crc32)
- [Gapotchenko.FX.Diagnostics](../../../../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../../../../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../../../../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../../../../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../../../../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Security.Cryptography](../../../../Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../../../../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../../../../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.ValueTuple](../../../../Gapotchenko.FX.ValueTuple)

Or look at the [full list of modules](../../../..#available-modules).
