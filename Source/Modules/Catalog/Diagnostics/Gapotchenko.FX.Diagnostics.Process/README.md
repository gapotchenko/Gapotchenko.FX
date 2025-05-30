﻿# Gapotchenko.FX.Diagnostics.Process

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../../../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Diagnostics.Process.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Diagnostics.Process)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Diagnostics.Process.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Diagnostics.Process)

The module provides extended functionality for process manipulation.

## Process Extensions

### GetParent()

`GetParent()` is an extension method provided by `Gapotchenko.FX.Diagnostics.Process` module
for `System.Diagnostics.Process` class.

`GetParent()` method returns a parent process of the specified process, or `null` when the parent process is absent or is no longer running.

### EnumerateParents()

Enumerates a chain of parent processes beginning with the closest parent.

### ReadEnvironmentVariables()

Reads environment variables of a process.

The functionality is achieved by reading the process environment block (PEB) at the operating system level.

For example, this is how a PATH environment variable can be retrieved from all running instances of Microsoft Visual Studio:

``` C#
using Gapotchenko.FX.Diagnostics;
using System;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        var processes = Process.GetProcessesByName("devenv");

        if (processes.Length == 0)
            Console.WriteLine("Process with a given name not found. Please modify the code and specify the existing process name.");

        foreach (var process in processes)
        {
            Console.WriteLine();
            Console.WriteLine("Process with ID {0} has the following value of PATH environment variable:", process.Id);

            var env = process.ReadEnvironmentVariables();

            string path = env["PATH"];
            Console.WriteLine(path);
        }
    }
}
```

### ReadArguments()

The method retrieves a set of command-line arguments specified at the process start.
The functionality is achieved by reading the process environment block at the operating system level.

``` C#
using Gapotchenko.FX.Diagnostics;
using System;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        var processes = Process.GetProcessesByName("devenv");

        if (processes.Length == 0)
            Console.WriteLine("Process with a given name not found. Please modify the code and specify the existing process name.");

        foreach (var process in processes)
        {
            Console.WriteLine(
                "Process with ID {0} was started with the following command line: {1}",
                process.Id,
                process.ReadArguments());
        }
    }
}
```

### ReadArgumentList()

The method retrieves a sequence of command-line arguments specified at the process start.
It is similar to aforementioned `ReadArguments()` method but returns a sequence of command-line arguments instead of a single command line string.

This fundamental difference may be essential in multi-platform scenarios.
For instance, Windows OS natively represents the command line of a process as a single string, while Unix operating systems natively represent the command line as a strongly-typed array of command-line arguments.

Whichever method is used the results are similar, but every method provides a higher degree of detalization for a particular operating system.

### End()

Allows to end a process according to a specified mode of operation.

The `End()` method is interesting and a bit intricate.
The stock `Process` class already provides a similar `Kill()` method
which performs an immediate forceful termination of a process without giving it a chance to exit gracefully.

Depending on a kind of process being terminated, `Kill()` is not always suitable.
For example, it may have devastating consequences if someone kills a Microsoft Visual Studio process without giving it a graceful shutdown.
Lost files, potentially corrupted extensions and so on.

Meet the `End()` method provided by `Gapotchenko.FX.Diagnostics.Process` module.
It allows to end a process according to a specified mode of operation.
The default mode of operation is `ProcessEndMode.Complete` that follows a sequence presented below:

 1. Graceful techniques are tried first:  
   1.1. `End()` method tries to close a main window of the process  
   1.2. If that fails, it tries to send Ctrl+C (SIGTERM) signal to the process
 2. Forceful techniques:  
   2.1. If graceful techniques fail, `End()` method tries to exit the process (suitable for the current process only)  
   2.2. If that fails, it kills the process (SIGKILL)

The method returns a `ProcessEndMode` value on completion indicating how the process was actually ended.

Let's take a look on example that tries to end all running instances of Notepad:

``` C#
using Gapotchenko.FX.Diagnostics;

foreach (var process in Process.GetProcessesByName("notepad"))
{
    var result = process.End();
    Console.WriteLine(result);
}
```

Once there is a running Notepad app, the sample produces the following output:

```
Close
```

signifying that a Notepad process was gracefully ended by closing its main window.

Now let's repeat the experiment by launching a Notepad app again and opening its "File Save" dialog via menu (File -> Save As...).
This time, let's keep the "File Save" dialog open, and launch the example code once again.

This time the result will be different:
```
Kill
```

The Notepad process was unable to shutdown gracefully and thus was forcefully killed.
Graceful shutdown was not possible because the process had an active modal dialog.

<details>
  <summary>More examples</summary>

Let's modify sample a bit:

``` C#
foreach (var process in Process.GetProcessesByName("notepad"))
{
    var result = process.End();

    Console.WriteLine("PID {0}", process.Id);
    Console.WriteLine("Graceful: {0}", (result & ProcessEndMode.Graceful) != 0);
    Console.WriteLine("Forceful: {0}", (result & ProcessEndMode.Forceful) != 0);
}
```

