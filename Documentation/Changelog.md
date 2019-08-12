# What's New in Gapotchenko.FX

## Gapotchenko.FX 2019.2

There is no official release yet, the version is in progress. See the [`new`](../../../tree/new) repository branch for more details.

- Intrinsic compiler for hardware-accelerated operations 
- Added `System.Numerics.BitOperations` polyfill with `Log2` and `PopCount` hardware-accelerated operations
- Added ability to nullify specific integer values in `Empty` functional primitive
- Added ability to canonicalize a file path with `FileSystem.CanonicalizePath` method. It works as follows: the alternative directory separators are replaced with native ones; the duplicate adjacent separators are removed
- Introduced `Gapotchenko.FX.Reflection.Loader` module. It provides a versatile `AssemblyAutoLoader` type that can be used to automatically find and load assembly dependencies in various dynamic scenarios
- Introduced `Gapotchenko.FX.Drawing` module
- Introduced `Gapotchenko.FX.Data.Linq` module. Currently it provides async operations for LINQ to SQL technology
- Introduced `Gapotchenko.FX.Runtime.InteropServices.MemoryOperations` class that provides highly-optimized interoperability operations for memory
- Implemented `ProcessArchitecture` and `OSArchitecture` properties in `System.Runtime.InteropServices.RuntimeInformation` polyfill
- Improved wording of `ProgramExitException` message


## Gapotchenko.FX 2019.1

Release date: March 29, 2019

- First public release
- Improved wording in summary tags
- Fixed a critical omission with `TaskBridge` namespace
