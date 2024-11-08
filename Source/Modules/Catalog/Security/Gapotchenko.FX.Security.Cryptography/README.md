# Gapotchenko.FX.Security.Cryptography

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Security.Cryptography.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Security.Cryptography)

The module provides an extended set of cryptography primitives and algorithms for .NET.

## ARC4

ARC4 (Alleged Rivest Cipher 4 also known as ARCFOUR or RC4) is a stream cipher.
While it is remarkable for its simplicity and performance, nowadays its usage is limited due to discovered limitations and vulnerabilities.

Still, ARC4 may be a good enough fit for less sensitive tasks.
For example, it can be used for communication with legacy or energy-constrained systems.

Example:

``` C#
using Gapotchenko.FX.Security.Cryptography;

byte[] messageToEncrypt = ...;
byte[] key = ...;

// Encryption

byte[] encryptedMessage;

using (var arc4 = Arc4.Create())
{
    arc4.Key = key;

    var ms = new MemoryStream();
    using (var cryptoStream = new CryptoStream(ms, arc4.CreateEncryptor(), CryptoStreamMode.Write))
        cryptoStream.Write(messageToEncrypt, 0, messageToEncrypt.Length);

    encryptedMessage = ms.ToArray();
}

// Decryption

byte[] decryptedMessage;

using (var arc4 = Arc4.Create())
{
    arc4.Key = key;

    var ms = new MemoryStream();
    using (var cryptoStream = new CryptoStream(ms, arc4.CreateDecryptor(), CryptoStreamMode.Write))
        cryptoStream.Write(encryptedMessage, 0, encryptedMessage.Length);

    decryptedMessage = ms.ToArray();
}
```

## Usage

`Gapotchenko.FX.Security.Cryptography` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Security.Cryptography):

```
PM> Install-Package Gapotchenko.FX.Security.Cryptography
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../Data/Encoding/Gapotchenko.FX.Data.Encoding#readme)
- [Gapotchenko.FX.Diagnostics](../../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../../Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../../Gapotchenko.FX.Memory#readme)
- &#x27B4; [Gapotchenko.FX.Security.Cryptography](../Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../Gapotchenko.FX.Tuples#readme)

Or look at the [full list of modules](../../../..#readme).
