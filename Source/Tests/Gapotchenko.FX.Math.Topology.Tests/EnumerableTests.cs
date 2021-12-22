﻿using Gapotchenko.FX.Math.Topology.Tests.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology.Tests
{
    [TestClass]
    public class EnumerableTests
    {
        [TestMethod]
        public void Enumerable_TopologicalOrderBy_1()
        {
            DependencyFunction<char> df = (a, b) =>
            {
                switch (a + " depends on " + b)
                {
                    case "A depends on B":
                        return true;

                    default:
                        return false;
                }
            };

            var source = "ABCDEF";
            string result = string.Concat(source.TopologicalOrderBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);

            Assert.AreEqual("BACDEF", result);
        }

        static void AssertTopologicalOrderIsCorrect<T>(IEnumerable<T> source, IEnumerable<T> result, DependencyFunction<T> dependencyFunction)
        {
            Assert.IsTrue(TopologicalOrderProof.Verify(source, result, dependencyFunction), "Topological order is violated.");
            Assert.IsTrue(MinimalDistanceProof.Verify(source, result, dependencyFunction), "Minimal distance not reached.");
        }

        [TestMethod]
        public void Enumerable_TopologicalOrderBy_20()
        {
            var proof = new TopologicalOrderProof
            {
                Sorter = (source, df) => source.TopologicalOrderBy(Fn.Identity, df),
                VerticesCount = 4,
                VerifyMinimalDistance = true
            };
            proof.Run();
        }

        [TestMethod]
        public void Enumerable_TopologicalOrderBy_21()
        {
            var proof = new TopologicalOrderProof
            {
                Sorter = (source, df) => source.TopologicalOrderBy(Fn.Identity, df),
                VerticesCount = 5,
                MaxGraphConfigurationsCount = 10000,
                //VerifyMinimalDistance = true
            };
            proof.Run();
        }

        [TestMethod]
        public void Enumerable_TopologicalOrderBy_25()
        {
            bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "1 depends on 0":
                    case "2 depends on 0":
                        return true;
                    default:
                        return false;
                }
            }

            var source = "1320";
            string result = string.Concat(source.TopologicalOrderBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);

            Assert.AreEqual("0132", result);
        }

    }
}
