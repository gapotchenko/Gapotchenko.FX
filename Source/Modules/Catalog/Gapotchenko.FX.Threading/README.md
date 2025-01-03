﻿# Gapotchenko.FX.Threading

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Threading.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Threading)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Threading.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Threading)

The module provides complementary primitives for multithreaded and asynchronous programming in .NET.

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

<details>
<summary>More details on TaskBridge</summary>

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

</details>

## `Sequential`, an Antogonist to `Parallel`

.NET platform provides `System.Threading.Tasks.Parallel` class that contains a bunch of static methods allowing one to execute the tasks in parallel.
But what if you want to temporarily switch them to a sequential execution mode?

Of course, you can do that manually, for example, by changing `Parallel.ForEach` method to `foreach` C# language keyword.
But this constitutes a lot of manual labour prone to errors.
That's why `Gapotchenko.FX.Threading` module provides `Sequential` class, a drop-in anotogonist to `Parallel`.
It allows you to make the switch by just changing the class name from `Parallel` to `Sequential` in a corresponding function call.
So `Parallel.ForEach` becomes `Sequential.ForEach`, and voila, the tasks are now executed sequentially allowing you to isolate that pesky multithreading bug you were hunting for.

### Automatic Selection Between Parallel and Sequential

`System.Threading.Tasks.Parallel` class allows you to execute tasks in parallel,
while `Gapotchenko.FX.Threading.Tasks.Sequential` allows you to do the same,
but sequentially.
But what if you want to get the best of two worlds?
Meet `DebuggableParallel` class provided by `Gapotchenko.FX.Threading` module that does an automatic contextful choice for you.

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

## Asynchronous Concurrency

`Gapotchenko.FX.Threading` module provides plenty of synchronization primitives supporting not only synchronous, but also asynchronous execution models.
This closes the gap in the mainstream .NET BCL which had a decade-old lack of them.

<details>
<summary>Historical context</summary>

One of the main barriers for implementing asynchronous synchronization in .NET was the impossibility to achieve reentrancy.
That impossibility was caused by certain limitations of `System.AsyncLocal<T>` class that only supported downward propagation of information associated with an asynchronous control flow.

However, using the tradition of rigorous and meticulous mathematical problem solving, `Gapotchenko.FX.Threading` module became the world's first (clean) implementation of reentrant synchronization primitives for .NET's asynchronous execution model.
The word "clean" means that it does not use such unreliable techniques as `System.Diagnostics.StackTrace`.
Previously, clean implementations were considered impossible due to aforementioned limitations of the `System.AsyncLocal<T>` class.
If you are interested in gory implementation details, take a look at the corresponding [source file](Utils/ExecutionContextHelper.AsyncLocal.cs).

</details>

### AsyncLock

`Gapotchenko.FX.Threading.AsyncLock` represents a **reentrant** synchronization primitive
that ensures that only one task or thread can access a resource at any given time:

``` C#
using Gapotchenko.FX.Threading;

var lockObj = new AsyncLock();

await lockObj.EnterAsync();
try
{
    // Only one task can execute this section of code at any given time.
    // ...
}
finally
{
    lockObj.Exit();
}
```

`AsyncLock` implements `Gapotchenko.FX.Threading.IAsyncLockable` interface that gives you access to handy shortcuts for entering and exiting the lock scope asynchronously without manual `try`/`finally` constructs:

``` C#
using Gapotchenko.FX.Threading;

var lockObj = new AsyncLock();

using (await lockObj.EnterScopeAsync())
{
    // Only one task can execute this section of code at any given time.
    // ...
}
```

The `AsyncLock` can be acquired multiple times by the same task because the primitive supports reentrancy (which is also called recursion):

``` C#
using Gapotchenko.FX.Threading;

var lockObj = new AsyncLock();

using (await lockObj.EnterScopeAsync())
{
    using (await lockObj.EnterScopeAsync())
    {
        // Only one task can execute this section of code at any given time.
        // ...
    }
}
```

`Gapotchenko.FX.Threading.AsyncLock` is an asynchronous equivalent of [`System.Threading.Lock`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.lock) class.

### AsyncCriticalSection

`Gapotchenko.FX.Threading.AsyncCriticalSection` represents a **non-reentrant** synchronization primitive
that ensures that only one task or thread can access a resource at any given time.
`AsyncCriticalSection` is a non-reentrant variant of the `AsyncLock` class:

``` C#
using Gapotchenko.FX.Threading;

var cs = new AsyncCriticalSection();

using (await cs.EnterScopeAsync())
{
    // Only one task can execute this section of code at any given time.
    // ...
}
```

The benefit of using `AsyncCriticalSection` in comparison to `AsyncLock` is that the former uses fewer computational resources because it has no need to track reentrancy.
So if you know that your algorithm does not require recursive locking, using `AsyncCriticalSection` is more preferable.

### AsyncManualResetEvent

`Gapotchenko.FX.Threading.AsyncManualResetEvent` represents a synchronization primitive that, when signaled, allows one or more tasks waiting on it to proceed.
`AsyncManualResetEvent` must be reset manually.

This is an asynchronous equivalent of [`System.Threading.ManualResetEvent`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.manualresetevent) class.

### AsyncAutoResetEvent

