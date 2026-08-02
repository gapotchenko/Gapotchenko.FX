// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Harness;

using System.Diagnostics;

static class StressExercise
{
    public static void Run()
    {
        ExerciseUnwindIntrinsic();
    }

    static void ExerciseUnwindIntrinsic()
    {
        Assert.That.FirstIntrinsicInvocationIsOK(StressOperations.UnwindIntrinsic());
        Assert.AreEqual(OperationResults.NonLeafIntrinsic, StressOperations.UnwindIntrinsic());

        using var cancellation = new CancellationTokenSource();
        var workers = StartWorkers(cancellation.Token);

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var delay = TimeSpan.FromSeconds(3);
            while (stopwatch.Elapsed < delay)
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                Thread.Yield();
            }
        }
        finally
        {
            cancellation.Cancel();
        }

        Task.WaitAll(workers);
    }

    static Task[] StartWorkers(CancellationToken cancellationToken)
    {
        var workers = new Task[Math.Max(2, Math.Min(Environment.ProcessorCount, 8))];
        for (int i = 0; i < workers.Length; ++i)
        {
            workers[i] = Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Assert.AreEqual(
                        OperationResults.NonLeafIntrinsic,
                        StressOperations.UnwindIntrinsic());
                    Thread.Yield();
                }
            });
        }
        return workers;
    }
}
