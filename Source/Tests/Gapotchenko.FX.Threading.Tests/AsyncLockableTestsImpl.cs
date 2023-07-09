using Gapotchenko.FX.Threading.Tasks;
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
        Assert.IsFalse(lockable.IsLocked);
    }

    // ----------------------------------------------------------------------

    public void Lock_Nesting()
    {
        var lockable = CreateLockable();

        if (lockable.IsRecursive)
        {
            lockable.Lock();
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsTrue(lockable.TryLock());
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
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
#if NET6_0_OR_GREATER
        await VerifyLockingSemanticsAsync_Concurrency(lockable, lockAsyncFunc);
#endif
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

#if NET6_0_OR_GREATER
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

        const int iterationCount = 1000;
        for (int iteration = 1; iteration <= iterationCount; ++iteration)
        {
            var task =
                Parallel.ForEachAsync(
                    Enumerable.Range(1, ThreadingCapabilities.LogicalProcessorCount),
                    cts.Token,
                    async (_, cancellationToken) =>
                    {
                        try
                        {
                            await ThreadEntry(
                                lockable, getRecursionLevel(), cancellationToken, lockAsyncFunc,
                                iteration, iterationCount);
                        }
                        catch (Exception e) when (!e.IsControlFlowException())
                        {
                            // Capture the first exception.
                            exceptionInfo ??= ExceptionDispatchInfo.Capture(e);

                            // Cancel the remaining tasks.
                            cts.Cancel();

                            throw;
                        }

                        static async Task ThreadEntry(
                            IAsyncLockable lockable,
                            int recursionDepth,
                            CancellationToken cancellationToken,
                            Func<IAsyncLockable, CancellationToken, Task> lockAsyncFunc,
                            int iteration, int iterationCount)
                        {
                            await lockAsyncFunc(lockable, cancellationToken);
                            Assert.IsTrue(lockable.IsLocked, "TP1");

                            string GetTestPointText(string id, int i) => $"{id} #{i} ♽ {iteration}/{iterationCount}";

                            for (int i = 0; i < recursionDepth; ++i)
                            {
                                Assert.IsTrue(await lockable.TryLockAsync(0, cancellationToken), GetTestPointText("TP2", i));
                                Assert.IsTrue(lockable.IsLocked, GetTestPointText("TP3", i));
                            }

                            // Switch the context to verify that the lock recursion information is flowing.
                            await Task.Yield();

                            for (int i = 0; i < recursionDepth; ++i)
                            {
                                lockable.Unlock();
                                Assert.IsTrue(lockable.IsLocked, GetTestPointText("TP4", i));
                            }

                            // Switch the context to verify that the lock recursion information is flowing.
                            await Task.Yield();

                            for (int i = 0; i < recursionDepth; ++i)
                            {
                                await lockAsyncFunc(lockable, cancellationToken);
                                Assert.IsTrue(lockable.IsLocked, GetTestPointText("TP5", i));
                            }

                            // Switch the context to verify that the lock recursion information is flowing.
                            await Task.Yield();

                            for (int i = 0; i < recursionDepth; ++i)
                            {
                                lockable.Unlock();
                                Assert.IsTrue(lockable.IsLocked, GetTestPointText("TP6", i));
                            }

                            lockable.Unlock();
                        }
                    });

            try
            {
                // Some threads may be in a non-cancelable state due to bugs in the code being tested.
                // Hence canceling the task as a whole here, just in case.
                // Otherwise, the test may just hang.
                await task.WaitAsync(cts.Token);
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
#endif

    // ----------------------------------------------------------------------

    public async Task LockAsync_Nesting()
    {
        var lockable = CreateLockable();

        if (lockable.IsRecursive)
        {
            await lockable.LockAsync();
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsTrue(lockable.TryLock());
            Assert.IsTrue(lockable.IsLocked);

            // Switch the context to verify that the lock recursion information is flowing.
            await Task.Yield();

            lockable.Unlock();
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
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

        Assert.IsTrue(lockable.TryLock());
        Assert.IsTrue(lockable.IsLocked);
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
        if (lockable.IsRecursive)
        {
            Assert.IsTrue(tryLockFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsTrue(tryLockFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
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
        if (lockable.IsRecursive)
        {
            Assert.IsTrue(await tryLockAsyncFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            Assert.IsTrue(await tryLockAsyncFunc(lockable));
            Assert.IsTrue(lockable.IsLocked);

            // Switch the context to verify that the lock recursion information is flowing.
            await Task.Yield();

            lockable.Unlock();
            Assert.IsTrue(lockable.IsLocked);

            lockable.Unlock();
            Assert.IsFalse(lockable.IsLocked);
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
