# Gapotchenko.FX.Threading

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Threading.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Threading)

The module provides extended primitives for multithreaded and asynchronous programming.

## TaskBridge

`TaskBridge` class from `Gapotchenko.FX.Threading` module provides seamless interoperability between synchronous and asynchronous code execution models.

Executing an async task from synchronous code poses a few rather big challenges in conventional .NET:
- The wait operation for an async task is prone to deadlocks unless a proper synchronization context is in place
- The cancellation models of sync and async code are different and often incompatible

Meet `TaskBridge`. It makes interoperability a breeze:

``` csharp
using System;
using Gapotchenko.FX.Threading;

class Program
{
    static void Main()
    {
        TaskBridge.Execute(RunAsync);
    }

    static async Task RunAsync()
    {
        await Console.Out.WriteLineAsync("Hello, Async World!");
    }
}
```

### Cancellation Models

`TaskBridge` provides automatic interoperability between different cancellation models.

Let's call a cancelable async method from a synchronous thread that can be aborted by `Thread.Abort()` method:

``` csharp
using System.Threading;
using Gapotchenko.FX.Threading;

void SyncMethod() // can be canceled by Thread.Abort()
{
    // Executes an async task that is gracefully canceled via cancellation
    // token when current thread is being aborted or interrupted.
    TaskBridge.Execute(DoJobAsync); // <-- TaskBridge DOES THE MAGIC
}

async Task DoJobAsync(CancellationToken ct)
{
    …
    // Gracefully handles cancellation opportunities.
    ct.ThrowIfCancellationRequested();
    …
}
```

You see this? A simple one-liner for a *complete* interoperability between two execution models.

Now, let's take a look at the opposite scenario where a cancelable async task calls an abortable synchronous code:

``` csharp
using System.Threading;
using Gapotchenko.FX.Threading;

async Task DoJobAsync(CancellationToken ct) // can be canceled by supplied cancellation token
{
    // Executes a synchronous method that is thread-aborted when
    // the specified cancellation token is being canceled.
    await TaskBridge.ExecuteAsync(SyncMethod, ct); // <-- TaskBridge DOES THE MAGIC
}

void SyncMethod()
{
    …
}
```

As you can see, `TaskBridge` has a lot of chances to become your tool #1,
as it elegantly solves a world-class problem of bridging sync and async models together.

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- &#x27B4; [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
