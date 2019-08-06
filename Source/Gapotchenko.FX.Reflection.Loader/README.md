# Gapotchenko.FX.Reflection.Loader

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Reflection.Loader.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Reflection.Loader)

The module provides versatile primitives that can be used to automatically lookup and load assembly dependencies in various dynamic scenarios.

<hr/>

![The Assembly Loader](../../Documentation/Assets/the-assembly-loader.png)

Assembly loading plays a crucial role in .NET apps.
Once the app is started, .NET runtime ensures that all required assemblies are gradually loaded.

Whenever the code hits the point where a type from another assembly is used, it raises `AppDomain.AssemblyResolve` event.
The good thing is .NET comes pre-equipped with default assembly loader which does a sensible job for most applications.

However, there are situations when having a default assembly loader is just not enough.
This is where `Gapotchenko.FX.Reflection.Loader` module becomes extremely handy.

<hr/>

## Scenario #1. Load dependent assemblies from an app's outside folder

Let's take a look on example scenario.
Suppose we have `ContosoApp` installed at `C:\Program Files\ContosoApp` folder.
The folder contains a single `ContosoApp.exe` assembly which represents the main executable file of the app.

`ContosoApp.exe` has a dependency on `ContosoEngine.dll` assembly which is located at
`C:\Program Files\Common Files\Contoso\Engine` folder.
It so happens ContosoApp uses a common engine developed by the company.

Now when `ContosoApp.exe` is run it bails out with the following exception:

```
System.IO.FileNotFoundException: Could not load file or assembly 'ContosoEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies. The system cannot find the file specified.
```

It occurs because `ContoseEngine.dll` assembly is located at the outside folder,
and the default .NET assembly loader does not provide a way to cover scenarios like this.

In order to cover that scenario, a developer would subscribe to `AppDomain.CurrentDomain.AssemblyResolve` event.
Then he would come up with a custom assembly lookup and loading logic.
The thing is: that is **not** a straightforward thing to do.
Even more than that, it is full of gotchas and caveats.
And they will painfully *bite* a developer on subtle occasions, now and then.

That's why `Gapotchenko.FX.Reflection.Loader` module provides a ready to use `AssemblyAutoLoader` class that reliably covers the scenarios like that.

Here is the solution for ContosoApp:

``` csharp
using System;
using System.IO;
using Gapotchenko.FX.Reflection;

namespace ContosoApp
{
    class Program
    {
        static void Main()
        {
            // The statement below instructs Gapotchenko.FX assembly loader to use
            // 'C:\Program Files\Common Files\Contoso\Engine' folder as a probing path for
            // dependent assemblies.
            AssemblyAutoLoader.AddProbingPath(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                    @"Contoso\Engine"));
            
            Run();
        }

        static void Run()
        {
            // ...
        }
    }
}
```

## Scenario #2. Load dependent assemblies from an inner folder of an app

ContosoApp continues to evolve and now it has a dependency on `Newtonsoft.Json.dll` assembly.
A straightforward approach would be to put `Newtonsoft.Json.dll` assembly just besides `ContosoApp.exe`.


But Mr. Alberto Olivetti from Contoso's Deployment Division decided that an additional file laying near `ContosoApp.exe` would be an unwanted distraction for command line users of the app.
Mr. Olivetti tends to pay a lot of respect to his customers and wants to save their time while they are hanging around `ContosoApp.exe`.
Thus Alberto came up with a respectful solution to put all third-party assemblies to `Components` subfolder of the app.

Now how can `ContosoApp.exe` module load the required assemblies from `Components` folder?
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

The task is solved for `ContosoApp` (and every other .NET app as well).
The default .NET assembly loader can be instructed to load dependent assemblies from inner folders of an app by specifying a set of private probing paths.

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
which was already present at `C:\Program Files\Common Files\Contoso\Engine` folder.

So the AutoCAD plugin (a .DLL assembly) had to gain an ability to load the dependencies from that folder.

This is what Alberto did. He created `AssemblyLoader` class in AutoCAD plugin assembly with just one method `Activate`:

``` csharp
namespace ContosoApp.Integration.AutoCAD
{
    static class AssemblyLoader
    {
        public static void Activate()
        {
        }
    }
}
```

