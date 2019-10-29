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
    }
}
