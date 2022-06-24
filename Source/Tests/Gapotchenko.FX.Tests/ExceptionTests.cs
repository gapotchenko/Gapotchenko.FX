using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Tests;

[TestClass]
public class ExceptionTests
{
    [TestMethod]
    public void Exception_ControlFlow_Basics()
    {
        Assert.IsTrue(new TaskCanceledException().IsCancellationException());
        Assert.IsTrue(new OperationCanceledException().IsCancellationException());
        Assert.IsTrue(new ThreadInterruptedException().IsCancellationException());

        Assert.IsFalse(new InvalidOperationException().IsCancellationException());
    }

    [TestMethod]
    public void Exception_ControlFlow_Nested_Homomorphic()
    {
        Assert.IsTrue(
            new AggregateException(
                new TaskCanceledException(),
                new TaskCanceledException())
            .IsCancellationException());
    }

    [TestMethod]
    public void Exception_ControlFlow_Nested_Mixed()
    {
        Assert.IsTrue(
            new AggregateException(
                new TaskCanceledException(),
                new OperationCanceledException())
            .IsCancellationException());

        Assert.IsFalse(
            new AggregateException(
                new TaskCanceledException(),
                new InvalidOperationException())
            .IsCancellationException());
    }
}
