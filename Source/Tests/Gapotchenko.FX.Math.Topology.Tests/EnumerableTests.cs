using Gapotchenko.FX.Math.Combinatorics;
using Gapotchenko.FX.Math.Topology.Tests.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology.Tests
{
    [TestClass]
    public class EnumerableTests
    {
        static void AssertTopologicalOrderIsCorrect<T>(
            IEnumerable<T> source,
            IEnumerable<T> result, Func<T, T, bool> dependencyFunction,
            bool minProof = true)
        {
            Assert.IsTrue(TopologicalOrderProof.Verify(source, result, dependencyFunction), "Topological order is violated.");
            if (minProof)
                Assert.IsTrue(MinimalDistanceProof.Verify(source, result, dependencyFunction), "Minimal distance not reached.");
        }

        static void AssertReverseTopologicalOrderIsCorrect<T>(
            IEnumerable<T> source,
            IEnumerable<T> result,
            Func<T, T, bool> dependencyFunction,
            bool minProof = true)
        {
            bool df(T a, T b) => dependencyFunction(b, a);
            Assert.IsTrue(TopologicalOrderProof.Verify(source, result, df), "Reverse topological order is violated.");
            if (minProof)
                Assert.IsTrue(MinimalDistanceProof.Verify(source, result, df), "Minimal distance not reached.");
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

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df);
            Assert.AreEqual("ABCDEF", result);
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

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            Assert.AreEqual("ABCDEF", result);
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

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df);
            Assert.AreEqual("ABCDEF", result);
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

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df);
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

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df);
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

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df);
            Assert.AreEqual("ABCDEF", result);
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
            AssertTopologicalOrderIsCorrect(source, result, df, minProof: false);

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df, minProof: false);
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
            AssertTopologicalOrderIsCorrect(source, result, df, minProof: false);

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df, minProof: false);
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
            AssertTopologicalOrderIsCorrect(source, result, df, minProof: false);

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df, minProof: false);
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
            AssertTopologicalOrderIsCorrect(source, result, df, minProof: false);

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df, minProof: false);
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

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df);
            Assert.AreEqual("ABCDEF", result);
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
            AssertTopologicalOrderIsCorrect(source, result, df, minProof: false);

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df, minProof: false);
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

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df);
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
            AssertTopologicalOrderIsCorrect(source, result, df, minProof: false);
            //Assert.AreEqual("01243", result);

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df, minProof: false);
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

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df);
            Assert.AreEqual("1320", result);
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

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df);
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
            AssertTopologicalOrderIsCorrect(source, result, df, minProof: false);

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df, minProof: false);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_28A()
        {
            static bool df(char a, char b) =>
                (a, b) switch
                {
                    ('A', 'B') or ('D', 'C') or ('E', 'D') => true,
                    _ => false,
                };

            var source = "ABCDEF";

            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));
            AssertTopologicalOrderIsCorrect(source, result, df);
            Assert.AreEqual("BACDEF", result);

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            AssertReverseTopologicalOrderIsCorrect(source, result, df);
            Assert.AreEqual("ABEDCF", result);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_28B()
        {
            static IEnumerable<char>? df(char v) =>
                v switch
                {
                    'A' => new[] { 'B' },
                    'D' => new[] { 'C' },
                    'E' => new[] { 'D' },
                    _ => null,
                };

            var source = "ABCDEF";

            string result = string.Concat(source.OrderTopologicallyBy(Fn.Identity, df));
            Assert.AreEqual("BACDEF", result);

            result = string.Concat(source.OrderTopologicallyByReverse(Fn.Identity, df));
            Assert.AreEqual("ABEDCF", result);
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyBy_Subsequent()
        {
            IEnumerable<char> seq = "";

            IEnumerable<char> order = seq.OrderTopologicallyBy(Fn.Identity, (a, b) => false).ThenBy(Fn.Identity);
            Assert.AreEqual(0, order.Count());

            /***************/

            seq = "a";

            order = seq.OrderTopologicallyBy(Fn.Identity, (a, b) => false).ThenBy(Fn.Identity);
            Assert.AreEqual("a", string.Concat(order));

            /***************/

            seq = "ab";

            order = seq.OrderTopologicallyBy(Fn.Identity, (a, b) => false).ThenBy(Fn.Identity);
            Assert.AreEqual("ab", string.Concat(order));

            order = seq.OrderTopologicallyBy(Fn.Identity, (a, b) => false).ThenBy(x => -x);
            Assert.AreEqual("ba", string.Concat(order));

            var customComparer = Comparer<char>.Create((x, y) => x.CompareTo(y));
            order = seq.OrderTopologicallyBy(Fn.Identity, (a, b) => false).ThenBy(Fn.Identity, customComparer);
            Assert.AreEqual("ab", string.Concat(order));

            customComparer = Comparer<char>.Create((x, y) => y.CompareTo(x));
            order = seq.OrderTopologicallyBy(Fn.Identity, (a, b) => false).ThenBy(Fn.Identity, customComparer);
            Assert.AreEqual("ba", string.Concat(order));

            /***************/

            seq = "0123";

            Func<char, char, bool> df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('0', '1') or ('0', '2') or ('0', '3') => true,
                        _ => false
                    };

            foreach (var p in Permutations.Of(seq))
            {
                order = p.OrderTopologicallyBy(Fn.Identity, df).ThenBy(Fn.Identity);
                Assert.AreEqual("1230", string.Concat(order));
            }

            /***************/

            seq = "012345";

            df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('0', '4') or ('0', '5') or
                        ('1', '4') or
                        ('2', '5') or
                        ('3', '1') or ('3', '2') => true,
                        _ => false
                    };

            foreach (var p in Permutations.Of(seq))
            {
                order = p.OrderTopologicallyBy(Fn.Identity, df).ThenBy(Fn.Identity);
                Assert.AreEqual("450123", string.Concat(order));
            }

            /***************/

            seq = "abcdefZyz";

            df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('b', 'a') or
                        ('c', 'b') or
                        ('d', 'c') or
                        ('Z', 'd') or
                        ('y', 'Z') or
                        ('z', 'd') or
                        ('y', 'd') or
                        ('e', 'd') or
                        ('f', 'd') => true,
                        _ => false
                    };

            order = seq.OrderTopologicallyBy(Fn.Identity, df).ThenBy(Fn.Identity);
            Assert.AreEqual("abcdZefyz", string.Concat(order));

            /***************/

            seq = "76543210";

            df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('1', '0') or
                        ('4', '0') or
                        ('3', '1') or
                        ('5', '1') or
                        ('5', '3') or
                        ('7', '3') or
                        ('6', '5') or
                        ('7', '6') => true,
                        _ => false
                    };

            order = seq.OrderTopologicallyBy(Fn.Identity, df).ThenBy(Fn.Identity);
            Assert.AreEqual("01234567", string.Concat(order));

            /***************/

            seq = "a987654321";

            df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('2', '1') or
                        ('3', '2') or
                        ('4', '2') or
                        ('5', '2') or
                        ('6', '2') or
                        ('7', '3') or
                        ('8', '3') or
                        ('9', '4') or
                        ('9', '6') or
                        ('a', '7') or
                        ('9', '7') or
                        ('a', '8') or
                        ('9', '8') => true,
                        _ => false
                    };

            order = seq.OrderTopologicallyBy(Fn.Identity, df).ThenBy(Fn.Identity);
            Assert.AreEqual("123456789a", string.Concat(order));

            /***************/

            seq = "apparatus";

            df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('a', 'a') => true,
                        _ => false
                    };

            order = seq.OrderTopologicallyBy(Fn.Identity, df).ThenBy(Fn.Identity);
            Assert.AreEqual("aaapprstu", string.Concat(order));

            /***************/

            seq = "cab";

            df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('a', 'c') or ('b', 'a') or ('c', 'b') => true,
                        _ => false
                    };

            order = seq.OrderTopologicallyBy(Fn.Identity, df).ThenBy(Fn.Identity);
            Assert.AreEqual("abc", string.Concat(order));

            /***************/

            seq = "acedb";

            df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('d', 'b') => true,
                        _ => false
                    };

            order = seq.OrderTopologicallyBy(Fn.Identity, df).ThenBy(Fn.Identity);
            Assert.AreEqual("abcde", string.Concat(order));

            order = seq.OrderTopologicallyBy(Fn.Identity, df).ThenByDescending(Fn.Identity);
            Assert.AreEqual("ebdca", string.Concat(order));

            /***************/

            IEnumerable<(int a, int b)> seq_001 = new[]
            {
                (1, 0), (1, 1), (1, 2), (1, 3),
                (2, 0), (2, 1), (2, 2), (2, 3)
            };

            static bool df_001((int, int) x, (int, int) y) =>
                (x, y) switch
                {
                    ((2, 0), (1, 0)) => true,
                    _ => false
                };

            var order_001 = seq_001
                .OrderTopologicallyBy(Fn.Identity, df_001)
                .ThenBy(x => x.a)
                .ThenBy(x => x.b)
                .AsEnumerable();
            Assert.AreEqual("10 11 12 13 20 21 22 23", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

            order_001 = seq_001
                .OrderTopologicallyBy(Fn.Identity, df_001)
                .ThenByDescending(x => x.a)
                .ThenBy(x => x.b);
            Assert.AreEqual("10 20 21 22 23 11 12 13", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

            order_001 = seq_001
                .OrderTopologicallyBy(Fn.Identity, df_001)
                .ThenBy(x => x.a)
                .ThenByDescending(x => x.b);
            Assert.AreEqual("13 12 11 10 23 22 21 20", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

            order_001 = seq_001
                .OrderTopologicallyBy(Fn.Identity, df_001)
                .ThenByDescending(x => x.a)
                .ThenByDescending(x => x.b);
            Assert.AreEqual("23 22 21 10 20 13 12 11", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

            order_001 = seq_001
                .OrderTopologicallyBy(Fn.Identity, df_001)
                .ThenBy(x => x.b)
                .ThenBy(x => x.a);
            Assert.AreEqual("10 20 11 21 12 22 13 23", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

            order_001 = seq_001
                .OrderTopologicallyBy(Fn.Identity, df_001)
                .ThenByDescending(x => x.b)
                .ThenBy(x => x.a);
            Assert.AreEqual("13 23 12 22 11 21 10 20", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

            order_001 = seq_001
                .OrderTopologicallyBy(Fn.Identity, df_001)
                .ThenBy(x => x.b)
                .ThenByDescending(x => x.a);
            Assert.AreEqual("10 20 21 11 22 12 23 13", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

            order_001 = seq_001
                .OrderTopologicallyBy(Fn.Identity, df_001)
                .ThenByDescending(x => x.b)
                .ThenByDescending(x => x.a);
            Assert.AreEqual("23 13 22 12 21 11 10 20", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));
        }

        [TestMethod]
        public void Enumerable_OrderTopologicallyByReverse_Subsequent()
        {
            IEnumerable<char> seq = "";

            IEnumerable<char> order = seq.OrderTopologicallyByReverse(Fn.Identity, (a, b) => false).ThenBy(Fn.Identity);
            Assert.AreEqual(0, order.Count());

            /***************/

            seq = "a";

            order = seq.OrderTopologicallyByReverse(Fn.Identity, (a, b) => false).ThenBy(Fn.Identity);
            Assert.AreEqual("a", string.Concat(order));

            /***************/

            seq = "ab";

            order = seq.OrderTopologicallyByReverse(Fn.Identity, (a, b) => false).ThenBy(Fn.Identity);
            Assert.AreEqual("ab", string.Concat(order));

            order = seq.OrderTopologicallyByReverse(Fn.Identity, (a, b) => false).ThenBy(x => -x);
            Assert.AreEqual("ba", string.Concat(order));

            var customComparer = Comparer<char>.Create((x, y) => x.CompareTo(y));
            order = seq.OrderTopologicallyByReverse(Fn.Identity, (a, b) => false).ThenBy(Fn.Identity, customComparer);
            Assert.AreEqual("ab", string.Concat(order));

            customComparer = Comparer<char>.Create((x, y) => y.CompareTo(x));
            order = seq.OrderTopologicallyByReverse(Fn.Identity, (a, b) => false).ThenBy(Fn.Identity, customComparer);
            Assert.AreEqual("ba", string.Concat(order));

            /***************/

            seq = "0123";

            Func<char, char, bool> df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('0', '1') or ('0', '2') or ('0', '3') => true,
                        _ => false
                    };

            foreach (var p in Permutations.Of(seq))
            {
                order = p.OrderTopologicallyByReverse(Fn.Identity, df).ThenBy(Fn.Identity);
                Assert.AreEqual("0123", string.Concat(order));
            }

            /***************/

            seq = "012345";

            df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('0', '4') or ('0', '5') or
                        ('1', '4') or
                        ('2', '5') or
                        ('3', '1') or ('3', '2') => true,
                        _ => false
                    };

            foreach (var p in Permutations.Of(seq))
            {
                order = p.OrderTopologicallyByReverse(Fn.Identity, df).ThenBy(Fn.Identity);
                Assert.AreEqual("031245", string.Concat(order));
            }

            /***************/

            seq = "abcdefZyz";

            df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('b', 'a') or
                        ('c', 'b') or
                        ('d', 'c') or
                        ('Z', 'd') or
                        ('y', 'Z') or
                        ('z', 'd') or
                        ('y', 'd') or
                        ('e', 'd') or
                        ('f', 'd') => true,
                        _ => false
                    };

            order = seq.OrderTopologicallyByReverse(Fn.Identity, df).ThenBy(Fn.Identity);
            Assert.AreEqual("yZefzdcba", string.Concat(order));

            /***************/

            seq = "76543210";

            df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('1', '0') or
                        ('4', '0') or
                        ('3', '1') or
                        ('5', '1') or
                        ('5', '3') or
                        ('7', '3') or
                        ('6', '5') or
                        ('7', '6') => true,
                        _ => false
                    };

            order = seq.OrderTopologicallyByReverse(Fn.Identity, df).ThenByDescending(Fn.Identity);
            Assert.AreEqual("76543210", string.Concat(order));

            /***************/

            seq = "a987654321";

            df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('2', '1') or
                        ('3', '2') or
                        ('4', '2') or
                        ('5', '2') or
                        ('6', '2') or
                        ('7', '3') or
                        ('8', '3') or
                        ('9', '4') or
                        ('9', '6') or
                        ('a', '7') or
                        ('9', '7') or
                        ('a', '8') or
                        ('9', '8') => true,
                        _ => false
                    };

            order = seq.OrderTopologicallyByReverse(Fn.Identity, df).ThenByDescending(Fn.Identity);
            Assert.AreEqual("a987654321", string.Concat(order));

            /***************/

            seq = "apparatus";

            df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('a', 'a') => true,
                        _ => false
                    };

            order = seq.OrderTopologicallyByReverse(Fn.Identity, df).ThenBy(Fn.Identity);
            Assert.AreEqual("aaapprstu", string.Concat(order));

            /***************/

            seq = "cab";

            df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('a', 'c') or ('b', 'a') or ('c', 'b') => true,
                        _ => false
                    };

            order = seq.OrderTopologicallyByReverse(Fn.Identity, df).ThenBy(Fn.Identity);
            Assert.AreEqual("abc", string.Concat(order));

            /***************/

            seq = "acedb";

            df =
                static (a, b) =>
                    (a, b) switch
                    {
                        ('d', 'b') => true,
                        _ => false
                    };

            order = seq.OrderTopologicallyBy(Fn.Identity, df).ThenBy(Fn.Identity);
            Assert.AreEqual("abcde", string.Concat(order));

            order = seq.OrderTopologicallyBy(Fn.Identity, df).ThenByDescending(Fn.Identity);
            Assert.AreEqual("ebdca", string.Concat(order));

            /***************/

            IEnumerable<(int a, int b)> seq_001 = new[]
            {
                (1, 0), (1, 1), (1, 2), (1, 3),
                (2, 0), (2, 1), (2, 2), (2, 3)
            };

            static bool df_001((int, int) x, (int, int) y) =>
                (x, y) switch
                {
                    ((1, 0), (2, 0)) => true,
                    _ => false
                };

            var order_001 = seq_001
                .OrderTopologicallyByReverse(Fn.Identity, df_001)
                .ThenBy(x => x.a)
                .ThenBy(x => x.b)
                .AsEnumerable();
            Assert.AreEqual("10 11 12 13 20 21 22 23", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

            order_001 = seq_001
                .OrderTopologicallyByReverse(Fn.Identity, df_001)
                .ThenByDescending(x => x.a)
                .ThenBy(x => x.b);
            Assert.AreEqual("10 20 21 22 23 11 12 13", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

            order_001 = seq_001
                .OrderTopologicallyByReverse(Fn.Identity, df_001)
                .ThenBy(x => x.a)
                .ThenByDescending(x => x.b);
            Assert.AreEqual("13 12 11 10 23 22 21 20", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

            order_001 = seq_001
                .OrderTopologicallyByReverse(Fn.Identity, df_001)
                .ThenByDescending(x => x.a)
                .ThenByDescending(x => x.b);
            Assert.AreEqual("23 22 21 10 20 13 12 11", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

            order_001 = seq_001
                .OrderTopologicallyByReverse(Fn.Identity, df_001)
                .ThenBy(x => x.b)
                .ThenBy(x => x.a);
            Assert.AreEqual("10 20 11 21 12 22 13 23", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

            order_001 = seq_001
                .OrderTopologicallyByReverse(Fn.Identity, df_001)
                .ThenByDescending(x => x.b)
                .ThenBy(x => x.a);
            Assert.AreEqual("13 23 12 22 11 21 10 20", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

            order_001 = seq_001
                .OrderTopologicallyByReverse(Fn.Identity, df_001)
                .ThenBy(x => x.b)
                .ThenByDescending(x => x.a);
            Assert.AreEqual("10 20 21 11 22 12 23 13", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));

            order_001 = seq_001
                .OrderTopologicallyByReverse(Fn.Identity, df_001)
                .ThenByDescending(x => x.b)
                .ThenByDescending(x => x.a);
            Assert.AreEqual("23 13 22 12 21 11 10 20", string.Join(" ", order_001.Select(x => $"{x.a}{x.b}")));
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
        public void Enumerable_OrderTopologicallyByReverse_Proof4Vertices()
        {
            var proof = new TopologicalOrderProof
            {
                PredicateSorter = (source, df) => source.OrderTopologicallyByReverse(Fn.Identity, (a, b) => df(b, a)),
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
