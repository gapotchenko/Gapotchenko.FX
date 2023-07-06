﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Threading.Tests.Tasks.TaskPolyfills;

partial class TaskPolyfillTests
{
    #region Settings

#pragma warning disable IDE1006 // Naming Styles
    static readonly TimeSpan TestData_Delay = TimeSpan.FromMilliseconds(50);
    static readonly TimeSpan TestData_WaitTimeout = TimeSpan.FromMilliseconds(200);
#pragma warning restore IDE1006 // Naming Styles
    const int TestData_ExpectedResult = 123;

    #endregion

    #region Utilities

    public required TestContext TestContext { get; init; }

    Task CreateDelayedTask(TimeSpan delay) => Task.Delay(delay, TestContext.CancellationTokenSource.Token);

    async Task<TResult> CreateDelayedTask<TResult>(TimeSpan delay, TResult result)
    {
        await CreateDelayedTask(delay).ConfigureAwait(false);
        return result;
    }

    Task CreateInfiniteTask() => CreateDelayedTask(Timeout.InfiniteTimeSpan);

    async Task<TResult> CreateInfiniteTask<TResult>()
    {
        await CreateInfiniteTask().ConfigureAwait(false);
        throw new InvalidOperationException("Infinite task cannot be finished.");
    }

    #endregion

    #region WaitAsync(CancellationToken)

    [TestMethod]
    public Task Task_WaitAsync_CancellationToken() =>
        Task_WaitAsync_CancellationToken_Core(
            (task, cancellationToken) => task.WaitAsync(cancellationToken));

    Task Task_WaitAsync_CancellationToken_Core(Func<Task, CancellationToken, Task> waitAsync) =>
        Task_WaitAsync_TResult_CancellationToken_Core(
            async (task, cancellationToken) =>
            {
                await waitAsync(task, cancellationToken);
                Assert.IsTrue(task.IsCompleted);
                return task.Result;
            },
            TestData_ExpectedResult);

    [TestMethod]
    public Task Task_WaitAsync_TResult_CancellationToken() =>
        Task_WaitAsync_TResult_CancellationToken_Core(
            (task, cancellationToken) => task.WaitAsync(cancellationToken),
            TestData_ExpectedResult);

