// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Diagnostics.Process.Tests;

using Process = System.Diagnostics.Process;

[TestClass]
public sealed class EnvironmentTests
{
    [TestMethod]
    public void Environment_ProcessId()
    {
#pragma warning disable CA1837 // Use 'Environment.ProcessId'

        Assert.AreEqual(
            Process.GetCurrentProcess().Id,
            Environment.ProcessId);

#pragma warning restore CA1837
    }
}
