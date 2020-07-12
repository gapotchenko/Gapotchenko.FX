using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

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

        [TestMethod]
        public void ProcEnv_ReadLarge()
        {
            var process = new Process();

            var psi = process.StartInfo;
            psi.FileName = "dotnet";
            psi.Arguments = "fsi";

            var expectedEnv = psi.EnvironmentVariables;
            expectedEnv.Clear();

            for (int i = 0; i < 1000; ++i)
            {
                string value = Guid.NewGuid().ToString("D");
                expectedEnv["PROC_ENV_TEST_" + i] = value;
            }

            Assert.IsTrue(process.Start());
            try
            {
                var actualEnv = process.ReadEnvironmentVariables();
                foreach (DictionaryEntry i in expectedEnv)
                    Assert.AreEqual((string)i.Value, actualEnv[(string)i.Key]);
            }
            finally
            {
                process.Kill(true);
            }
        }
    }
}
