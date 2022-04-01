# Gapotchenko.FX.Data.Integrity.Checksum.Crc8
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Integrity.Checksum.Crc8.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum.Crc8)

The module provides the implementation of checksum algorithms belonging to CRC-8 family.

## Example

Use the following code to calculate a CRC-8 checksum for the specified data:

``` c#
using Gapotchenko.FX.Data.Integrity.Checksum;

var checksum = Crc8.Standard.ComputeChecksum(data);
```

If you need to calculate a CRC-8 checksum iteratively then the following code becomes handy:

```csharp
var iterator = Crc8.Standard.CreateIterator();

iterator.ComputeBlock(...); // block 1
// ...
iterator.ComputeBlock(...); // block N
// ...

// Compute the final checksum:
var checksum = iterator.ComputeFinal();
```

## Available Algorithms

CRC-8 family of cyclic redundancy checks consists of several attested checksum algorithms:

| Algorithm | Aliases | Implementation | Parameters: poly | init | refin | refout | xorout | check |
| --------- | ------- | -------- | ---- | ---- | ----- | ------ | ------ | ----- |
| CRC-8 (standard) | CRC-8/SMBUS | `Crc8.Standard` | 0x07 | 0x00 | false | false | 0x00 | 0xf4 |
| CRC-8/TECH-3250 | CRC-8/AES, CRC-8/EBU | `Crc8.Attested.Tech3250` | 0x1d | 0xff | true | true | 0x00 | 0x97 |


## Usage

`Gapotchenko.FX.Data.Integrity.Checksum.Crc8` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum.Crc8):

```
PM> Install-Package Gapotchenko.FX.Data.Integrity.Checksum.Crc8
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
