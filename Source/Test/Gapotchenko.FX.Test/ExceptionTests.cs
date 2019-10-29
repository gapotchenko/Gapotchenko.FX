using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Test
{
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
}