Alberto then ensured that `Activate` method is getting called at the early stages of a plugin lifecycle:

``` csharp
namespace ContosoApp.Integration.AutoCAD
{
    public class Plugin : AutodeskPluginBase
    {
        public override void Initialize()
        {
            AssemblyLoader.Activate();

            base.Initialize();

            // ...
        }
    }
}
```

Now Alberto had a skeleton for a proper assembly loader initialization.
The only missing part was the actual assembly loader implementation which was going to be enormous.

Thanks to the prior experience with custom assembly loading, Alberto was aware about that fancy `AssemblyAutoLoader` class provided by `Gapotchenko.FX.Reflection.Loader` package.
Then he wrote:

``` csharp
using System;
using System.IO;
using Gapotchenko.FX.Reflection;

namespace ContosoApp.Integration.AutoCAD
{
    static class AssemblyLoader
    {
        static AssemblyLoader()
        {
            // The statement below instructs Gapotchenko.FX assembly loader to use
            // 'C:\Program Files\Common Files\Contoso\Engine' folder as a probing path for
            // resolution of 'ContosoApp.Integration.AutoCAD.dll' assembly dependencies.
            AssemblyAutoLoader.AddAssembly(
                typeof(AssemblyLoader).Assembly,
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                    @"Contoso\Engine"));
        }

        public static void Activate()
        {
        }
    }
}
```

Please note how Alberto wrote an implementation inside a static constructor while leaving `Activate` method empty.
In that way, he was able to achieve a single mode of execution, where the actual assembly loader initialization takes place only once on a first call to `Activate` method.
Smart.

But even if Alberto did not create a singleton, `AssemblyAutoLoader` is smart enough to do the right job out of the box.

Now why did Alberto call `AssemblyAutoLoader.AddAssembly` method instead of `AssemblyAutoLoader.AddProbingPath`?
Both would work, actually. There is a subtle but very important difference.

`AssemblyAutoLoader.AddProbingPath` is a coarse "catch-all" method.
It would serve not only the dependencies of a given plugin assembly but would also cover the whole app domain.
Sometimes this is a beneficial behavior, like in case with the root `ContosoApp.exe` assembly.

In contrast, `AssemblyAutoLoader.AddAssembly` method provides a finer control.
It only serves the dependencies of a _specified assembly_.
It turns out that this is a much saner choice for plugins where app domain is shared among a lot of things.
In this way, assembly loaders from different plugins would not clash with each other, even when they look for a conflicting assembly dependency (it's easy to imagine that a lot of plugins would use the "same" but subtly different version of `Newtonsoft.Json`).

## Scenario #4. Automatic handling of binding redirects for a .DLL assembly

Assembly binding redirects allow to "remap" specific ranges of assembly versions.
The redirects are automatically created by build tools, and then put to corresponding `.config` files of resulting assemblies.
[(Learn more)](https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/how-to-enable-and-disable-automatic-binding-redirection)

Assembly binding redirects work well for apps, but get completely broken if you want to employ them for .DLL assemblies like plugins.
The default .NET assembly loader simply ignores `.config` files of .DLL assemblies!

`Gapotchenko.FX.Reflection.Loader` solves this. Just add the following code early at the assembly lifecycle:

``` csharp
AssemblyLoader.Activate()
```

`AssemblyLoader` implementation then goes as follows:

``` csharp
using Gapotchenko.FX.Reflection;

namespace MyPlugin
{
    static class AssemblyLoader
    {
        static AssemblyLoader()
        {
            // The statement below instructs Gapotchenko.FX assembly loader to add a specified
            // assembly to the list of sources to consider during assembly resolution process.
            // The loader automatically handles binding redirects according to a corresponding assembly
            // configuration (.config) file. If configuration file is missing then binding redirects are
            // automatically deducted according to the assembly compatibility heuristics.
            AssemblyAutoLoader.AddAssembly(typeof(AssemblyLoader).Assembly);
        }

        public static void Activate()
        {
        }
    }
}
```

There are a lot of situations when you may want this: T4 templates, MSBuild tasks, plugins, extensions etc.

## Usage

`Gapotchenko.FX.Reflection.Loader` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Reflection.Loader):

```
PM> Install-Package Gapotchenko.FX.Reflection.Loader
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
- &#x27B4; [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)
