# Gapotchenko.FX.Diagnostics.Process

The module provides extended functionality for process manipulation.

## Process Extensions

### GetParent()

`GetParent()` is an extension method provided by `Gapotchenko.FX.Diagnostics.Process` module
for `System.Diagnostics.Process` class.

What it does is returns the parent process. Or `null` when parent process is absent or no longer running.

### EnumerateParents()

Enumerates a chain of parent processes beginning with the closest parent.

### ReadEnvironmentVariables()

Reads environment variables of a process.

The functionality is achieved by reading the process environment block (PEB) at the operating system level.

For example, this is how a PATH environment variable can be retrieved from all running instances of Microsoft Visual Studio:

``` csharp
using System;
using System.Diagnostics;
using Gapotchenko.FX.Diagnostics;

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
The default mode of operation is `ProcessEndMode.Complete` that goes as follows:

 1. Graceful techniques:  
   1.1. Tries to close a main window of a process  
   1.2. If that fails, tries to send Ctrl+C (SIGTERM) signal
 2. Forceful techniques:  
   2.1. If graceful techniques failed, tries to exit the process (suitable for the current process only)  
   2.2. If that fails, kills the process (SIGKILL)

`End()` method returns a `ProcessEndMode` value on completion indicating how the process was actually ended.

Let's take a look on example that tries to end all running instances of Notepad:

``` csharp
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

Now let's repeat the experiment by launching a Notepad app again and opening its File Save dialog via menu (File -> Save As...).
Let's keep the File Save dialog open, and launch the example code once again.

This time the result will be different:
```
Kill
```

The Notepad process was unable to shutdown gracefully and thus was forcefully killed.
Graceful shutdown was not possible because the process had an active modal dialog.

#### More Examples

Let's modify sample a bit:

``` csharp
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

``` csharp
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

``` csharp
foreach (var process in Process.GetProcessesByName("notepad"))
{
    var result = process.End(ProcessEndMode.Interrupt | ProcessEndMode.Kill);

    Console.WriteLine("PID {0}", process.Id);
    Console.WriteLine(result);
}
```

As you can see, despite a simple-looking `End(…)` method the possibilities of achieving a specific goal are enormous.

### EndAsync()

The method is similar to `End()` but has an async implementation.
If was created in order to efficiently handle a lot of processes in a bulk.

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
  - [Gapotchenko.FX.Diagnostics.CommandLine](../Gapotchenko.FX.Diagnostics.CommandLine)
  - &#x27B4; [Gapotchenko.FX.Diagnostics.Process](../Gapotchenko.FX.Diagnostics.Process)
  - [Gapotchenko.FX.Diagnostics.WebBrowser](../Gapotchenko.FX.Diagnostics.WebBrowser)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
