﻿# Gapotchenko.FX.Data.Linq

<!--
<docmeta>
	<complexity>advanced</complexity>
</docmeta>
-->

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Data.Linq.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Data.Linq)

`Gapotchenko.FX.Data.Linq` is a complementary module to `System.Data.Linq` library provided as a part of .NET Framework.
The module provides async support for LINQ2SQL technology.


The async support is provided by two extension methods for `System.Data.Linq.DataContext` class:

- `ExecuteQueryAsync`
- `SubmitChangesAsync`

Those async methods can be used instead of synchronous `ExecuteQuery` and `SubmitChanges` alternatives whenever you need to execute a LINQ to SQL query asynchronously.

## Example

``` csharp
using Gapotchenko.Data.Linq;
using System.Linq;
using System.Threading.Tasks;

async Task<DateTime> GetCustomerRegistrationDate(string name)
{
    using (var db = new ContosoDataContext())
    {
        var query = db.Customers.Where(x => x.Name == name);
        var result = await db.ExecuteQueryAsync(query);
        return result.Single().RegistrationDate;
    }
}
```

## Usage

`Gapotchenko.FX.Data.Linq` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Data.Linq):

```
PM> Install-Package Gapotchenko.FX.Data.Linq
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data](../Encoding/Gapotchenko.FX.Data.Encoding)
  - [Gapotchenko.FX.Data.Encoding](../Encoding/Gapotchenko.FX.Data.Encoding)
  - [Gapotchenko.FX.Data.Integrity.Checksum](../Integrity/Checksum/Gapotchenko.FX.Data.Integrity.Checksum)
  - &#x27B4; [Gapotchenko.FX.Data.Linq](../Gapotchenko.FX.Data.Linq) ✱
- [Gapotchenko.FX.Diagnostics](../../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Numerics](../../Gapotchenko.FX.Numerics) ✱
- [Gapotchenko.FX.Reflection.Loader](../../Gapotchenko.FX.Reflection.Loader) ✱
- [Gapotchenko.FX.Security.Cryptography](../../Security/Cryptography/Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../../Gapotchenko.FX.Threading)

Symbol ✱ denotes an advanced module.

Or take a look at the [full list of modules](../..#available-modules).