    async Task Task_WaitAsync_TResult_CancellationToken_Core<TResult>(
        Func<Task<TResult>, CancellationToken, Task<TResult>> waitAsync,
        TResult expectedResult)
    {
        var completedTask = Task.FromResult(expectedResult);
        Assert.AreEqual(expectedResult, await waitAsync(completedTask, new CancellationTokenSource(TestData_WaitTimeout).Token));
        Assert.AreEqual(expectedResult, await waitAsync(completedTask, new CancellationToken(true)));

        var infiniteTask = CreateInfiniteTask<TResult>();
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () => await waitAsync(infiniteTask, new CancellationTokenSource(TestData_WaitTimeout).Token));
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () => await waitAsync(infiniteTask, new CancellationToken(true)));

        var delayedTask = CreateDelayedTask(TestData_Delay, expectedResult);
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () => await waitAsync(delayedTask, new CancellationToken(true)));
        Assert.AreEqual(expectedResult, await waitAsync(delayedTask, new CancellationTokenSource(TestData_WaitTimeout).Token));
        Assert.IsTrue(delayedTask.IsCompleted);
    }

    #endregion

    #region WaitAsync(TimeSpan)

    [TestMethod]
    public Task Task_WaitAsync_TimeSpan() =>
        Task_WaitAsync_TimeSpan_Core(
            (task, timeout) => task.WaitAsync(timeout));

    Task Task_WaitAsync_TimeSpan_Core(Func<Task, TimeSpan, Task> waitAsync) =>
        Task_WaitAsync_TResult_TimeSpan_Core(
            async (task, timeout) =>
            {
                await waitAsync(task, timeout);
                Assert.IsTrue(task.IsCompleted);
                return task.Result;
            },
            TestData_ExpectedResult);

    [TestMethod]
    public Task Task_WaitAsync_TResult_TimeSpan() =>
        Task_WaitAsync_TResult_TimeSpan_Core(
            (task, timeout) => task.WaitAsync(timeout),
            TestData_ExpectedResult);

    async Task Task_WaitAsync_TResult_TimeSpan_Core<TResult>(
        Func<Task<TResult>, TimeSpan, Task<TResult>> waitAsync,
        TResult expectedResult)
    {
        var completedTask = Task.FromResult(expectedResult);
        Assert.AreEqual(expectedResult, await waitAsync(completedTask, TestData_WaitTimeout));
        Assert.AreEqual(expectedResult, await waitAsync(completedTask, Timeout.InfiniteTimeSpan));
        Assert.AreEqual(expectedResult, await waitAsync(completedTask, TimeSpan.Zero));

        var infiniteTask = CreateInfiniteTask<TResult>();
        await Assert.ThrowsExceptionAsync<TimeoutException>(async () => await waitAsync(infiniteTask, TestData_WaitTimeout));
        await Assert.ThrowsExceptionAsync<TimeoutException>(async () => await waitAsync(infiniteTask, TimeSpan.Zero));

        var delayedTask = CreateDelayedTask(TestData_Delay, expectedResult);
        await Assert.ThrowsExceptionAsync<TimeoutException>(async () => await waitAsync(delayedTask, TimeSpan.Zero));
        Assert.AreEqual(expectedResult, await waitAsync(delayedTask, TestData_WaitTimeout));
        Assert.IsTrue(delayedTask.IsCompleted);
    }

    [TestMethod]
    public Task Task_WaitAsync_TimeSpan_ThrowsOnInvalidValue() =>
        Task_WaitAsync_TimeSpan_ThrowsOnInvalidValue_Core(
            (task, timeout) => task.WaitAsync(timeout));

    Task Task_WaitAsync_TimeSpan_ThrowsOnInvalidValue_Core(Func<Task, TimeSpan, Task> waitAsync) =>
        Task_WaitAsync_TResult_TimeSpan_ThrowsOnInvalidValue_Core(
            async (task, timeout) =>
            {
                await waitAsync(task, timeout);
                Assert.IsTrue(task.IsCompleted);
                return task.Result;
            },
            TestData_ExpectedResult);

    [TestMethod]
    public Task Task_WaitAsync_TResult_TimeSpan_ThrowsOnInvalidValue() =>
        Task_WaitAsync_TResult_TimeSpan_ThrowsOnInvalidValue_Core(
            (task, timeout) => task.WaitAsync(timeout),
            TestData_ExpectedResult);

    async Task Task_WaitAsync_TResult_TimeSpan_ThrowsOnInvalidValue_Core<TResult>(
        Func<Task<TResult>, TimeSpan, Task<TResult>> waitAsync,
        TResult expectedResult)
    {
        var task = Task.FromResult(expectedResult);
        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => waitAsync(task, TimeSpan.FromMilliseconds(-2)));
        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => waitAsync(task, TimeSpan.MaxValue));
    }

    #endregion

    #region WaitAsync(TimeSpan, CancellationToken)

    [TestMethod]
    public Task Task_WaitAsync_TimeSpan_CancellationToken_TimeSpanScenario() =>
        Task_WaitAsync_TimeSpan_Core(
            (task, timeout) => task.WaitAsync(timeout, CancellationToken.None));

    [TestMethod]
    public Task Task_WaitAsync_TResult_TimeSpan_CancellationToken_TimeSpanScenario() =>
        Task_WaitAsync_TResult_TimeSpan_Core(
            (task, timeout) => task.WaitAsync(timeout, CancellationToken.None),
            TestData_ExpectedResult);

    [TestMethod]
    public Task Task_WaitAsync_TimeSpan_CancellationToken_CancellationTokenScenario() =>
        Task_WaitAsync_CancellationToken_Core(
            (task, cancellationToken) => task.WaitAsync(Timeout.InfiniteTimeSpan, cancellationToken));

    [TestMethod]
    public Task Task_WaitAsync_TResult_TimeSpan_CancellationToken_CancellationTokenScenario() =>
        Task_WaitAsync_TResult_CancellationToken_Core(
            (task, cancellationToken) => task.WaitAsync(Timeout.InfiniteTimeSpan, cancellationToken),
            TestData_ExpectedResult);

    [TestMethod]
    public Task Task_WaitAsync_TimeSpan_CancellationToken_ThrowsOnInvalidTimeSpan() =>
        Task_WaitAsync_TimeSpan_ThrowsOnInvalidValue_Core(
            (task, timeout) => task.WaitAsync(timeout, CancellationToken.None));

    [TestMethod]
    public Task Task_WaitAsync_TResult_TimeSpan_CancellationToken_ThrowsOnInvalidTimeSpan() =>
        Task_WaitAsync_TResult_TimeSpan_ThrowsOnInvalidValue_Core(
            (task, timeout) => task.WaitAsync(timeout, CancellationToken.None),
            TestData_ExpectedResult);

    #endregion
}
