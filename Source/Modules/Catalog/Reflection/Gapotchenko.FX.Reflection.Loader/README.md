# Gapotchenko.FX.Reflection.Loader

<!--
<docmeta>
	<complexity>advanced</complexity>
</docmeta>
-->

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Reflection.Loader.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Reflection.Loader)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Gapotchenko.FX.Reflection.Loader.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Reflection.Loader)

The module provides versatile primitives that can be used to automatically lookup and load assembly dependencies in various dynamic scenarios.

-----------------------------------------------------------------------------

## Gentle Introduction

Assembly loading plays a crucial role in .NET apps.
Once the app is started, .NET runtime ensures that all required assemblies are gradually loaded.

Whenever the code hits the point where a type from another assembly is used, it raises `AppDomain.AssemblyResolve` event.
The good thing is .NET comes pre-equipped with a default assembly loader, which does a sensible job for most applications.

However, there are situations when having a default assembly loader is just not enough.
This is where `Gapotchenko.FX.Reflection.Loader` module becomes extremely handy.

-----------------------------------------------------------------------------

## Scenario #1. Load dependent assemblies from an app's outside directory

Let's take a look on example scenario.
Suppose we have `ContosoApp` installed at `C:\Program Files\ContosoApp` directory.
The directory contains a single `ContosoApp.exe` assembly which represents the main executable file of the app.

`ContosoApp.exe` has a dependency on `ContosoEngine.dll` assembly which is located at
`C:\Program Files\Common Files\Contoso\Engine` directory.
It so happens ContosoApp uses a common engine developed by the company.

Now when `ContosoApp.exe` is run, it bails out with the following exception:

```
System.IO.FileNotFoundException: Could not load file or assembly 'ContosoEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies. The system cannot find the file specified.
```

It occurs because `ContoseEngine.dll` assembly is located at the outside directory,
and the default .NET assembly loader does not provide an easy way to cover scenarios like this.

In order to cover that scenario, a developer would subscribe to `AppDomain.CurrentDomain.AssemblyResolve` event.
Then he would come up with a custom assembly lookup and loading logic.
The thing is: that is **not** a straightforward thing to do.
Even more than that, it is full of gotchas and caveats.
And they will painfully *bite* a developer on subtle occasions, now and then.

That's why `Gapotchenko.FX.Reflection.Loader` module provides a ready to use `AssemblyAutoLoader` class that reliably covers the scenarios like that.

Here is the solution for ContosoApp:

``` C#
using Gapotchenko.FX.Reflection;

namespace ContosoApp;

class Program
{
    static void Main()
    {
        // The statement below instructs Gapotchenko.FX assembly loader to use
        // 'C:\Program Files\Common Files\Contoso\Engine' directory as a probing path for
        // dependent assemblies.
        AssemblyAutoLoader.Default.AddProbingPath(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                @"Contoso\Engine"));
            
        Run();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void Run()
    {
        // ...
    }
}
```

> [!NOTE] 
> `Run` method is annotated by `[MethodImpl(MethodImplOptions.NoInlining)]` attribute.
> That attribute instructs .NET runtime to not inline the `Run` method into its calling methods.
> It is necessary to disable inlining because the `Run` method may potentially reference types from not yet loaded assemblies, specifically those from `ContosoEngine.dll`.
> This will lead to inability of .NET runtime to start executing its calling methods at all (i.e. the `Main` method), because those types are resolved before method starts to run.
> And they cannot be resolved until a proper assembly loader is configured,
> and it will not be configured due to the presence of unresolvable type references that were inlined from `Run` method.
> To break that chicken and egg lock, the `Run` method inlining should be disabled.

## Scenario #2. Load dependent assemblies from an inner directory of an app

ContosoApp continues to evolve and now it has a dependency on `Newtonsoft.Json.dll` assembly.
A straightforward approach would be to put `Newtonsoft.Json.dll` assembly just besides `ContosoApp.exe`.


But Mr. Alberto Olivetti from Contoso's Deployment Division decided that an additional file laying near `ContosoApp.exe` would be an unwanted distraction for command line users of the app.
Mr. Olivetti tends to pay a lot of respect to his customers and wants to save their time while they are hanging around `ContosoApp.exe`.
Thus Alberto came up with a respectful solution to put all third-party assemblies to `Components` subdirectory of the app.

Now how can `ContosoApp.exe` module load the required assemblies from `Components` directory?
Thankfully, the default .NET assembly loader allows to achieve that by specifying a set of private probing paths in application configuration file:

