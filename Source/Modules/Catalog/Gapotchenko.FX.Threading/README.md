# Gapotchenko.FX.Threading

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Threading.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Threading)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Threading.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Threading)

The module provides extended primitives for multithreaded and asynchronous programming.

## TaskBridge

`TaskBridge` class from `Gapotchenko.FX.Threading` module provides seamless interoperability between synchronous and asynchronous code execution models.

Executing an async task from synchronous code poses a few rather big challenges in conventional .NET:
- The wait operation for an async task is prone to deadlocks unless a proper synchronization context is in place
- The cancellation models of sync and async code are different and often incompatible

Meet `TaskBridge`. It makes interoperability a breeze:

``` C#
using Gapotchenko.FX.Threading.Tasks;
using System;
using System.Threading.Tasks;

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

``` C#
using Gapotchenko.FX.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

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

``` C#
using Gapotchenko.FX.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

async Task DoJobAsync(CancellationToken ct) // can be canceled by a specified cancellation token
{
    // Executes a synchronous method that is thread-aborted when
    // a specified cancellation token is being canceled.
    await TaskBridge.ExecuteAsync(SyncMethod, ct); // <-- TaskBridge DOES THE MAGIC
}

void SyncMethod()
{
    …
}
```

As you can see, `TaskBridge` has a lot of chances to become your tool #1,
as it elegantly solves a world-class problem of bridging sync and async models together.

## `Sequential`, an Antogonist to `Parallel`

.NET platform provides `System.Threading.Tasks.Parallel` class that contains a bunch of static methods allowing one to execute the tasks in parallel.
But what if you want to temporarily switch them to a sequential execution mode?

Of course, you can do that manually, for example, by changing `Parallel.ForEach` method to `foreach` C# language keyword.
But this constitutes a lot of manual labour prone to errors.
That's why `Gapotchenko.FX.Threading` module provides `Sequential` class, a drop-in anotogonist to `Parallel`.
It allows you to make the switch by just changing the class name from `Parallel` to `Sequential` in a corresponding function call.
So `Parallel.ForEach` becomes `Sequential.ForEach`, and voila, the tasks are now executed sequentially allowing you to isolate that pesky multithreading bug you were hunting for.

### Automatic Selection Between Parallel and Sequential Execution Modes

`System.Threading.Tasks.Parallel` class allows you to execute tasks in parallel,
while `Gapotchenko.FX.Threading.Tasks.Sequential` allows you to do the same,
but sequentially.
But what if you want to get the best of two worlds?
Meet `DebuggableParallel` class provided by `Gapotchenko.FX.Threading` that does an automatic contextful choice for you.

<details>
<summary>More details</summary>

When a project has an attached debugger, `DebuggableParallel` primitive executes the specified tasks sequentially.
When there is no debugger attached, `DebuggableParallel` will execute the tasks in parallel.
And of course, it's a drop-in replacement for the ubiquitous `System.Threading.Tasks.Parallel` class.

The automatic selection of the task execution mode enables multi-threaded code to be effortlessly debugged,
while preserving the multi-core efficiency when no debugger is present.
The selection can also be overridden from code.
For example, if you want to disallow that debugger friendliness in `Release` configuration,
you can correspondingly configure the `DebuggableParallel` class at the very start of a program:

``` C#
using Gapotchenko.FX.Threading.Tasks;

#if !DEBUG
DebuggableParallel.Mode = DebuggableParallelMode.AlwaysParallel;
#endif
```

This makes the behavior of `DebuggableParallel` class to be essentially indistinguishable from `System.Threading.Tasks.Parallel`
without changing any other code.

</details>

## Asynchronous Synchronization

`Gapotchenko.FX.Threading` module provides plenty of synchronization primitives supporting not only synchronous, but also asynchronous execution models.
This closes the gap in the mainstream .NET BCL which has a decade-old lack of them.

<details>
<summary>Historical context</summary>

One of the main barriers for implementing asynchronous synchronization in .NET was the impossibility to achieve reentrancy.
That impossibility was caused by certain limitations of `System.AsyncLocal<T>` class that only supported downward control flow propagation.

However, using the tradition of rigorous and meticulous mathematical problem solving, `Gapotchenko.FX.Threading` became the world's first "clean" implementation of reentrant synchronization primitives for .NET.
The word "clean" means that it does not use such unreliable techniques as `System.Diagnostics.StackTrace`.
Previously, "clean" implementations were considered impossible to achieve due to aforementioned limitations of the `System.AsyncLocal<T>` class.

</details>

## Usage

`Gapotchenko.FX.Threading` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Threading):

```
PM> Install-Package Gapotchenko.FX.Threading
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Data](../Data/Encoding/Gapotchenko.FX.Data.Encoding)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Runtime.InteropServices](../Gapotchenko.FX.Runtime.InteropServices)
- [Gapotchenko.FX.Security.Cryptography](../Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- &#x27B4; [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.ValueTuple](../Gapotchenko.FX.ValueTuple)

Or look at the [full list of modules](..#available-modules).
