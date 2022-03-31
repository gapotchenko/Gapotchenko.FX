# Gapotchenko.FX.Data.Integrity.Checksum
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Integrity.Checksum.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Integrity.Checksum)

The module provides a quasi-universal framework for cyclic redundancy checks.
It supports both synchronous and asynchronous data processing, including the iterative checksum computation.

## IChecksumAlgorithm&lt;T&gt;

This is the root interface provided by a checksum algorithm.
The generic parameter `T` defines the data type of a checksum value.
For example, `System.UInt32` would be a type `T` for a CRC-32 algorithm.

The interface provides several notable methods.

### ComputeChecksum(ReadOnlySpan&lt;byte&gt; data)

The method allows to compute the checksum value for the specified byte span.
This is the most widely used operation.

```c#
var data = new byte[] { ... };
var checksum = checksumAlgorithm.ComputeChecksum(data);
Console.WriteLine("The array checksum is {0}.", checksum);
```

### ComputeChecksum(Stream stream)

The method computes the checksum for the specified `System.IO.Stream` object.
This operation is useful for computing the checksum for a file or any other data stream in one go.

```c#
using var file = File.OpenRead(...);
var checksum = checksumAlgorithm.ComputeChecksum(file);
Console.WriteLine("The file checksum is {0}.", checksum);
```

### CreateIterator()

Sometimes the checksum computation should be performed in chunks.
That's why every checksum algorithm provides `CreateIterator()` method.

Once the iterator is created, it can be used to compute the checksum iteratively without data concatenation overhead:

```csharp
var iterator = checksumAlgorithm.CreateIterator();

iterator.ComputeBlock(...);
// ...
iterator.ComputeBlock(...);
// ...

// Compute the final checksum value:
var checksum = iterator.ComputeFinal();

Console.WriteLine("The checksum is {0}.", checksum);
```

## Checksum Algorithms

`Gapotchenko.FX.Data.Integrity.Checksum` module provides only a framework for checksum algorithms.
If you want to use a popular checksum algorithm, Gapotchenko.FX provides quite a few out of the box:

| Family    | Module                                                                                                 | Algorithms |
| --------- | ------------------------------------------------------------------------------------------------------ | ---------- |
| CRC-8     | [Gapotchenko.FX.Data.Integrity.Checksum.Crc8](..\Gapotchenko.FX.Data.Integrity.Checksum.Crc8#readme)   | CRC-8, CRC-8/TECH-3250, CRC-8/SAE-J1850, CRC-8/OPENSAFETY, CRC-8/NRSC-5, CRC-8/MIFARE-MAD, CRC-8/MAXIM, CRC-8/I-CODE, CRC-8/HITAG, CRC-8/DARC, CRC-8/BLUETOOTH, CRC-8/AUTOSAR 
| CRC-16    | [Gapotchenko.FX.Data.Integrity.Checksum.Crc16](..\Gapotchenko.FX.Data.Integrity.Checksum.Crc16#readme) |
| CRC-32    | [Gapotchenko.FX.Data.Integrity.Checksum.Crc32](..\Gapotchenko.FX.Data.Integrity.Checksum.Crc32#readme) |

Moreover, you can create your own checksum algorithm.
Gapotchenko.FX project welcomes contributions, or it can be a standalone NuGet package that uses `Gapotchenko.FX.Data.Integrity.Checksum` module as a wireframe.