`Gapotchenko.FX.Threading.AsyncAutoResetEvent` represents a synchronization primitive that, when signaled, allows one or more tasks waiting on it to proceed.
`AsyncAutoResetEvent` resets automatically after releasing a single waiting task.

This is an asynchronous equivalent of [`System.Threading.AutoResetEvent`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.autoresetevent) class.

### AsyncMonitor

`Gapotchenko.FX.Threading.AsyncMonitor` represents a reentrant concurrency primitive that provides a mechanism that synchronizes access to objects:

``` C#
using Gapotchenko.FX.Threading;

// Only one task can access this variable at any given time.
int? result = null;

var monitor = new AsyncMonitor();

// Start consuming and producing tasks in parallel.
await Task.WhenAll(
    Consume("Consumer #1"),
    Consume("Consumer #2"),
    Produce());

async Task Consume(string taskName)
{
    using (await monitor.EnterScopeAsync())
    {
        while (result == null)
            await monitor.WaitAsync();
        await Console.Out.WriteLineAsync($"Result for {taskName}: {result}");
    }
}

async Task Produce()
{
    await Task.Delay(1000); // pretend that it takes time to come up with a result
    using (await monitor.EnterScopeAsync())
    {
        result = 42;
        monitor.NotifyAll();
    }
}
```

The example code above produces the following output after one second of waiting:

```
Result for Consumer #1: 42
Result for Consumer #2: 42
```

`Gapotchenko.FX.Threading.AsyncMonitor` is an asynchronous equivalent of [`System.Threading.Monitor`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.monitor) class.

<details>
<summary>More details on AsyncMonitor</summary>

`Gapotchenko.FX.Threading.AsyncMonitor` class can be used as a drop-in replacement for `System.Threading.Monitor`.

Let's take a look at example.
The synchronous code below uses `System.Threading.Monitor`:

``` C#
class OldCode
{
    Queue<int> m_WorkItems = new();

    public void AddWorkItem(int item)
    {
        lock (m_WorkItems)
        {
            m_WorkItems.Enqueue(item);
            Monitor.Pulse(m_WorkItems);
        }
    }

    public int GetWorkItem()
    {
        lock (m_WorkItems)
        {
            while (m_WorkItems.Count == 0) // wait for items to appear in the queue
                Monitor.Wait(m_WorkItems);
            return m_WorkItems.Dequeue();
        }    
    }
}
```

It can be translated to asynchronous code using `Gapotchenko.FX.Threading.AsyncMonitor` class, almost word for word:

``` C#
class NewCode
{
    Queue<int> m_WorkItems = new();

    public async Task AddWorkItemAsync(int item)
    {
        var monitor = AsyncMonitor.For(m_WorkItems);
        using (await monitor.EnterScopeAsync())
        {
            m_WorkItems.Enqueue(item);
            monitor.Notify();
        }
    }

    public async Task<int> GetWorkItemAsync()
    {
        var monitor = AsyncMonitor.For(m_WorkItems);
        using (await monitor.EnterScopeAsync())
        {
            while (m_WorkItems.Count == 0) // wait for items to appear in the queue
                await monitor.WaitAsync();
            return m_WorkItems.Dequeue();
        }    
    }

    // Note that it's possible to mix asynchronous and synchronous execution models
    // when using synchronization primitives provided by Gapotchenko.FX.Threading module.
    // Both models happily coexist in terms of concurrency.
    //
    // For example, that useful possibility may be used for a gradual codebase migration
    // from synchronous execution model to asynchronous.

    public void AddWorkItem(int item)
    {
        var monitor = AsyncMonitor.For(m_WorkItems);
        using (monitor.EnterScope())
        {
            m_WorkItems.Enqueue(item);
            monitor.Notify();
        }
    }

    public int GetWorkItem()
    {
        var monitor = AsyncMonitor.For(m_WorkItems);
        using (monitor.EnterScope())
        {
            while (m_WorkItems.Count == 0) // wait for items to appear in the queue
                monitor.Wait();
            return m_WorkItems.Dequeue();
        }    
    }
}
```

Note the use of `AsyncMonitor.For(object)` method in the code above.
It allows to imitate the semantics associated with `lock (object)` C# keyword that attaches a monitor to the specified object.
`AsyncMonitor.For(object)` method is provided for semantical and ideological compatibility with `System.Threading.Monitor` class in order to ease the translation of existing codebases.
As a more efficient approach, however, it is recommended to use an instance of `AsyncMonitor` explicitly without "attaching" it to a particular object:

``` C#
class NewCode
{
    Queue<int> m_WorkItems = new();
    AsyncMonitor m_Monitor = new();

    public async Task AddWorkItemAsync(int item)
    {
        using (await m_Monitor.EnterScopeAsync())
        {
            m_WorkItems.Enqueue(item);
            m_Monitor.Notify();
        }
    }

    // ...
}
```

</details>

## Usage

`Gapotchenko.FX.Threading` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Threading):

```
PM> Install-Package Gapotchenko.FX.Threading
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../Data/Encoding/Gapotchenko.FX.Data.Encoding#readme)
- [Gapotchenko.FX.Diagnostics](../Diagnostics/Gapotchenko.FX.Diagnostics.CommandLine#readme)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text#readme)
- &#x27B4; [Gapotchenko.FX.Threading](.#readme)
- [Gapotchenko.FX.Tuples](../Gapotchenko.FX.Tuples#readme)

Or look at the [full list of modules](../..#readme).
