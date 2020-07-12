using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Gapotchenko.FX.Diagnostics.Process.Test
{
    using Process = System.Diagnostics.Process;

    [TestClass]
    public class ProcessEnvironmentTests
    {
        [TestMethod]
        public void ProcEnv_Read()
        {
            var process = new Process();

            var psi = process.StartInfo;
            psi.FileName = "dotnet";
            psi.Arguments = "fsi";

            string value = Guid.NewGuid().ToString("D");
            psi.EnvironmentVariables["PROC_ENV_TEST"] = value;

            Assert.IsTrue(process.Start());
            try
            {
                var env = process.ReadEnvironmentVariables();
                Assert.AreEqual(value, env["PROC_ENV_TEST"]);
            }
            finally
            {
                process.Kill(true);
            }
        }
    }
}
