using Gapotchenko.FX.Math.Topology.Tests.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology.Tests
{
    [TestClass]
    public class EnumerableTests
    {
        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_1()
        {
            static bool df(char a, char b) =>
                (a + " depends on " + b) switch
                {
                    "A depends on B" => true,
                    _ => false,
                };

            var source = "ABCDEF";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);

            Assert.AreEqual("BACDEF", result);
        }

        static void AssertTopologicalOrderIsCorrect<T>(IEnumerable<T> source, IEnumerable<T> result, Func<T, T, bool> dependencyFunction)
        {
            Assert.IsTrue(TopologicalOrderProof.Verify(source, result, dependencyFunction), "Topological order is violated.");
            Assert.IsTrue(MinimalDistanceProof.Verify(source, result, dependencyFunction), "Minimal distance not reached.");
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_Proof4Vertices()
        {
            var proof = new TopologicalOrderProof
            {
                PredicateSorter = (source, df) => source.OrderTopologicallyBy(Fn.Identity, df),
                VerticesCount = 4,
                //VerifyMinimalDistance = true,
            };
            proof.Run();
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_Proof5Vertices()
        {
            var proof = new TopologicalOrderProof
            {
                PredicateSorter = (source, df) => source.OrderTopologicallyBy(Fn.Identity, df),
                VerticesCount = 5,
                MaxTopologiesCount = 10000,
                //SkipCyclicGraphs = true,
                //VerifyMinimalDistance = true
            };
            proof.Run();
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_25()
        {
            static bool df(char a, char b) =>
                (a + " depends on " + b) switch
                {
                    "1 depends on 0" or "2 depends on 0" => true,
                    _ => false,
                };

            var source = "1320";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);

            Assert.AreEqual("0132", result);
        }

    }
}
