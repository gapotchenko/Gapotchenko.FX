# Gapotchenko.FX.Data.Integrity.Checksum
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Integrity.Checksum.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum)

The module provides a quasi-universal framework for cyclic redundancy checks.
It supports both synchronous and asynchronous data processing, including the iterative checksum computation.

## IChecksumAlgorithm&lt;T&gt;

This is the root interface provided by a checksum algorithm.
The generic parameter `T` defines the data type of a checksum value.
For example, `System.UInt32` would be a type `T` for a CRC-32 algorithm.

Checksum algorithm interface provides several notable methods.

### ComputeChecksum(ReadOnlySpan&lt;byte&gt; data)

The method allows to compute a checksum value for the specified byte span.
This is the most widely used operation.
Example:

```c#
var data = new byte[] { ... };
var checksum = checksumAlgorithm.ComputeChecksum(data);
Console.WriteLine("The array checksum is {0}.", checksum);
```

### ComputeChecksum(Stream stream)

The method computes the checksum for the specified `System.IO.Stream` object.
This operation is useful for computing a checksum for a file or any other data stream in one go.
Example:

```c#
using var file = File.OpenRead(...);
var checksum = checksumAlgorithm.ComputeChecksum(file);
Console.WriteLine("The file checksum is {0}.", checksum);
```

### CreateIterator()

Sometimes the checksum computation cannot be performed in one go, and should be performed in chunks instead.
That's why every checksum algorithm provides `CreateIterator()` method.

Once the iterator is created, it can be used for iterative checksum computation without data concatenation overhead:

```csharp
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

`Gapotchenko.FX.Data.Integrity.Checksum` module provides only a framework for checksum algorithms.
If you want to use a popular and ready-to-use checksum algorithm, Gapotchenko.FX provides quite a few out of the box:

| Algorithm Family    | Module                                                                                                 | Algorithms |
| ------------------- | ------------------------------------------------------------------------------------------------------ | ---------- |
| CRC-8               | [Gapotchenko.FX.Data.Integrity.Checksum.Crc8](../Gapotchenko.FX.Data.Integrity.Checksum.Crc8#readme)   | CRC-8/SMBUS, CRC-8/TECH-3250, CRC-8/SAE-J1850, CRC-8/OPENSAFETY, CRC-8/NRSC-5, CRC-8/MIFARE-MAD, CRC-8/MAXIM, CRC-8/I-CODE, CRC-8/HITAG, CRC-8/DARC, CRC-8/BLUETOOTH, CRC-8/AUTOSAR 
| CRC-16              | [Gapotchenko.FX.Data.Integrity.Checksum.Crc16](../Gapotchenko.FX.Data.Integrity.Checksum.Crc16#readme) | CRC-16/CCITT, CRC-16/ISO-IEC-14443-3-A, CRC-16/ISO-IEC-14443-3-B, CRC-16/NRSC-5, CRC-16/MAXIM, CRC-16/SPI-FUJITSU, CRC-16/UMTS, CRC-16/USB, CRC-16/XMODEM, CRC-16/PROFIBUS, CRC-16/MODBUS, CRC-16/GENIBUS, CRC-16/GSM, CRC-16/OPENSAFETY-A, CRC-16/OPENSAFETY-B, CRC-16/TMS37157, CRC-16/MCRF4XX, CRC-16/DECT-R, CRC-16/DECT-X, CRC-16/DDS-110, CRC-16/CCITT-FALSE
| CRC-32              | [Gapotchenko.FX.Data.Integrity.Checksum.Crc32](../Gapotchenko.FX.Data.Integrity.Checksum.Crc32#readme) | CRC-32/ISO-HDLC, CRC-32C, CRC-32Q, CRC-32/AUTOSAR, CRC-32/POSIX, CRC-32/BZIP2, CRC-32/MEF, CRC-32/MPEG-2

Moreover, you can create your own checksum algorithm.
Gapotchenko.FX project welcomes contributions, or it can be a standalone NuGet package that uses `Gapotchenko.FX.Data.Integrity.Checksum` module as a wireframe.

## Usage

`Gapotchenko.FX.Data.Integrity.Checksum` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum):

```
PM> Install-Package Gapotchenko.FX.Data.Integrity.Checksum
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../../../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../../../../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../../../../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../../../../Gapotchenko.FX.Console)
- &#x27B4; [Gapotchenko.FX.Data.Integrity.Checksum](../Gapotchenko.FX.Data.Integrity.Checksum)
  - [Gapotchenko.FX.Data.Integrity.Checksum.Crc8](../Gapotchenko.FX.Data.Integrity.Checksum.Crc8)
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