Now it shows the `Id` of a process that was ended together with a graceful/forceful classification of the result.

What if we want to limit the `End()` method to only perform a graceful process termination?
Let's use the `End(ProcessEndMode)` method overload:

``` C#
foreach (var process in Process.GetProcessesByName("notepad"))
{
    var result = process.End(ProcessEndMode.Graceful);

    Console.WriteLine("PID {0}", process.Id);
    Console.WriteLine(result);
}
```

Now the result will only be graceful, or have `ProcessEndMode.None` value if a process could not be gracefully ended.

But what if we want to limit the `End()` method to only perform a graceful process termination via Ctrl+C (SIGINT) signal and forceful kill?
No problem:

``` C#
foreach (var process in Process.GetProcessesByName("notepad"))
{
    var result = process.End(ProcessEndMode.Interrupt | ProcessEndMode.Kill);

    Console.WriteLine("PID {0}", process.Id);
    Console.WriteLine(result);
}
```

As you can see, despite a simple-looking signature, the `End(…)` method gives enormous possibilities for achieving a specific goal.

</details>

### EndAsync()

The method is similar to `End()` but has an async implementation.
It can be used to efficiently handle dozens of processes in a bulk.

For example, there is a need to shut down quite a few processes, say 20.
One possible solution is to shut down the processes sequentially, one by one.
But this may take quite a few moments to complete, up to several dozen seconds.
On a positive side, it only takes one CPU thread.
The second solution is to shut down the processes in parallel.
This solution will reduce the overall time significantly, but that reduction will come at the expense of 20 wasted CPU threads.
What if we could have the benefits of both solutions at the same time?

We can achieve that by using asynchronous code, like so:

``` C#
static Task<ProcessEndMode[]> EndProcessesAsync(IEnumerable<Process> processesToEnd) =>
    Task.WhenAll(processesToEnd.Select(x => x.EndAsync()));
```

Even if you write a single-threaded app, you still get the benefits of asynchronous code here: the processes are shut down in parallel and no CPU threads are wasted.

Here is a full example that demonstrates that:

``` C#
using Gapotchenko.FX.Diagnostics;
using Gapotchenko.FX.Threading.Tasks;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        var processEndModes = TaskBridge.Execute(
            EndProcessesAsync(
                Process.GetProcessesByName("notepad"));

        Console.WriteLine(
            "Notepads were ended with the following results: {0}.",
            string.Join(", ", processEndModes.Select(x => x.ToString())));
    }

    static Task<ProcessEndMode[]> EndProcessesAsync(IEnumerable<Process> processesToEnd) =>
        Task.WhenAll(processesToEnd.Select(x => x.EndAsync()));
}
```

(Reminder: the correct way to wait for the completion of an asynchronous task in synchronous code is to use [`TaskBridge`](../../Gapotchenko.FX.Threading#taskbridge))

## Usage

`Gapotchenko.FX.Diagnostics.Process` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Diagnostics.Process):

```
PM> Install-Package Gapotchenko.FX.Diagnostics.Process
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../../Gapotchenko.FX#readme)
- [Gapotchenko.FX.AppModel.Information](../../AppModel/Gapotchenko.FX.AppModel.Information#readme)
- [Gapotchenko.FX.Collections](../../Gapotchenko.FX.Collections#readme)
- [Gapotchenko.FX.Console](../../Gapotchenko.FX.Console#readme)
- [Gapotchenko.FX.Data](../../Data/Encoding/Gapotchenko.FX.Data.Encoding#readme)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine#readme)
  - [Gapotchenko.FX.Diagnostics.CommandLine](../Gapotchenko.FX.Diagnostics.CommandLine#readme)
  - &#x27B4; [Gapotchenko.FX.Diagnostics.Process](.#readme)
  - [Gapotchenko.FX.Diagnostics.WebBrowser](../Gapotchenko.FX.Diagnostics.WebBrowser#readme)
- [Gapotchenko.FX.IO](../../Gapotchenko.FX.IO#readme)
- [Gapotchenko.FX.Linq](../../Linq/Gapotchenko.FX.Linq#readme)
- [Gapotchenko.FX.Math](../../Math/Gapotchenko.FX.Math#readme)
- [Gapotchenko.FX.Memory](../../Gapotchenko.FX.Memory#readme)
- [Gapotchenko.FX.Security.Cryptography](../../Security/Gapotchenko.FX.Security.Cryptography#readme)
- [Gapotchenko.FX.Text](../../Gapotchenko.FX.Text#readme)
- [Gapotchenko.FX.Threading](../../Gapotchenko.FX.Threading#readme)
- [Gapotchenko.FX.Tuples](../../Gapotchenko.FX.Tuples#readme)

Or look at the [full list of modules](../../..#readme).
