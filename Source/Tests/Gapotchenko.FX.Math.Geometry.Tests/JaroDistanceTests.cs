using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Geometry.Tests;

[TestClass]
public class JaroDistanceTests
{
    [TestMethod]
    public void JaroDistance_Basics()
    {
        RunTest("loans and accounts", "loan account", 0.17);
        RunTest("loan account", "loans and accounts", 0.17);
        RunTest("trace", "crate", 0.27);
        RunTest("trace", "trace", 0);
        RunTest("trace", "", 1);
        RunTest("", "trace", 1);
        RunTest("", "", 0);
        RunTest("abcd", "badc", 0.17);
        RunTest("abcd", "dcba", 0.5);
        RunTest("washington", "notgnihsaw", 0.57);
        RunTest("washington", "washingtonx", 0.03);
        RunTest("daniel", "danielle", 0.08);
        RunTest("sat", "urn", 1);
    }

    void RunTest(string a, string b, double expectedDistance)
    {
        var actualDistance = StringMetrics.JaroDistance(a, b);

        Assert.IsTrue(actualDistance >= 0, $"Jaro distance of '{a}' and '{b}' should be >= 0.");
        Assert.IsTrue(actualDistance <= 1, $"Jaro distance of '{a}' and '{b}' should be <= 1.");

        const double epsilon = 0.01;
        Assert.IsTrue(
            System.Math.Abs(actualDistance - expectedDistance) < epsilon,
            $"Jaro distance of '{a}' and '{b}' is expected to be {expectedDistance}. Actual value is {actualDistance}.");
    }
}
