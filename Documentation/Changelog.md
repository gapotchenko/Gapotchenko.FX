# What's New in Gapotchenko.FX

## 2026

### Gapotchenko FX 2026.1

Release date: not released yet

- Added string parsing functionality for `Interval<T>` and `ValueInterval<T>` types
- Added `SemanticVersion.Parse` and `SemanticVersion.TryParse` method overloads accepting `ReadOnlySpan<char>` parameters
- Improved `SemanticVersion` type converter
- Polyfills:
    - Added `System.IO.Stream` polyfill methods: `Read(Span<byte>)`, `Write(ReadOnlySpan<byte>)`
    - Added `System.IO.TextWriter` polyfill methods: `Write(ReadOnlySpan<char>)`, `WriteLine(ReadOnlySpan<char>)`, `WriteAsync(ReadOnlyMemory<char>, CancellationToken)`, and `WriteLineAsync(ReadOnlyMemory<char>, CancellationToken)`

## 2025

### Gapotchenko FX 2025.1

Release date: December 25, 2025

- Support for .NET 10.0 target framework
- Introduced `Gapotchenko.FX.IO.Vfs` module that provides the concept of a virtual file system which allows to work with file systems in a unified way independently of their actual nature.
  Be it a local ZIP archive, a CD/DVD ISO image, a custom FAT12 implementation, or a remote network storage
- Improved compatibility with AOT publishing
- Improved default buffer size selection algorithm for stream block copy operations in `Gapotchenko.FX.IO` module
- `Gapotchenko.FX.Collections.Generic.Deque<T>` made compatible with collection initializers
- `Gapotchenko.FX.Threading.EvaluateOnce<T>` primitive can now be created pre-initialized with a value
- Added `EnumerableEx.Lazy(Func<IEnumerable<T>>)` method that allows to lazily initialize an `IEnumerable<T>` instance by the specified factory
- Added `Gapotchenko.FX.IO.PositionTrackingTextReader` class that can track the current position within a sequential series of characters
- Polyfills:
    - Added `Contains`, `ContainsAny`, `SequenceEqual`, `StartsWith` and `EndsWith` polyfill methods for `ReadOnlySpan<T>` type
    - Added `Path.EndsInDirectorySeparator(string?)` and `Path.EndsInDirectorySeparator(ReadOnlySpan<char>)` polyfill methods
    - `Stream.ReadExactly` and `Stream.ReadAtLeast` polyfill methods provided by the `Gapotchenko.FX.IO` module are now available for all supported target frameworks
    - Implemented polyfill `ThrowIf...` methods for `ArgumentException`, `ArgumentNullException`, `ArgumentOutOfRangeException` and `ObjectDisposedException` classes
    - Added `StringBuilder.Append(ReadOnlySpan<char>)` polyfill method
    - Added `Clear(Array)`, `Fill<T>(T[], T)` and `Fill<T>(T[], T, int, int)` polyfill methods for `Array` type
    - Added `Random.Shared` and `Random.Shuffle<T>(Span<T>)` polyfill members
    - Added `Shuffle` polyfill method for `IEnumerable<T>` type
    - Added `Enum.GetValues<TEnum>()` polyfill method
