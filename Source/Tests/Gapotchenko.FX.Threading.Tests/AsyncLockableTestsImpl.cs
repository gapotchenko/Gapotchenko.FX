// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Tasks;
using Gapotchenko.FX.Threading.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.ExceptionServices;

namespace Gapotchenko.FX.Threading.Tests;

readonly struct AsyncLockableTestsImpl
{
    public AsyncLockableTestsImpl(Func<IAsyncLockable> createLockableFunc)
    {
        m_CreateLockableFunc = createLockableFunc;
    }

    readonly Func<IAsyncLockable> m_CreateLockableFunc;

    IAsyncLockable CreateLockable() => m_CreateLockableFunc();

    // ----------------------------------------------------------------------

    public void Constuction()
    {
        var lockable = CreateLockable();
        Assert.AreEqual(lockable is IAsyncRecursiveLockable, lockable.IsRecursive);
        Assert.IsFalse(lockable.IsLocked);
        if (lockable is IAsyncRecursiveLockable recursiveLockable)
            Assert.IsFalse(recursiveLockable.IsLockHeld);
    }

    // ----------------------------------------------------------------------

    public void Lock_Nesting()
    {
        var lockable = CreateLockable();

        if (lockable is IAsyncRecursiveLockable recursiveLockable)
        {
            recursiveLockable.Lock();
            Assert.IsTrue(recursiveLockable.IsLocked);
            Assert.IsTrue(recursiveLockable.IsLockHeld);

            Assert.IsTrue(recursiveLockable.TryLock());
            Assert.IsTrue(recursiveLockable.IsLocked);
            Assert.IsTrue(recursiveLockable.IsLockHeld);

            recursiveLockable.Unlock();
            Assert.IsTrue(recursiveLockable.IsLocked);
            Assert.IsTrue(recursiveLockable.IsLockHeld);

            recursiveLockable.Unlock();
            Assert.IsFalse(recursiveLockable.IsLocked);
            Assert.IsFalse(recursiveLockable.IsLockHeld);
        }
        else
        {
            lockable.Lock();
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsFalse(lockable.TryLock());
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
        }
    }

    // ----------------------------------------------------------------------

    public void Lock_Rollback()
    {
        var lockable = CreateLockable();

        bool wasCanceled = false;
        try
        {
            lockable.Lock(new CancellationToken(true));
        }
        catch (OperationCanceledException)
        {
            wasCanceled = true;
        }
        Assert.IsTrue(wasCanceled);

        Assert.IsFalse(lockable.IsLocked);

        Assert.IsTrue(lockable.TryLock());
        Assert.IsTrue(lockable.IsLocked);
    }

    // ----------------------------------------------------------------------

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
                lockable.Lock();
                lockEvent.Set();
                unlockEvent.Wait();
                lockable.Unlock();
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

            Assert.IsTrue(lockable.IsLocked);
            Assert.IsFalse(lockable.TryLock());
        }
        finally
        {
            unlockEvent.Set();
            await lockerTask;
        }

        Assert.IsFalse(lockable.IsLocked);

        Assert.IsTrue(lockable.TryLock());
        Assert.IsTrue(lockable.IsLocked);

        lockable.Unlock();
        Assert.IsFalse(lockable.IsLocked);
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
                    cts.Cancel();

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
                        Assert.IsFalse(recursiveLockable.IsLockHeld, GetTPText("TP1"));

                    await lockAsyncFunc(lockable, cancellationToken);

                    Assert.IsTrue(lockable.IsLocked, GetTPText("TP2"));
                    if (recursiveLockable != null)
                        Assert.IsTrue(recursiveLockable.IsLockHeld, GetTPText("TP3"));

                    string GetIterativeTPText(string id, int i) => GetTPText($"{id} #{i}");

                    for (int i = 0; i < recursionDepth; ++i)
                    {
                        Assert.IsTrue(await lockable.TryLockAsync(0, cancellationToken), GetIterativeTPText("TP4", i));
                        Assert.IsTrue(lockable.IsLocked, GetIterativeTPText("TP5", i));
                        if (recursiveLockable != null)
                            Assert.IsTrue(recursiveLockable.IsLockHeld, GetIterativeTPText("TP6", i));
                    }

                    if (random.NextBoolean())
                    {
                        // Switch the context to verify that the lock recursion information is flowing.
                        await Task.Yield();
                    }

                    for (int i = 0; i < recursionDepth; ++i)
                    {
                        lockable.Unlock();
                        Assert.IsTrue(lockable.IsLocked, GetIterativeTPText("TP7", i));
                        if (recursiveLockable != null)
                            Assert.IsTrue(recursiveLockable.IsLockHeld, GetIterativeTPText("TP8", i));
                    }

                    if (random.NextBoolean())
                    {
                        // Switch the context to verify that the lock recursion information is flowing.
                        await Task.Yield();
                    }

                    for (int i = 0; i < recursionDepth; ++i)
                    {
                        await lockAsyncFunc(lockable, cancellationToken);
                        Assert.IsTrue(lockable.IsLocked, GetIterativeTPText("TP9", i));
                        if (recursiveLockable != null)
                            Assert.IsTrue(recursiveLockable.IsLockHeld, GetIterativeTPText("TP10", i));
                    }

                    if (random.NextBoolean())
                    {
                        // Switch the context to verify that the lock recursion information is flowing.
                        await Task.Yield();
                    }

                    for (int i = 0; i < recursionDepth; ++i)
                    {
                        lockable.Unlock();
                        Assert.IsTrue(lockable.IsLocked, GetIterativeTPText("TP11", i));
                        if (recursiveLockable != null)
                            Assert.IsTrue(recursiveLockable.IsLockHeld, GetIterativeTPText("TP12", i));
                    }

                    if (random.NextBoolean())
                    {
                        // Switch the context to verify that the lock recursion information is flowing.
                        await Task.Yield();
                    }

                    Assert.IsTrue(lockable.IsLocked, GetTPText("TP13"));
                    if (recursiveLockable != null)
                        Assert.IsTrue(recursiveLockable.IsLockHeld, GetTPText("TP14"));

                    lockable.Unlock();

                    if (recursiveLockable != null)
                        Assert.IsFalse(recursiveLockable.IsLockHeld, GetTPText("TP15"));
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

    // ----------------------------------------------------------------------

    public async Task LockAsync_Nesting()
    {
        var lockable = CreateLockable();

        if (lockable is IAsyncRecursiveLockable recursiveLockable)
        {
            await recursiveLockable.LockAsync();
            Assert.IsTrue(recursiveLockable.IsLocked);
            Assert.IsTrue(recursiveLockable.IsLockHeld);

            Assert.IsTrue(recursiveLockable.TryLock());
            Assert.IsTrue(recursiveLockable.IsLocked);
            Assert.IsTrue(recursiveLockable.IsLockHeld);

            // Switch the context to verify that the lock recursion information is flowing.
            await Task.Yield();

            recursiveLockable.Unlock();
            Assert.IsTrue(recursiveLockable.IsLocked);
            Assert.IsTrue(recursiveLockable.IsLockHeld);

            recursiveLockable.Unlock();
            Assert.IsFalse(recursiveLockable.IsLocked);
            Assert.IsFalse(recursiveLockable.IsLockHeld);
        }
        else
        {
            await lockable.LockAsync();
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsFalse(lockable.TryLock());
            Assert.IsTrue(lockable.IsLocked);

            // Switch the context to verify that the lock recursion information is flowing.
            await Task.Yield();

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
        }

        // --------------------------------------------

        await VerifyLockingSemanticsAsync(
            lockable,
            (x, ct) => x.LockAsync(ct));
    }

    // ----------------------------------------------------------------------

    public async Task LockAsync_Rollback()
    {
        var lockable = CreateLockable();
        var recursiveLockable = lockable as IAsyncRecursiveLockable;

        bool wasCanceled = false;
        try
        {
            await lockable.LockAsync(new CancellationToken(true));
        }
        catch (OperationCanceledException)
        {
            wasCanceled = true;
        }
        Assert.IsTrue(wasCanceled);

        Assert.IsFalse(lockable.IsLocked);
        if (recursiveLockable != null)
            Assert.IsFalse(recursiveLockable.IsLockHeld);

        Assert.IsTrue(lockable.TryLock());
        Assert.IsTrue(lockable.IsLocked);
        if (recursiveLockable != null)
            Assert.IsTrue(recursiveLockable.IsLockHeld);
    }

    // ----------------------------------------------------------------------

    public void TryLock_Nesting() =>
        TryLock_Nesting_Core(CreateLockable(), x => x.TryLock());

    public void TryLock_TimeSpan_Nesting()
    {
        var lockable = CreateLockable();
        var timeout = TimeSpan.Zero;

        TryLock_Nesting_Core(lockable, x => x.TryLock(timeout));
        TryLock_Nesting_Core(lockable, x => x.TryLock(timeout, CancellationToken.None));
    }

    public void TryLock_Int32_Nesting()
    {
        var lockable = CreateLockable();
        var timeout = 0;

        TryLock_Nesting_Core(lockable, x => x.TryLock(timeout));
        TryLock_Nesting_Core(lockable, x => x.TryLock(timeout, CancellationToken.None));
    }

    static void TryLock_Nesting_Core(IAsyncLockable lockable, Func<IAsyncLockable, bool> tryLockFunc)
    {
        if (lockable is IAsyncRecursiveLockable recursiveLockable)
        {
            Assert.IsTrue(tryLockFunc(recursiveLockable));
            Assert.IsTrue(recursiveLockable.IsLocked);
            Assert.IsTrue(recursiveLockable.IsLockHeld);

            Assert.IsTrue(tryLockFunc(recursiveLockable));
            Assert.IsTrue(recursiveLockable.IsLocked);
            Assert.IsTrue(recursiveLockable.IsLockHeld);

            recursiveLockable.Unlock();
            Assert.IsTrue(recursiveLockable.IsLocked);
            Assert.IsTrue(recursiveLockable.IsLockHeld);

            recursiveLockable.Unlock();
            Assert.IsFalse(recursiveLockable.IsLocked);
            Assert.IsFalse(recursiveLockable.IsLockHeld);
        }
        else
        {
            Assert.IsTrue(tryLockFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsFalse(tryLockFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
        }
    }

    // ----------------------------------------------------------------------

    public void TryLock_TimeSpan_Rollback() =>
        TryLock_Rollback_Core(
            CreateLockable(),
            (x, ct) => x.TryLock(Timeout.InfiniteTimeSpan, ct));

    public void TryLock_Int32_Rollback() =>
        TryLock_Rollback_Core(
            CreateLockable(),
            (x, ct) => x.TryLock(Timeout.Infinite, ct));

    static void TryLock_Rollback_Core(IAsyncLockable lockable, Func<IAsyncLockable, CancellationToken, bool> tryLockFunc)
    {
        bool wasCanceled = false;
        try
        {
            tryLockFunc(lockable, new CancellationToken(true));
        }
        catch (OperationCanceledException)
        {
            wasCanceled = true;
        }
        Assert.IsTrue(wasCanceled);

        Assert.IsFalse(lockable.IsLocked);

        Assert.IsTrue(lockable.TryLock());
        Assert.IsTrue(lockable.IsLocked);
    }

    // ----------------------------------------------------------------------

    [TestMethod]
    public async Task TryLockAsync_TimeSpan_Nesting()
    {
        var lockable = CreateLockable();
        var timeout = TimeSpan.Zero;

        static Task LockAsync(IAsyncLockable lockable, CancellationToken cancellationToken) =>
            lockable.TryLockAsync(Timeout.InfiniteTimeSpan, cancellationToken);

        await TryLockAsync_Nesting_Core(
            lockable,
            x => x.TryLockAsync(timeout),
            LockAsync);

        await TryLockAsync_Nesting_Core(
            lockable,
            x => x.TryLockAsync(timeout, CancellationToken.None),
            LockAsync);
    }

    [TestMethod]
    public async Task TryLockAsync_Int32_Nesting()
    {
        var lockable = CreateLockable();
        var timeout = 0;

        static Task LockAsync(IAsyncLockable lockable, CancellationToken cancellationToken) =>
            lockable.TryLockAsync(Timeout.Infinite, cancellationToken);

        await TryLockAsync_Nesting_Core(
            lockable,
            x => x.TryLockAsync(timeout),
            LockAsync);

        await TryLockAsync_Nesting_Core(
            lockable,
            x => x.TryLockAsync(timeout, CancellationToken.None),
            LockAsync);
    }

    static async Task TryLockAsync_Nesting_Core(
        IAsyncLockable lockable,
        Func<IAsyncLockable, Task<bool>> tryLockAsyncFunc,
        Func<IAsyncLockable, CancellationToken, Task> lockAsyncFunc)
    {
        if (lockable is IAsyncRecursiveLockable recursiveLockable)
        {
            Assert.IsTrue(await tryLockAsyncFunc(recursiveLockable));
            Assert.IsTrue(recursiveLockable.IsLocked);
            Assert.IsTrue(recursiveLockable.IsLockHeld);

            Assert.IsTrue(await tryLockAsyncFunc(recursiveLockable));
            Assert.IsTrue(recursiveLockable.IsLocked);
            Assert.IsTrue(recursiveLockable.IsLockHeld);

            // Switch the context to verify that the lock recursion information is flowing.
            await Task.Yield();

            recursiveLockable.Unlock();
            Assert.IsTrue(recursiveLockable.IsLocked);
            Assert.IsTrue(recursiveLockable.IsLockHeld);

            recursiveLockable.Unlock();
            Assert.IsFalse(recursiveLockable.IsLocked);
            Assert.IsFalse(recursiveLockable.IsLockHeld);
        }
        else
        {
            Assert.IsTrue(await tryLockAsyncFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsFalse(await tryLockAsyncFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            // Switch the context to verify that the lock recursion information is flowing.
            await Task.Yield();

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
        }

        // --------------------------------------------

        await VerifyLockingSemanticsAsync(lockable, lockAsyncFunc);
    }

    // ----------------------------------------------------------------------

    public Task TryLockAsync_TimeSpan_Rollback() =>
        TryLockAsync_Rollback_Core(
            CreateLockable(),
            (x, ct) => x.TryLockAsync(Timeout.InfiniteTimeSpan, ct));

    public Task TryLockAsync_Int32_Rollback() =>
        TryLockAsync_Rollback_Core(
            CreateLockable(),
            (x, ct) => x.TryLockAsync(Timeout.Infinite, ct));

    static async Task TryLockAsync_Rollback_Core(IAsyncLockable lockable, Func<IAsyncLockable, CancellationToken, Task<bool>> tryLockAsyncFunc)
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

        Assert.IsFalse(lockable.IsLocked);

        Assert.IsTrue(lockable.TryLock());
        Assert.IsTrue(lockable.IsLocked);
    }

    // ----------------------------------------------------------------------

    public void Unlock_NonLocked()
    {
        var lockable = CreateLockable();
        Assert.ThrowsException<SynchronizationLockException>(lockable.Unlock);
    }
}
