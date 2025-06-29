// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Tasks;
using Gapotchenko.FX.Threading.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.ExceptionServices;

namespace Gapotchenko.FX.Threading.Tests;

[TestCategory("async")]
public abstract class IAsyncLockableTests : ILockableTests
{
    protected abstract IAsyncLockable CreateAsyncLockable();

    protected sealed override ILockable CreateLockable() => CreateAsyncLockable();

    // ----------------------------------------------------------------------

    [TestMethod]
    public async Task IAsyncLockable_EnterAsync_Nesting()
    {
        var lockable = CreateAsyncLockable();

        if (lockable is IAsyncRecursiveLockable recursiveLockable)
        {
            await recursiveLockable.EnterAsync();
            Assert.IsTrue(recursiveLockable.IsEntered);
            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread);

            Assert.IsTrue(recursiveLockable.TryEnter());
            Assert.IsTrue(recursiveLockable.IsEntered);
            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread);

            // Switch the context to verify that the lock recursion information is flowing.
            await Task.Yield();

            recursiveLockable.Exit();
            Assert.IsTrue(recursiveLockable.IsEntered);
            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread);

            recursiveLockable.Exit();
            Assert.IsFalse(recursiveLockable.IsEntered);
            Assert.IsFalse(recursiveLockable.IsLockedByCurrentThread);
        }
        else
        {
            await lockable.EnterAsync();
            Assert.IsTrue(lockable.IsEntered);

            Assert.IsFalse(lockable.TryEnter());
            Assert.IsTrue(lockable.IsEntered);

            // Switch the context to verify that the lock recursion information is flowing.
            await Task.Yield();

            lockable.Exit();
            Assert.IsFalse(lockable.IsEntered);
        }

        // --------------------------------------------

        await VerifyLockingSemanticsAsync(
            lockable,
            (x, ct) => x.EnterAsync(ct));
    }


    [TestMethod]
    public async Task IAsyncLockable_EnterAsync_Rollback()
    {
        var lockable = CreateAsyncLockable();
        var recursiveLockable = lockable as IAsyncRecursiveLockable;

        bool wasCanceled = false;
        try
        {
            await lockable.EnterAsync(new CancellationToken(true));
        }
        catch (OperationCanceledException)
        {
            wasCanceled = true;
        }
        Assert.IsTrue(wasCanceled);

        Assert.IsFalse(lockable.IsEntered);
        if (recursiveLockable != null)
            Assert.IsFalse(recursiveLockable.IsLockedByCurrentThread);

        Assert.IsTrue(lockable.TryEnter());
        Assert.IsTrue(lockable.IsEntered);
        if (recursiveLockable != null)
            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread);
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    public async Task IAsyncLockable_TryEnterAsync_TimeSpan_Nesting()
    {
        var lockable = CreateAsyncLockable();
        var timeout = TimeSpan.Zero;

        await TryEnterAsync_Nesting_Core(
            lockable,
            x => x.TryEnterAsync(timeout),
            LockAsync);

        await TryEnterAsync_Nesting_Core(
            lockable,
            x => x.TryEnterAsync(timeout, CancellationToken.None),
            LockAsync);

        static Task LockAsync(IAsyncLockable lockable, CancellationToken cancellationToken) =>
            lockable.TryEnterAsync(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    [TestMethod]
    public async Task IAsyncLockable_TryEnterAsync_Int32_Nesting()
    {
        var lockable = CreateAsyncLockable();
        var timeout = 0;

        await TryEnterAsync_Nesting_Core(
            lockable,
            x => x.TryEnterAsync(timeout),
            EnterAsync);

        await TryEnterAsync_Nesting_Core(
            lockable,
            x => x.TryEnterAsync(timeout, CancellationToken.None),
            EnterAsync);

        static Task EnterAsync(IAsyncLockable lockable, CancellationToken cancellationToken) =>
            lockable.TryEnterAsync(Timeout.Infinite, cancellationToken);
    }

    static async Task TryEnterAsync_Nesting_Core(
        IAsyncLockable lockable,
        Func<IAsyncLockable, Task<bool>> tryLockAsyncFunc,
        Func<IAsyncLockable, CancellationToken, Task> lockAsyncFunc)
    {
        if (lockable is IAsyncRecursiveLockable recursiveLockable)
        {
            Assert.IsTrue(await tryLockAsyncFunc(recursiveLockable));
            Assert.IsTrue(recursiveLockable.IsEntered);
            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread);

            Assert.IsTrue(await tryLockAsyncFunc(recursiveLockable));
            Assert.IsTrue(recursiveLockable.IsEntered);
            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread);

            // Switch the context to verify that the lock recursion information is flowing.
            await Task.Yield();

            recursiveLockable.Exit();
            Assert.IsTrue(recursiveLockable.IsEntered);
            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread);

            recursiveLockable.Exit();
            Assert.IsFalse(recursiveLockable.IsEntered);
            Assert.IsFalse(recursiveLockable.IsLockedByCurrentThread);
        }
        else
        {
            Assert.IsTrue(await tryLockAsyncFunc(lockable));
            Assert.IsTrue(lockable.IsEntered);

            Assert.IsFalse(await tryLockAsyncFunc(lockable));
            Assert.IsTrue(lockable.IsEntered);

            // Switch the context to verify that the lock recursion information is flowing.
            await Task.Yield();

            lockable.Exit();
            Assert.IsFalse(lockable.IsEntered);
        }

        // --------------------------------------------

        await VerifyLockingSemanticsAsync(lockable, lockAsyncFunc);
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    public Task IAsyncLockable_TryEnterAsync_TimeSpan_Rollback() =>
        TryEnterAsync_Rollback_Core(
            CreateAsyncLockable(),
            (x, ct) => x.TryEnterAsync(Timeout.InfiniteTimeSpan, ct));

    [TestMethod]
    public Task IAsyncLockable_TryEnterAsync_Int32_Rollback() =>
        TryEnterAsync_Rollback_Core(
            CreateAsyncLockable(),
            (x, ct) => x.TryEnterAsync(Timeout.Infinite, ct));

    static async Task TryEnterAsync_Rollback_Core(IAsyncLockable lockable, Func<IAsyncLockable, CancellationToken, Task<bool>> tryLockAsyncFunc)
    {
        bool wasCanceled = false;
        try
        {
            await tryLockAsyncFunc(lockable, new CancellationToken(true));
        }
        catch (OperationCanceledException)
        {
            wasCanceled = true;
        }
        Assert.IsTrue(wasCanceled);

        Assert.IsFalse(lockable.IsEntered);

        Assert.IsTrue(lockable.TryEnter());
        Assert.IsTrue(lockable.IsEntered);
    }

    // ----------------------------------------------------------------------

    #region Helpers

    static async Task VerifyLockingSemanticsAsync(
        IAsyncLockable lockable,
        Func<IAsyncLockable, CancellationToken, Task> lockAsyncFunc)
    {
        await VerifyLockingSemanticsAsync_Cancellation(lockable, lockAsyncFunc);
        await VerifyLockingSemanticsAsync_Concurrency(lockable, lockAsyncFunc);
    }

    static async Task VerifyLockingSemanticsAsync_Cancellation(
        IAsyncLockable lockable,
        Func<IAsyncLockable, CancellationToken, Task> lockAsyncFunc)
    {
        var lockEvent = new ManualResetEventSlim();
        var unlockEvent = new ManualResetEventSlim();
        var lockerTask = TaskBridge.ExecuteAsync(
            () =>
            {
                lockable.Enter();
                lockEvent.Set();
                unlockEvent.Wait();
                lockable.Exit();
            });

        try
        {
            var cts = new CancellationTokenSource();
            lockEvent.Wait();
            cts.CancelAfter(1);

            bool wasCanceled = false;
            try
            {
                await lockAsyncFunc(lockable, cts.Token);
            }
            catch (OperationCanceledException)
            {
                wasCanceled = true;
            }
            Assert.IsTrue(wasCanceled);

            Assert.IsTrue(lockable.IsEntered);
            Assert.IsFalse(lockable.TryEnter());
        }
        finally
        {
            unlockEvent.Set();
            await lockerTask;
        }

        Assert.IsFalse(lockable.IsEntered);

        Assert.IsTrue(lockable.TryEnter());
        Assert.IsTrue(lockable.IsEntered);

        lockable.Exit();
        Assert.IsFalse(lockable.IsEntered);
    }

    static async Task VerifyLockingSemanticsAsync_Concurrency(
        IAsyncLockable lockable,
        Func<IAsyncLockable, CancellationToken, Task> lockAsyncFunc)
    {
        Func<int> getRecursionLevel;
        if (lockable.IsRecursive)
        {
            var random = new Random();
            getRecursionLevel =
                () =>
                {
                    lock (random)
                        return random.Next(0, 20);
                };
        }
        else
        {
            getRecursionLevel = Fn.Default<int>;
        }

        var cts = new CancellationTokenSource();
        ExceptionDispatchInfo? exceptionInfo = null;

        // Doing this many times increases the chances of catching a concurrency bug.
        const int iterationCount = 1000;

        for (int iteration = 1; iteration <= iterationCount; ++iteration)
        {
            async Task ThreadEntry(int threadId, CancellationToken cancellationToken)
            {
                try
                {
                    // The source of pseudo-random data for test variations.
                    var random = new Random(iteration + threadId * iterationCount);

                    await Run(
                        lockable, getRecursionLevel(), cancellationToken, lockAsyncFunc,
                        iteration, iterationCount,
                        random);
                }
                catch (Exception e) when (!e.IsControlFlowException())
                {
                    // Capture the very first exception.
                    exceptionInfo ??= ExceptionDispatchInfo.Capture(e);

                    // Cancel the remaining tasks.
#if NET8_0_OR_GREATER
                    await cts.CancelAsync();
#else
                    cts.Cancel();
#endif

                    throw;
                }

                // The main function of a test thread.
                static async Task Run(
                    IAsyncLockable lockable,
                    int recursionDepth,
                    CancellationToken cancellationToken,
                    Func<IAsyncLockable, CancellationToken, Task> lockAsyncFunc,
                    int iteration, int iterationCount,
                    Random random)
                {
                    var recursiveLockable = lockable as IAsyncRecursiveLockable;

                    // Gets a test point text.
                    string GetTPText(string id) => $"{id} ♽ {iteration}/{iterationCount}";

                    if (recursiveLockable != null)
                        Assert.IsFalse(recursiveLockable.IsLockedByCurrentThread, GetTPText("TP1"));

                    await lockAsyncFunc(lockable, cancellationToken);

                    Assert.IsTrue(lockable.IsEntered, GetTPText("TP2"));
                    if (recursiveLockable != null)
                        Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread, GetTPText("TP3"));

                    string GetIterativeTPText(string id, int i) => GetTPText($"{id} #{i}");

                    for (int i = 0; i < recursionDepth; ++i)
                    {
                        Assert.IsTrue(await lockable.TryEnterAsync(0, cancellationToken), GetIterativeTPText("TP4", i));
                        Assert.IsTrue(lockable.IsEntered, GetIterativeTPText("TP5", i));
                        if (recursiveLockable != null)
                            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread, GetIterativeTPText("TP6", i));
                    }

                    if (random.NextBoolean())
                    {
                        // Switch the context to verify that the lock recursion information is flowing.
                        await Task.Yield();
                    }

                    for (int i = 0; i < recursionDepth; ++i)
                    {
                        lockable.Exit();
                        Assert.IsTrue(lockable.IsEntered, GetIterativeTPText("TP7", i));
                        if (recursiveLockable != null)
                            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread, GetIterativeTPText("TP8", i));
                    }

                    if (random.NextBoolean())
                    {
                        // Switch the context to verify that the lock recursion information is flowing.
                        await Task.Yield();
                    }

                    for (int i = 0; i < recursionDepth; ++i)
                    {
                        await lockAsyncFunc(lockable, cancellationToken);
                        Assert.IsTrue(lockable.IsEntered, GetIterativeTPText("TP9", i));
                        if (recursiveLockable != null)
                            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread, GetIterativeTPText("TP10", i));
                    }

                    if (random.NextBoolean())
                    {
                        // Switch the context to verify that the lock recursion information is flowing.
                        await Task.Yield();
                    }

                    for (int i = 0; i < recursionDepth; ++i)
                    {
                        lockable.Exit();
                        Assert.IsTrue(lockable.IsEntered, GetIterativeTPText("TP11", i));
                        if (recursiveLockable != null)
                            Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread, GetIterativeTPText("TP12", i));
                    }

                    if (random.NextBoolean())
                    {
                        // Switch the context to verify that the lock recursion information is flowing.
                        await Task.Yield();
                    }

                    Assert.IsTrue(lockable.IsEntered, GetTPText("TP13"));
                    if (recursiveLockable != null)
                        Assert.IsTrue(recursiveLockable.IsLockedByCurrentThread, GetTPText("TP14"));

                    lockable.Exit();

                    if (recursiveLockable != null)
                        Assert.IsFalse(recursiveLockable.IsLockedByCurrentThread, GetTPText("TP15"));
                }
            }

            Task controlTask;
            int processorCount = ThreadingCapabilities.LogicalProcessorCount;

#if NET6_0_OR_GREATER
            controlTask =
                Parallel.ForEachAsync(
                    Enumerable.Range(0, processorCount),
                    cts.Token,
                    (i, cancellationToken) => new ValueTask(ThreadEntry(i, cancellationToken)));
#else
            var tasks = new Task[processorCount];
            for (int i = 0; i < tasks.Length; ++i)
                tasks[i] = ThreadEntry(i, cts.Token);
            controlTask = Task.WhenAll(tasks);
#endif

            try
            {
                // Some threads may be in a non-cancelable state due to bugs in the code that is being tested.
                // Hence the control task is canceled as a whole here, just in case.
                // Otherwise, the test may just hang forever.
                await controlTask.WaitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                if (exceptionInfo != null)
                    exceptionInfo.Throw();
                else
                    throw;
            }
        }
    }

    #endregion
}