```xml
<configuration>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <probing privatePath="Components" />
    </assemblyBinding>
  </runtime>
</configuration>
```

The task is solved for `ContosoApp` (and every other .NET Framework app as well).
The default .NET Framework assembly loader can be instructed to load dependent assemblies from inner directories of an app by specifying a set of private probing paths.

There is another story for .NET Core and .NET which do not directly support additional probing paths.
For those target frameworks, using `AssemblyAutoLoader` becomes worthy even for inner directories: 

``` C#
using Gapotchenko.FX.Reflection;

namespace ContosoApp;

class Program
{
    static void Main()
    {
        AssemblyAutoLoader.Default.AddProbingPath(
            Path.Combine(
                Path.GetDirectoryName(typeof(Program).Assembly.Location),
                "Components"));
            
        Run();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void Run()
    {
        // ...
    }
}
```

## Scenario #3. Specifying probing paths for a .DLL assembly

But what if you need to specify probing paths not for a whole app, but for a specific assembly only?
Say you created an Autodesk AutoCAD plugin that depends on `Newtonsoft.Json.dll` and a bunch of other assemblies,
and then want to put all those third-party files somewhere else.

Contoso company met the very same challenge.
They created an AutoCAD plugin for their ContosoApp product.
A straightforward way was to redistribute the dependencies together with plugin but its size then skyrocketed to 1 GB in ZIP file.
Bummer.

The substantial contributor to the size was ContosoEngine which was about 3 GB unzipped.
Mr. Alberto Olivetti, Contoso's deployment specialist, quickly recognized an opportunity to use a shared setup of ContosoEngine,
which was already present at `C:\Program Files\Common Files\Contoso\Engine` directory.

So the AutoCAD plugin (a .DLL assembly) had to gain an ability to load the dependencies from that directory.

This is what Alberto did. He created `AssemblyLoader` class in AutoCAD plugin assembly with just one method `Activate`:

``` C#
namespace ContosoApp.Integration.AutoCAD;

static class AssemblyLoader
{
    public static void Activate()
    {
    }
}
```

Alberto then ensured that `Activate` method is getting called at the early stages of a plugin lifecycle:

``` C#
namespace ContosoApp.Integration.AutoCAD;

public class Plugin : AutodeskPluginBase
{
    public override void Initialize()
    {
        AssemblyLoader.Activate();

        base.Initialize();

        // ...
    }
}
```

Now Alberto had a skeleton for a proper assembly loader initialization.
The only missing part was the actual implementation which was going to be enormous.

Thanks to the prior experience with custom assembly loading, Alberto was aware about that fancy `AssemblyAutoLoader` class provided by `Gapotchenko.FX.Reflection.Loader` package.
So he wrote:

``` C#
using Gapotchenko.FX.Reflection;

namespace ContosoApp.Integration.AutoCAD;

static class AssemblyLoader
{
    static AssemblyLoader()
    {
        // The statement below instructs Gapotchenko.FX assembly loader to use
        // 'C:\Program Files\Common Files\Contoso\Engine' directory as a probing path for
        // resolution of 'ContosoApp.Integration.AutoCAD.dll' assembly dependencies.
        AssemblyAutoLoader.Default.AddAssembly(
            typeof(AssemblyLoader).Assembly,
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                @"Contoso\Engine"));
    }

    public static void Activate()
    {
    }
}
```

Please note how Alberto put the implementation inside a static constructor while leaving `Activate` method empty.
In that way, he was able to achieve a one-shot mode of execution, where the actual assembly loader initialization takes place only once on a first call to `Activate` method.
Smart.

But even if Alberto did not create a singleton, `AssemblyAutoLoader` is sophisticated enough to do the right job out of the box.

Now why did Alberto call `AddAssembly` method instead of `AddProbingPath`?
Both would work, actually. There is a subtle but very important difference.

`AddProbingPath` is a coarse "catch-all" method.
It would serve not only the dependencies of a given plugin assembly but would also cover the whole app domain.
Sometimes this is a beneficial behavior, like in case with the root `ContosoApp.exe` assembly.

