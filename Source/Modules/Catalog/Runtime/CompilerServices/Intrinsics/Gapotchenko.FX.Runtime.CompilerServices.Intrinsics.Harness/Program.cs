// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Harness;

static class Program
{
    static int Main(string[] args)
    {
        try
        {
            Run(args);
            return 0;
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            var error = Console.Error;
            error.Write("Error: ");
            error.Write(e);
            Console.ResetColor();
            error.WriteLine();
            return 1;
        }
    }

    static void Run(IReadOnlyList<string> args)
    {
        ShowEnvironmentInfo();
        InitializeLogging();
        Exercise();
    }

    static void ShowEnvironmentInfo()
    {
        Console.WriteLine("OS: {0}", RuntimeInformation.OSDescription);
        Console.WriteLine("Process architecture: {0}", RuntimeInformation.ProcessArchitecture);
    }

    static void InitializeLogging()
    {
#if NET
        var consoleListener = new ConsoleTraceListener
        {
            Name = "console"
        };

        TraceSource.Initializing += (_, e) =>
        {
            var source = e.TraceSource;

            if (source.Name is "Gapotchenko.FX.Runtime.CompilerServices.Intrinsics")
            {
                source.Switch.Level = SourceLevels.Verbose;
                source.Listeners.Add(consoleListener);
                e.WasInitialized = true;
            }
        };
#endif
    }

    static void Exercise()
    {
        DoExercise(() => NormalExercise(), "Normal exercise");
        DoExercise(() => EdgeCaseExercise(), "Edge-case exercise");
    }

    static void DoExercise(Action action, string title)
    {
        Console.WriteLine("{0}: in progress...", title);
        try
        {
            action();
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("{0}: FAIL", title);
            Console.ResetColor();
            Console.WriteLine();
            throw;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("{0}: PASS", title);
        Console.ResetColor();
        Console.WriteLine();
    }

    static void NormalExercise()
    {
        Assert.AreEqual(0, NormalOperations.Log2_Intrinsic(0));
        Assert.AreEqual(4, NormalOperations.Log2_Intrinsic(31));
        Assert.AreEqual(5, NormalOperations.Log2_Intrinsic(32));
        Assert.AreEqual(28, NormalOperations.Log2_Intrinsic(323483272));
    }

    static void EdgeCaseExercise()
    {
        // Long intrinsic

        int result = EdgeCaseOperations.LongIntrinsic();
        Assert.IsTrue(
            result is EdgeCaseOperations.ManagedResult or EdgeCaseOperations.IntrinsicResult,
            "The first intrinsic invocation returned an unexpected value.");

        // When initialization occurs inside the first managed invocation, that active frame
        // finishes by executing the original body. Every later invocation must be redirected.
        for (int i = 1; i < 100_000; ++i)
            Assert.AreEqual(EdgeCaseOperations.IntrinsicResult, EdgeCaseOperations.LongIntrinsic());

        // Long frame activation

        result = EdgeCaseOperations.LongFrameActivation.LongFrameIntrinsic();
        Assert.IsTrue(
            result is EdgeCaseOperations.ManagedResult or EdgeCaseOperations.IntrinsicResult,
            "The first intrinsic invocation returned an unexpected value.");

        for (int i = 1; i < 100_000; ++i)
        {
            Assert.AreEqual(
                EdgeCaseOperations.IntrinsicResult,
                EdgeCaseOperations.LongFrameActivation.LongFrameIntrinsic());
        }
    }
}
