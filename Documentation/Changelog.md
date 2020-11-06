# What's New in Gapotchenko.FX

## 2020

### Gapotchenko FX 2020.2

Release date: not released yet

- Improved LINQ memoization operation by avoiding the sync lock when the whole collection is fully retrieved

### Gapotchenko.FX 2020.1

Release date: November 5, 2020

- Added support for .NET 5.0 target framework
- Introduced `Gapotchenko.FX.AppModel.Information` module that allows to programmatically retrieve information about the app
- Introduced `Gapotchenko.FX.Console` module that provides virtual terminal functionality, console traits and a powerful `MoreTextWriter` primitive for improved user friendliness of your console apps
- Added nullability annotations for public module APIs
- Added `System.MathF` polyfill
- Added polyfills for nullable annotation attributes
- Added polyfill for init-only property setters
- Added `Fn.Ignore(value)` function that ignores a specified value for languages that do not have a built-in equivalent. A typical usage is to fire and forget a parallel `Task` without producing a compiler warning
- Added `Process.GetImageFileName()` method that allows to retrieve the file name of a running process without security limitations imposed by the host OS
- Added `bool ISet.AddRange(collection)` extension method that allows to add elements to a set in bulk
- Added `CommandLine.EscapeArgument` and `CommandLine.EscapeFileName` methods
- .NET Framework 4.0 target is retired. The minimal supported .NET Framework version is 4.5
- Fixed issue with ambiguous match of `IsNullOrEmpty` polyfill method of `HashSet<T>` type that occurred in .NET 4.6+, .NET Standard 2.0+ and .NET Core 2.0+ target frameworks
- Fixed issue with ambiguous match of `ToHashSet` polyfill method of `IEnumerable<T>` type that occurred in .NET 4.7.2+ target frameworks

## 2019

### Gapotchenko.FX 2019.3

Release date: November 4, 2019

- Added support for .NET Core 3.0 target framework
- Introduced `Sequential` and `DebuggableParallel` primitives in `Gapotchenko.FX.Threading` module.
Both primitives constitute drop-in replacements for `System.Threading.Tasks.Parallel` and are useful for debugging purposes
- Added `CommandLine.OemEncoding` property that gets OEM encoding used by Windows command line and console applications
- Added ability to create LINQ expressions from functions via `Gapotchenko.FX.Fn` primitive
- Added `IndexOf(IEnumerable<T> source, IEnumerable<T> value)` and `IndexOf(IEnumerable<T> source, IEnumerable<T> value, IEqualityComparer<T> comparer)` LINQ methods
- Implemented polyfill for `Enumerable.ToHashSet<T>` operation
- Implemented polyfills for `BitConverter.SingleToInt32Bits` and `Int32BitsToSingle` operations
- Implemented polyfills to the future for `WaitForExit(CancellationToken)` and `WaitForExit(int, CancellationToken)` methods of `System.Diagnostics.Process` type
- Implemented polyfill for `SwitchExpressionException`
- `Empty.Task` is not suggested by the code editor when it is natively provided by the host platform
- Fixed issue with ambiguous match of `Append` and `Prepend` polyfills for `IEnumerable<T>` type for some target frameworks
- Fixed issue with binding redirects handling in `Gapotchenko.FX.Reflection.Loader` module that could lead to `StackOverflowException` under specific conditions

### Gapotchenko.FX 2019.2

Release date: August 14, 2019

- Added support for .NET Standard 2.1 target
- Added `System.Numerics.BitOperations` polyfill with `Log2` and `PopCount` hardware-accelerated operations
- Added ability to nullify specific integer values in `Empty` functional primitive
- Added ability to canonicalize a file path with `FileSystem.CanonicalizePath` method. It works as follows: the alternative directory separators are replaced with native ones; the duplicate adjacent separators are removed
- Introduced `Gapotchenko.FX.Reflection.Loader` module. It provides a versatile `AssemblyAutoLoader` type that can be used to automatically find and load assembly dependencies in various dynamic scenarios
- Introduced `Gapotchenko.FX.Drawing` module
- Introduced `Gapotchenko.FX.Data.Linq` module. Currently it provides async operations for LINQ to SQL technology
- Introduced `Gapotchenko.FX.Runtime.InteropServices.MemoryOperations` class that provides highly-optimized block operations for memory
- Introduced intrinsic compiler for hardware-accelerated operations 
- Implemented `ProcessArchitecture` and `OSArchitecture` properties in `System.Runtime.InteropServices.RuntimeInformation` polyfill
- Improved wording of `ProgramExitException` message

### Gapotchenko.FX 2019.1

Release date: March 29, 2019

- First public release
- Improved wording in summary tags
- Fixed a critical omission with `TaskBridge` namespace
