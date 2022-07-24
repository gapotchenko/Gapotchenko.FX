using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace Gapotchenko.FX.Diagnostics.Process.Tests;

using Process = System.Diagnostics.Process;

[TestClass]
public class ProcessEnvironmentTests
{
    [TestMethod]
    public void Process_ReadEnvironment()
    {
        using var process = new Process();

        var psi = process.StartInfo;
        psi.FileName = "dotnet";
        psi.Arguments = "fsi";
        psi.UseShellExecute = false;
        psi.RedirectStandardOutput = true;

        string value = Guid.NewGuid().ToString("D");
        psi.EnvironmentVariables["PROC_ENV_TEST"] = value;

        Assert.IsTrue(process.Start());
        try
        {
            // Ensure that the process is fully initialized.
            process.StandardOutput.ReadLine();

            var env = process.ReadEnvironmentVariables();
            Assert.AreEqual(value, env["PROC_ENV_TEST"]);
        }
        finally
        {
            process.Kill();
        }
    }

    [TestMethod]
    public void Process_ReadLargeEnvironment()
    {
        using var process = new Process();

        var psi = process.StartInfo;
        psi.FileName = "dotnet";
        psi.Arguments = "fsi";
        psi.UseShellExecute = false;
        psi.RedirectStandardOutput = true;

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
            // Ensure that the process is fully initialized.
            process.StandardOutput.ReadLine();

            var actualEnv = process.ReadEnvironmentVariables();

#pragma warning disable CS8605 // Unboxing a possibly null value.
            foreach (DictionaryEntry i in expectedEnv)
#pragma warning restore CS8605 // Unboxing a possibly null value.
            {
                string key = (string)i.Key;
                if (!key.StartsWith(envKeyPrefix, StringComparison.Ordinal))
                    continue;
                Assert.AreEqual((string?)i.Value, actualEnv[key]);
            }
        }
        finally
        {
            process.Kill();
        }
    }
}