In contrast, `AddAssembly` method provides a finer control.
It only serves the dependencies of a _specified assembly_.
It turns out to be a much saner choice for plugins where .NET app domain is shared among a lot of things.
In this way, assembly loaders from different plugins would not clash with each other, even when they look at a conflicting assembly dependency (it's easy to imagine that a lot of plugins would use the "same" but subtly different variant of `Newtonsoft.Json` module).

## Scenario #4. Automatic handling of binding redirects for a .DLL assembly

Assembly binding redirects allow to "remap" specific ranges of assembly versions.
The redirects are automatically created by build tools, and then being put to corresponding `.config` files of resulting assemblies.
[(Learn more)](https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/how-to-enable-and-disable-automatic-binding-redirection)

Assembly binding redirects work well for apps, but get completely broken if you want to employ them for dynamically loaded assemblies like plugins.
The default .NET loader simply ignores `.config` files of .DLL assemblies!

`Gapotchenko.FX.Reflection.Loader` solves this. Just add the following code to a place which gets executed at the early stage of the assembly lifecycle:

``` C#
AssemblyLoader.Activate()
```

`AssemblyLoader` implementation then goes as follows:

``` C#
using Gapotchenko.FX.Reflection;

namespace MyPlugin;

static class AssemblyLoader
{
    static AssemblyLoader()
    {
        // The statement below instructs Gapotchenko.FX assembly loader to add a specified
        // assembly to the list of sources to consider during assembly resolution process.
        // The loader automatically handles binding redirects according to a corresponding assembly
        // configuration (.config) file. If configuration file is missing then binding redirects are
        // automatically deducted according to the assembly compatibility heuristics.
        AssemblyAutoLoader.Default.AddAssembly(typeof(AssemblyLoader).Assembly);
    }

    public static void Activate()
    {
    }
}
```

There are a lot of projects that may need automatic handling of DLL binding redirects: T4 templates, MSBuild tasks, plugins, extensions etc.
Basically everything that gets dynamically loaded and depends on one or more NuGet packages with mishmash of versions.

-----------------------------------------------------------------------------

## A Chicken & Egg Dilemma

`Gapotchenko.FX.Reflection.Loader` module is distributed as a NuGet package with a single assembly file without dependencies.

This is done to avoid chicken & egg dilemma.
In this way, the default .NET assembly loader can always load the assembly despite the possible variety of different NuGet packages that can be used in the given project.

Another point to consider is **how to select a point of assembly loader installation** that is early enough in the assembly lifecycle.
This tends to be trivial for an app: the first few lines of the main entry point are good to go.
But it may be hard to do so for a class library due to the sheer breadth of the public API surface.
An assembly loader can then be installed at the module initializer of a class library to overcome that dilemma.

A module initializer can be seen as a constructor for an assembly (technically, it is a constructor for a module; each .NET assembly is comprised of one or more modules, typically just one).

[Fody/ModuleInit](https://github.com/Fody/ModuleInit) is an example of a tool that gives access to .NET module initialization functionality from high-level programming languages like C#/VB.NET.
Another option is to use a more specialized tool like [Eazfuscator.NET](https://www.gapotchenko.com/eazfuscator.net) that provides not only [module initialization functionality](https://help.gapotchenko.com/eazfuscator.net/63/sensei-features/module-initializers), but also intellectual property protection.

Please note that some .NET languages provide the out of the box support for module initializers.
For example, C# starting with version 9.0 treats all static methods marked with [`ModuleInitializerAttribute`](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.moduleinitializerattribute) as module initializers.

While `ModuleInitializerAttribute` is only available in .NET 5.0 and newer, the whole concept is perfectly functional with any .NET version once attribute definition is in place.
That's why [`Gapotchenko.FX`](../Gapotchenko.FX) module provides a ready to use [polyfill for that attribute](../Gapotchenko.FX/Runtime/CompilerServices/ModuleInitializerAttribute.cs).
The example of such an approach is presented below:

``` C#
using Gapotchenko.FX.Reflection;
using System.Runtime.CompilerServices;

namespace MyLibrary;

static class AssemblyLoader
{
    static AssemblyLoader()
    {
        AssemblyAutoLoader.Default.AddAssembly(typeof(AssemblyLoader).Assembly);
    }

    [ModuleInitializer]
    public static void Activate()
    {
    }
}
```

-----------------------------------------------------------------------------

## Usage

`Gapotchenko.FX.Reflection.Loader` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Reflection.Loader):

```
PM> Install-Package Gapotchenko.FX.Reflection.Loader
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
- [Gapotchenko.FX.Numerics](../Gapotchenko.FX.Numerics) ✱
- &#x27B4; [Gapotchenko.FX.Reflection.Loader](../Gapotchenko.FX.Reflection.Loader) ✱
- [Gapotchenko.FX.Security.Cryptography](../Gapotchenko.FX.Security.Cryptography)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
- [Gapotchenko.FX.ValueTuple](../Gapotchenko.FX.ValueTuple)

Symbol ✱ denotes an advanced module.

Or take a look at the [full list of modules](..#available-modules).
