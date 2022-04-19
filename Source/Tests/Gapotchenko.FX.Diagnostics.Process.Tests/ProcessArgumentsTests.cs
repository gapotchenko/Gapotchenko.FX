using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Gapotchenko.FX.Diagnostics.Process.Tests
{
    using Process = System.Diagnostics.Process;

    [TestClass]
    public class ProcessArgumentsTests
    {
        [TestMethod]
        public void Process_ReadArguments()
        {
            using var process = new Process();

            var psi = process.StartInfo;
            psi.FileName = "dotnet";
            psi.Arguments = "fsi";
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;

            Assert.IsTrue(process.Start());
            try
            {
                // Ensure that the process is fully initialized.
                process.StandardOutput.ReadLine();

                var commandLine = process.ReadArguments();
                Assert.AreEqual("dotnet fsi", CommandLine.Build(CommandLine.Split(commandLine)));
            }
            finally
            {
                process.Kill();
            }
        }

        [TestMethod]
        public void Process_ReadArgumentList()
        {
            using var process = new Process();

            var psi = process.StartInfo;
            psi.FileName = "dotnet";
            psi.Arguments = "fsi";
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;

            Assert.IsTrue(process.Start());
            try
            {
                // Ensure that the process is fully initialized.
                process.StandardOutput.ReadLine();

                var arguments = process.ReadArgumentList().ToList();
                Assert.AreEqual(2, arguments.Count);
                Assert.AreEqual("dotnet", arguments[0]);
                Assert.AreEqual("fsi", arguments[1]);
            }
            finally
            {
                process.Kill();
            }
        }
    }
}
