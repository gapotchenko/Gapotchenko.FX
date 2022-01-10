using Gapotchenko.FX.Math.Topology.Tests.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology.Tests
{
    [TestClass]
    public class EnumerableTests
    {
        static void AssertTopologicalOrderIsCorrect<T>(IEnumerable<T> source, IEnumerable<T> result, Func<T, T, bool> dependencyFunction)
        {
            Assert.IsTrue(TopologicalOrderProof.Verify(source, result, dependencyFunction), "Topological order is violated.");
            //Assert.IsTrue(MinimalDistanceProof.Verify(source, result, dependencyFunction), "Minimal distance not reached.");
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_1()
        {
            static bool df(char a, char b) =>
                (a, b) switch
                {
                    ('A', 'B') => true,
                    _ => false,
                };

            var source = "ABCDEF";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);

            Assert.AreEqual("BACDEF", result);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_1B()
        {
            static IEnumerable<char>? df(char v) =>
                v switch
                {
                    'A' => new[] { 'B' },
                    'B' => new[] { 'D' },
                    _ => null,
                };

            var source = "ABCDEF";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            Assert.AreEqual("DBACEF", result);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_2()
        {
            static bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "A depends on B":
                        return true;

                    case "B depends on C":
                        return true;

                    default:
                        return false;
                }
            };

            var source = "ABCDEF";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);

            Assert.AreEqual("CBADEF", result);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_3()
        {
            static bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "A depends on B":
                        return true;

                    case "B depends on C":
                        return true;

                    case "C depends on A":
                        return true;

                    default:
                        return false;
                }
            };

            var source = "ABCDEF";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);

            Assert.AreEqual("ABCDEF", result);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_4()
        {
            static bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "B depends on C":
                        return true;

                    case "C depends on B":
                        return true;

                    default:
                        return false;
                }
            };

            var source = "ABCDEF";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);

            Assert.AreEqual("ABCDEF", result);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_5()
        {
            static bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "A depends on C":
                        return true;

                    case "B depends on C":
                        return true;

                    default:
                        return false;
                }
            };

            var source = "ABCDEF";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);

            Assert.AreEqual("CABDEF", result);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_6()
        {
            static bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "A depends on C":
                        return true;

                    case "B depends on C":
                        return true;

                    case "C depends on E":
                        return true;

                    case "E depends on F":
                        return true;

                    default:
                        return false;
                }
            };

            var source = "ABCDEF";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_7()
        {
            static bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "A depends on C":
                        return true;

                    case "B depends on C":
                        return true;

                    case "C depends on E":
                        return true;

                    case "E depends on F":
                        return true;

                    case "B depends on E":
                        return true;

                    default:
                        return false;
                }
            };

            var source = "ABCDEF";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_8()
        {
            static bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "A depends on C":
                        return true;

                    case "B depends on C":
                        return true;

                    case "C depends on E":
                        return true;

                    case "E depends on F":
                        return true;

                    case "F depends on A":
                        return true;

                    default:
                        return false;
                }
            };

            var source = "ABCDEF";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_9()
        {
            static bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "A depends on C":
                        return true;

                    case "B depends on C":
                        return true;

                    case "C depends on E":
                        return true;

                    case "E depends on F":
                        return true;

                    case "E depends on E":
                        return true;

                    default:
                        return false;
                }
            };

            var source = "ABCDEF";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_10()
        {
            static bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "B depends on D":
                        return true;

                    default:
                        return false;
                }
            };

            var source = "ABCDEF";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);

            Assert.AreEqual("ADBCEF", result);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_11()
        {
            static bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "B depends on E":
                        return true;

                    case "E depends on D":
                        return true;

                    default:
                        return false;
                }
            };

            var source = "ABCDEF";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_12()
        {
            static bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "A depends on C":
                    case "A depends on E":
                    case "B depends on A":
                    case "B depends on C":
                    case "D depends on C":
                    case "E depends on C":
                    case "F depends on C":
                        return true;

                    default:
                        return false;
                }
            };

            var source = "ABCDEF";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_22()
        {
            static bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "2 depends on 1":
                    case "3 depends on 0":
                    case "1 depends on 0":
                        return true;

                    default:
                        return false;
                }
            };

            var source = "02431";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);

            //Assert.AreEqual("01243", result);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_25()
        {
            static bool df(char a, char b) =>
                (a, b) switch
                {
                    ('1', '0') or ('2', '0') => true,
                    _ => false,
                };

            var source = "1320";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);

            Assert.AreEqual("0132", result);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_26()
        {
            bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "0 depends on 1":
                    case "2 depends on 0":
                        return true;
                    default:
                        return false;
                }
            }

            var source = "0321";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_27()
        {
            bool df(char a, char b)
            {
                switch (a + " depends on " + b)
                {
                    case "2 depends on 1":
                    case "3 depends on 0":
                        return true;
                    default:
                        return false;
                }
            }

            var source = "2130";
            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));

            AssertTopologicalOrderIsCorrect(source, result, df);
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
    }
}
