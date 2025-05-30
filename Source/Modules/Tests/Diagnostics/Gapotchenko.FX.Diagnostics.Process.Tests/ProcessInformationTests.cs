﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Diagnostics.Process.Tests;

using Process = System.Diagnostics.Process;

[TestClass]
public class ProcessInformationTests
{
    [TestMethod]
    public void Process_GetParent()
    {
        var process = new Process();

        var psi = process.StartInfo;
        psi.FileName = "dotnet";
        psi.Arguments = "fsi";
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;

        Assert.IsTrue(process.Start());
        try
        {
            var parent = process.GetParent();

            Assert.IsNotNull(parent);

            var currentProcess = Process.GetCurrentProcess();
            Assert.AreEqual(currentProcess.Id, parent!.Id);
            Assert.AreEqual(currentProcess.StartTime, parent.StartTime);
        }
        finally
        {
            process.Kill();
        }
    }

    [TestMethod]
    public void Process_GetImageFileName()
    {
        var process = Process.GetCurrentProcess();
        Assert.AreEqual(process.MainModule?.FileName, process.GetImageFileName());
    }
}