- Fixed issues:
    - Fixed issue that led to an empty string value returned by `AppInformation.Current.ExecutablePath` property for assemblies loaded from single-file bundles
    - Fixed issue that led to an incorrect value of `IAppInformation.ExecutablePath` property when the app information was requested for an assembly with its own entry point
    - Fixed issue that led to an empty string value returned by `AppInformation.Trademark` property instead of a `null` value
    - Fixed P/Invoke issues that could be potentially targeted by [WorstFit Attack](https://worst.fit/) vulnerability (applies to Windows OS only)
- Retired support for target frameworks older then .NET 6.0 and .NET Framework 4.7.2

## 2024

### Gapotchenko FX 2024.2

Release date: December 31, 2024

- Added support of `System.IFormattable` interface to `Interval<T>` and `ValueInterval<T>` types provided by `Gapotchenko.FX.Math.Intervals` module
- Added `System.Threading.Lock` support to `Gapotchenko.FX.Threading.LazyInitializerEx` primitive
- Added `System.Threading.Lock` support to `Gapotchenko.FX.Threading.EvaluateOnce<T>` primitive
- Added `System.Threading.Lock` support to `Gapotchenko.FX.Threading.ExecuteOnce` primitive
- `Gapotchenko.FX.Reflection.Loader` module made as lazy as possible to avoid chicken and egg pitfalls
- Added `Gapotchenko.FX.IOptional` interface for `Gapotchenko.FX.Optional<T>` type to allow an untyped introspection
- Implemented memory span-based operations in `Gapotchenko.FX.IO.FragmentedMemoryStream` type
- Polyfills:
    - Added polyfill for `System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute` type
    - Added `Zip(first, second)` and `Zip(first, second, third)` polyfill methods for `IEnumerable<T>` type
- Fixed issues:
    - Fixed issue with nested `Gapotchenko.FX.Optional<T>` values that could occur during implicit value conversion to `Gapotchenko.FX.Optional<object>`
    - Fixed issue in `Gapotchenko.FX.Reflection.Loader` module that was caused by not taking into account a base directory of the assembly that had probing paths specified in its `App.config` file

### Gapotchenko FX 2024.1

Release date: November 10, 2024

- Added support for .NET 8.0 and .NET 9.0 target frameworks
- Introduced primitives for interval arithmetic represented by `Interval<T>` and `ValueInterval<T>` types provided by `Gapotchenko.FX.Math.Intervals` module
- Introduced `Gapotchenko.FX.Collection.Generic.Deque<T>` primitive representing a linear collection that supports element insertion and removal at both ends with O(1) algorithmic complexity
- Added ability to choose between lowercase or uppercase text output of a case-insensitive data encoding by using `DataEncodingOptions.Lowercase` and `DataEncodingOptions.Uppercase` flags
- Added ability to create streams over contiguous memory regions represented by `System.Memory` and `System.ReadOnlyMemory` objects by using `ToStream` extension method provided by `Gapotchenko.FX.Memory` module
- Added `Gapotchenko.FX.Memory.SpanEqualityComparer` class that allows to compare read-only spans and calculate their hash codes
- `AssemblyAutoLoader` now automatically handles probing paths defined by assembly binding redirects
- Added new `InsertSubpath` and `EntryExists` methods to `FileSystem` class provided by `Gapotchenko.FX.IO` module
- Added `ReifyCollection` LINQ extension method for `IEnumerable<T>` which allows you to get a read-only view on a sequence of elements
- Added ability to retrieve connected components of a graph by using `Graph<T>.ConnectedComponents` property
- Added ability to supply an additional cancellation token to `Gapotchenko.FX.Threading.Tasks.TaskBridge.Execute` method
- Added a functional facility that implements a [pipe operator](https://github.com/dotnet/csharplang/discussions/96) concept.
  The facility is provided in the form of `PipeTo` extension method that resides in `Gapotchenko.FX.Linq.Operators` namespace provided by `Gapotchenko.FX.Linq` module.
  It allows you to have a pipe operator functionality in .NET languages that do not natively provide pipe operators yet
- Use hardware-accelerated CRC-32C checksum algorithm implementation when it is available
- Deprecated `Gapotchenko.FX.Math.Topology` module in favor of a formalized `Gapotchenko.FX.Math.Graphs` module
- Polyfills:
  - Added polyfill for required properties introduced in C# 11.0
  - Added polyfills for `System.Range` and `System.Index` types. They are used by the C# compiler to support the range syntax
  - Added `ExceptBy`, `IntersectBy` and `UnionBy` LINQ polyfills for `IEnumerable<T>`
  - Added `Order` and `OrderDescending` LINQ polyfills for `IEnumerable<T>`
  - Added `Chunk` LINQ polyfill for `IEnumerable<T>`
  - Added `EndsWith` LINQ polyfill for `IEnumerable<T>`
  - Added polyfill for `System.ArraySegment<T>.Slice` method
  - Added polyfill for `System.IO.Path.GetRelativePath` method
  - Added polyfill for `System.IO.Path.TrimEndingDirectorySeparator` method
  - Added polyfills for `ReadExactly` and `ReadAtLeast` methods of `System.IO.Stream` type
  - Added polyfill for `System.Char.Equals(System.Char, System.StringComparison)` method
  - Added polyfill for `System.String.Contains(System.Char, System.StringComparison)` method
  - Added polyfill for `System.String.GetHashCode(System.StringComparison)` method
  - Added polyfills for `Split` and `SplitAny` methods of `System.ReadOnlySpan<char>` type
  - Added polyfill for `ReadSpan` method of `System.Runtime.InteropServices.SafeBuffer` type
  - Added polyfill for `System.Runtime.CompilerServices.CallerArgumentExpressionAttribute` type
  - Added polyfills for `System.Threading.Tasks.Task.WaitAsync` and `System.Threading.Tasks.Task<TResult>.WaitAsync` methods
  - Added polyfill for `System.Diagnostics.StackTraceHiddenAttribute` type
  - Added polyfill for `System.Diagnostics.UnreachableException` type
  - Added polyfill for `System.Collections.Generic.Queue<T>.TryDeque` method
  - Added polyfill for `System.Collections.Generic.OrderedDictionary<TKey, TValue>` type
  - Added polyfill for `System.IO.Path.Join` method
  - Added polyfills for `System.Math.BitIncrement` and `System.Math.BitDecrement` methods
  - Added polyfill for `System.Threading.Lock` type
  - Removed `System.HashCode` polyfill implementation in favor of `Microsoft.Bcl.HashCode` package
- .NET Framework 4.6 support is retired. The minimal supported version of .NET Framework is 4.6.1
- Fixed issues:
  - Fixed case-sensitivity of a text data encoding padding character. This is important for data encodings that use custom padding characters
  - Fixed issue in `Gapotchenko.FX.Collections.Generic.AssociativeArray<TKey, TValue>` type with accessing `IEnumerable<T>.Current` property without checking the result of a prior call to `MoveNext` method
  - Fixed assembly name comparison bug in `Gapotchenko.FX.Reflection.Loader` module
  - Fixed a bug with the `null` key in `IDictionary.Remove` method of `AssociativeArray<TKey, TValue>` type
  - Fixed a bug with the `null` key in `IDictionary.Contains` method of `AssociativeArray<TKey, TValue>` type
  - Fixed a bug with the `null` key in `IDictionary.this[TKey]` getter method of `AssociativeArray<TKey, TValue>` type
  - Fixed a bug with a non-existing key handling in `IDictionary.this[TKey]` getter method of `AssociativeArray<TKey, TValue>` type

## 2022

### Gapotchenko FX 2022.2

Release date: May 1, 2022

- Introduced `Gapotchenko.FX.Data.Encoding` module that defines a wireframe for data encoding algorithms
- New `Gapotchenko.FX.Data.Encoding.Base16` module provides a ready-to-use implementation of popular data encoding algorithms belonging to Base16 family
- New `Gapotchenko.FX.Data.Encoding.Base24` module provides a ready-to-use implementation of data encoding algorithms belonging to Base24 family: Kuon Base24
- New `Gapotchenko.FX.Data.Encoding.Base32` module provides a ready-to-use implementation of popular data encoding algorithms belonging to Base32 family: Base32, base32-hex, Crockford Base 32, z-base-32
- New `Gapotchenko.FX.Data.Encoding.Base64` module provides a ready-to-use implementation of popular data encoding algorithms belonging to Base64 family: Base64, Base64 URL
- Added ability to read the command-line arguments of a running OS process
- Improved multi-platform support. Reached the functional parity among Linux, macOS, and Windows platforms  

### Gapotchenko FX 2022.1

Release date: April 7, 2022

- Added support for .NET 7.0 target framework
- Introduced `Gapotchenko.FX.Security.Cryptography` module
- Introduced `Gapotchenko.FX.Data.Integrity.Checksum` module and primitives for cyclic redundancy check (CRC) calculations.
  They are grouped by the family and provided by the corresponding modules:
  `Gapotchenko.FX.Data.Integrity.Checksum.Crc32`, `Gapotchenko.FX.Data.Integrity.Checksum.Crc16`, `Gapotchenko.FX.Data.Integrity.Checksum.Crc8`
- Improved documentation
- .NET Framework 4.5 target is retired. The minimal supported .NET Framework version is 4.6
- `Process.GetImageFileName()` extension method provided by `Gapotchenko.FX.Diagnostics.Process` module now returns `null` when a process is not associated with an image file
- Fixed issue that could lead to `System.IO.EndOfStreamException` exception in `Process.ReadEnvironmentVariables()` method provided by `Gapotchenko.FX.Diagnostics.Process` module (issue GH-2)

## 2021

### Gapotchenko FX 2021.2

Release date: December 31, 2021

- Introduced `Gapotchenko.FX.Math.Topology` module that provides `Graph<T>` type and accompanying primitives including topological sorting
- New `FileSystem.PathEquivalenceComparer` property returns the file path string comparer that takes into account path normalization and equivalence rules of the host environment
- New `AssociativeArray` key/value map primitive which is similar to `Dictionary<TKey, TValue>` but handles the whole space of key values including `null`
- New string metric functions: `DamerauLevenshteinDistance`, `HammingDistance`, `JaroDistance`, `LcsDistance`, `OsaDistance`
- Added `PriorityQueue` polyfill
- Added `IEnumerable<byte> AsEnumerable()` polyfill for `System.IO.Stream`
- Added `LongIndexOf` LINQ polyfill
- Added `TryGetNonEnumeratedCount` LINQ polyfill
- Improved support for .NET 6.0 target framework
- Improved `AssemblyAutoLoader` which can now work simultaneously with app domains and assembly load contexts
- String metric functions now take an optional `maxDistance` parameter that limits computational resources required to calculate the distance

### Gapotchenko FX 2021.1

Release date: July 6, 2021

- Added support for .NET 6.0 target framework
- Introduced `Gapotchenko.FX.Memory` module
- Introduced `Gapotchenko.FX.Math.Geometry` module
- Introduced `Gapotchenko.FX.Math.Combinatorics` module
- Added `MathEx.Clamp` function that clamps a value to the specified range
- Added `MathEx.Lerp` function that performs linear interpolation between two values by the specified coefficient
- Added `AppInformation.For(assembly)` static function that retrieves app information for a specified assembly
- Added LINQ function that simultaneously determines whether any elements of a sequence satisfy the specified conditions (`(bool, bool) IEnumerable<T>.Any(Func<T, bool> predicate1, Func<T, bool> predicate2)` with higher dimensional overloads)
- Added `ConsoleEx.ReadPassword` function for securely reading a password from the console
- Added `System.Runtime.CompilerServices.ModuleInitializerAttribute` polyfill
- Added `SkipLast` and `TakeLast` LINQ polyfills
- Added `System.Collections.Generic.ReferenceEqualityComparer` polyfill
- Added `MemberNotNullAttribute` and `MemberNotNullWhenAttribute` nullability annotation polyfills
- Added `GetValueOrDefault` polyfills for `IReadOnlyDictionary<TKey, TValue>`
- Added `TryAdd` and `Remove(key, out value)` polyfills for `IDictionary<TKey, TValue>`
- Added `System.Numerics.BitOperations.PopCount(ulong)` polyfill
- Improved performance of a thread-safe LINQ memoization
- Fixed nullability annotations for `MathEx.Min` and `MathEx.Max` functions
- Fixed nullability annotations for `LazyInitializerEx` class
- Fixed issue with UTF-8 BOM encoding returned by `CommandLine.OemEncoding` on Windows when system locale is set to UTF-8
  (cmd.exe cannot consume UTF-8 with BOM)
- Fixed potential thread safety issues that could occur on architectures with weaker memory models

## 2020

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
- Fixed issue in `Process.ReadEnvironmentVariables()` method with environment variable blocks longer than 32,768 bytes
- Fixed issue in `WebBrowser.Launch(url)` method that might cause an erroneous interpretation of `?` and `&` URL symbols by a web browser on Windows hosts

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

- Added support for .NET Standard 2.1 target framework
- Added `System.Numerics.BitOperations` polyfill with `Log2` and `PopCount` hardware-accelerated operations
- Added ability to nullify specific integer values in `Empty` functional primitive
- Added ability to canonicalize a file path with `FileSystem.CanonicalizePath` method. It works as follows: the alternative directory separators are replaced with native ones; the duplicate adjacent separators are removed
- Introduced `Gapotchenko.FX.Reflection.Loader` module. It provides a versatile `AssemblyAutoLoader` type that can be used to automatically find and load assembly dependencies in various dynamic scenarios
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
