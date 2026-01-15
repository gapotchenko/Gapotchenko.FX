# Gapotchenko.FX.Data.Integrity.Checksum
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Integrity.Checksum.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum)

The module provides a quasi-universal framework for cyclic redundancy checking algorithms.
It supports both synchronous and asynchronous data processing including iterative checksum computation.

## `IChecksumAlgorithm<T>`

Defines the root interface of a checksum algorithm.
The generic parameter `T` specifies the data type of a checksum value.
For example, `System.UInt32` would be the type `T` for a CRC-32 algorithm.

The checksum algorithm interface provides several notable methods presented below.

### `ComputeChecksum(ReadOnlySpan<byte> data)`

The method allows to compute a checksum value for the specified byte span.
This is the most widely used operation.
Example:

``` C#
var data = new byte[] { ... };
var checksum = checksumAlgorithm.ComputeChecksum(data);
Console.WriteLine("The array checksum is {0}.", checksum);
```

### `ComputeChecksum(Stream stream)`

The method computes the checksum for the specified `System.IO.Stream` object.
This operation is useful for computing a checksum for a file or any other data stream in one go.
Example:

``` C#
using var file = File.OpenRead(...);
var checksum = checksumAlgorithm.ComputeChecksum(file);
Console.WriteLine("The file checksum is {0}.", checksum);
```

### `CreateIterator()`

Sometimes the checksum computation cannot be performed in one go, and should be performed in chunks.
That's why every checksum algorithm provides `CreateIterator()` method.

Once the iterator is created, it can be used for iterative checksum computation without data concatenation overhead:

``` C#
var iterator = checksumAlgorithm.CreateIterator();

iterator.ComputeBlock(...); // block 1
// ...
iterator.ComputeBlock(...); // block N
// ...

// Compute the final checksum value:
var checksum = iterator.ComputeFinal();

Console.WriteLine("The checksum is {0}.", checksum);
```

## Checksum Algorithms

`Gapotchenko.FX.Data.Integrity.Checksum` module provides only the framework for checksum algorithm implementations.
If you want to use a ready-to-use checksum algorithm, Gapotchenko.FX provides quite a few out of the box:

| Algorithm Family    | Module                                                                                                 | Provided Algorithms |
| ------------------- | ------------------------------------------------------------------------------------------------------ | ------------------- |
| **CRC-8**               | [Gapotchenko.FX.Data.Integrity.Checksum.Crc8](../Gapotchenko.FX.Data.Integrity.Checksum.Crc8#readme)   | CRC-8/SMBUS, CRC-8/TECH-3250, CRC-8/SAE-J1850, CRC-8/OPENSAFETY, CRC-8/NRSC-5, CRC-8/MIFARE-MAD, CRC-8/MAXIM, CRC-8/I-CODE, CRC-8/HITAG, CRC-8/DARC, CRC-8/BLUETOOTH, CRC-8/AUTOSAR 
| **CRC-16**              | [Gapotchenko.FX.Data.Integrity.Checksum.Crc16](../Gapotchenko.FX.Data.Integrity.Checksum.Crc16#readme) | CRC-16/CCITT, CRC-16/ISO-IEC-14443-3-A, CRC-16/ISO-IEC-14443-3-B, CRC-16/NRSC-5, CRC-16/MAXIM, CRC-16/SPI-FUJITSU, CRC-16/UMTS, CRC-16/USB, CRC-16/XMODEM, CRC-16/PROFIBUS, CRC-16/MODBUS, CRC-16/GENIBUS, CRC-16/GSM, CRC-16/OPENSAFETY-A, CRC-16/OPENSAFETY-B, CRC-16/TMS37157, CRC-16/MCRF4XX, CRC-16/DECT-R, CRC-16/DECT-X, CRC-16/DDS-110, CRC-16/CCITT-FALSE
| **CRC-32**              | [Gapotchenko.FX.Data.Integrity.Checksum.Crc32](../Gapotchenko.FX.Data.Integrity.Checksum.Crc32#readme) | CRC-32/ISO-HDLC, CRC-32C, CRC-32Q, CRC-32/AUTOSAR, CRC-32/POSIX, CRC-32/BZIP2, CRC-32/MEF, CRC-32/MPEG-2

Moreover, you can create your own checksum algorithm implementations.
Gapotchenko.FX project welcomes contributions, or it can be a standalone NuGet package that uses `Gapotchenko.FX.Data.Integrity.Checksum` module as a wireframe.

## Usage

`Gapotchenko.FX.Data.Integrity.Checksum` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum):

```
dotnet package add Gapotchenko.FX.Data.Integrity.Checksum
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
  - &#x27B4; [Gapotchenko.FX.Data.Integrity.Checksum](.#readme)
    - [Gapotchenko.FX.Data.Integrity.Checksum.Crc8](../Gapotchenko.FX.Data.Integrity.Checksum.Crc8#readme)
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
