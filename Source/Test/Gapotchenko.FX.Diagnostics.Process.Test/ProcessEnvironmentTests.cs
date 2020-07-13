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
        public void Process_ReadEnvironment()
        {
            var process = new Process();

            var psi = process.StartInfo;
            psi.FileName = "dotnet";
            psi.Arguments = "fsi";
            psi.RedirectStandardOutput = true;
            psi.StandardOutputEncoding = CommandLine.OemEncoding;

            string value = Guid.NewGuid().ToString("D");
            psi.EnvironmentVariables["PROC_ENV_TEST"] = value;

            Assert.IsTrue(process.Start());
            try
            {
                // Wait until the process is started.
                process.StandardOutput.ReadLine();

                var env = process.ReadEnvironmentVariables();
                Assert.AreEqual(value, env["PROC_ENV_TEST"]);
            }
            finally
            {
                process.Kill(true);
            }
        }

        [TestMethod]
        public void Process_ReadLargeEnvironment()
        {
            var process = new Process();

            var psi = process.StartInfo;
            psi.FileName = "dotnet";
            psi.Arguments = "fsi";
            psi.RedirectStandardOutput = true;
            psi.StandardOutputEncoding = CommandLine.OemEncoding;

            const string envKeyPrefix = "PROC_ENV_TEST_";

            var expectedEnv = psi.EnvironmentVariables;
            for (int i = 0; i < 1000; ++i)
            {
                string value = Guid.NewGuid().ToString("D");
                expectedEnv[envKeyPrefix + i] = value;
            }

            Assert.IsTrue(process.Start());
            try
            {
                // Wait until the process is started.
                process.StandardOutput.ReadLine();

                var actualEnv = process.ReadEnvironmentVariables();

                foreach (DictionaryEntry i in expectedEnv)
                {
                    string key = (string)i.Key;
                    if (!key.StartsWith(envKeyPrefix, StringComparison.Ordinal))
                        continue;
                    Assert.AreEqual((string)i.Value, actualEnv[key]);
                }
            }
            finally
            {
                process.Kill(true);
            }
        }
    }
}
