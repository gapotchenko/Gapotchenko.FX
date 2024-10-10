# Gapotchenko.FX.Data.Encoding
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Encoding.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Encoding)

`Gapotchenko.FX.Data.Encoding` module provides a quasi-universal framework for data encodings.
It supports both synchronous and asynchronous data processing models, including the iterative transcoding.

The module serves as the implementation basis for a variety of popular encodings: [Base16](../Gapotchenko.FX.Data.Encoding.Base16#readme), [Base32](../Gapotchenko.FX.Data.Encoding.Base32#readme), [Base64](../Gapotchenko.FX.Data.Encoding.Base64#readme), and others.

## ITextDataEncoding

This is the root interface provided by a binary-to-text data encoding algorithm.
The interface has several notable methods.

### GetString(ReadOnlySpan&lt;byte&gt; data)

This method encodes all the bytes in the specified span into a string.

For example, `GetString` method of a Base16 encoding would be used like this:

``` C#
using Gapotchenko.FX.Data.Encoding;

var data = new byte[] { 1, 2, 3, 10 };
var s = Base16.GetString(data);
Console.WriteLine(s);
```

producing the following output:

```
0102030A
```

The `GetString` method can also take options.
Here is an example that would produce the indented output:

``` C#
var data = new byte[] { 1, 2, 3, 10 };
var s = Base16.GetString(data, DataEncodingOptions.Indent);
Console.WriteLine(s);
```

The output:

```
01 02 03 0A
```

Note that the output now contains space separators (indentations) between the encoded data values.
Not all encodings support indentation so `Indent` option flag may be ignored by some of them.

### GetBytes(ReadOnlySpan&lt;char&gt; s)

The method decodes all the characters in the specified read-only span into a byte array.
In this way, `GetBytes` method performs a reverse operation to `GetString`.

For example, `GetBytes` method of a Base16 encoding can be used like this:

``` C#
using Gapotchenko.FX.Data.Encoding;

byte[] data = Base16.GetBytes("0102030A");
foreach (var i in data)
    Console.WriteLine(i);
```

producing the following output:

```
1
2
3
10
```

### CreateEncoder(TextWriter textWriter)

The method creates a streaming encoder for the specified binary-to-text data encoding.

Example:

``` C#
using Gapotchenko.FX.Data.Encoding;
using System.IO;

var encoding = Base16.Instance;

// Create a destination text writer.
var sw = new StringWriter();

// Create a streaming encoder.
var stream = encoding.CreateEncoder(sw);

// Stream the data.
stream.Write(1);
stream.Write(2);
stream.Write(3);
stream.Write(10);

// Flush the data.
stream.Flush();

Console.WriteLine(sw.ToString());
```

The output:

```
0102030A
```

It is worth mentioning that the streaming encoder also supports the asynchronous operations.

### CreateDecoder(TextReader textReader)

The method creates a streaming decoder for the specified binary-to-text data encoding.
It can be viewed as a reverse operation to `CreateEncoder` and it fully supports the asynchronous operations as well.

## Transcoding Between Various Binary-To-Text Encodings

Imagine a Base64-encoded file that needs to be converted to Base32.
The file is pretty large, around 2 gigabytes of data.

A naive approach would be to read all the data from the file beforehand in order to re-encode it later:

``` C#
using Gapotchenko.FX.Data.Encoding;
using System.IO;

// Read the file.
var text = File.ReadAllText("2GB-base64.txt");

// Decode the Base64 data.
var data = Base64.GetBytes(text);

// Re-encode the data with Base32 encoding.
text = Base32.GetString(data);

// Save the new file.
File.WriteAllText("2GB-base32.txt", text);
```

It will work but obviously will consume at least 2 GB of RAM.

This is when the concept of streaming becomes handy.
A better transcoding algorithm can use just a fraction of RAM to perform the very same operation of re-encoding a Base64-encoded file to Base32:

``` C#
using Gapotchenko.FX.Data.Encoding;
using System.IO;

var sourceEncoding = Base64.Instance;
var destinationEncoding = Base32.Instance;

// Open the source file.
using var sourceTextReader = File.OpenText("2GB-base64.txt");
// Create the destination file.
using var destinationTextWriter = File.CreateText("2GB-base32.txt");

// Create a streaming decoder for the Base64-encoded source file.
var sourceStream = sourceEncoding.CreateDecoder(sourceTextReader);
// Create a streaming encoder for the Base32-encoded destination file.
var destinationStream = destinationEncoding.CreateEncoder(destinationTextWriter);

// Transcode the file by copying the source stream to destination.
sourceStream.CopyTo(destinationStream);
destinationStream.Flush();
```

The transcoding algorithm based on streaming codecs presented above is highly efficient in terms of memory usage and consumes just a few kilobytes of RAM to transcode a file of any size.

## Data Encoding Algorithms

`Gapotchenko.FX.Data.Encoding` module provides only the framework for data encoding algorithms.
If you want to use a ready-to-use algorithm, Gapotchenko.FX provides quite a few out of the box:

| Algorithm Family    | Module                                                                                                 | Algorithms |
| ------------------- | ------------------------------------------------------------------------------------------------------ | ---------- |
| Base16              | [Gapotchenko.FX.Data.Encoding.Base16](../Gapotchenko.FX.Data.Encoding.Base16#readme)   | Base16
| Base24              | [Gapotchenko.FX.Data.Encoding.Base24](../Gapotchenko.FX.Data.Encoding.Base24#readme)   | Kuon Base24
| Base32              | [Gapotchenko.FX.Data.Encoding.Base32](../Gapotchenko.FX.Data.Encoding.Base32#readme)   | Base32, base32-hex, Crockford Base 32, z-base-32
| Base64              | [Gapotchenko.FX.Data.Encoding.Base64](../Gapotchenko.FX.Data.Encoding.Base64#readme)   | Base64, Base64 URL

Moreover, you can create your own data encoding algorithm.
Gapotchenko.FX project welcomes contributions, or it can be a standalone NuGet package that uses `Gapotchenko.FX.Data.Encoding` module as a wireframe.

## Usage

`Gapotchenko.FX.Data.Encoding` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Encoding):

```
PM> Install-Package Gapotchenko.FX.Data.Encoding
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../../../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../../../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../../../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data](#)
  - &#x27B4; [Gapotchenko.FX.Data.Encoding](../Gapotchenko.FX.Data.Encoding)
    - [Gapotchenko.FX.Data.Encoding.Base16](../Gapotchenko.FX.Data.Encoding.Base16)
    - [Gapotchenko.FX.Data.Encoding.Base24](../Gapotchenko.FX.Data.Encoding.Base24)
    - [Gapotchenko.FX.Data.Encoding.Base32](../Gapotchenko.FX.Data.Encoding.Base32)
    - [Gapotchenko.FX.Data.Encoding.Base64](../Gapotchenko.FX.Data.Encoding.Base64)
  - [Gapotchenko.FX.Data.Integrity.Checksum](../../Integrity/Checksum/Gapotchenko.FX.Data.Integrity.Checksum)
- [Gapotchenko.FX.Diagnostics](../../../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../../../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../../../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../../../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../../../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Security.Cryptography](../../../Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../../../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../../../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.ValueTuple](../../../Gapotchenko.FX.ValueTuple)

Or look at the [full list of modules](../../..#available-modules).
