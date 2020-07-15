extern alias Testable;

using Microsoft.VisualStudio.TestTools.UnitTesting;

#nullable enable

namespace Gapotchenko.FX.Diagnostics.CommandLine.Test
{
    using CommandLine = Testable::Gapotchenko.FX.Diagnostics.CommandLine;

    partial class CommandLineTests
    {
        [TestMethod]
        public void CommandLine_BuildTwo_NoEscape()
        {
            string commandLine = CommandLine.Build("a", "b");
            Assert.AreEqual("a b", commandLine);
        }

        [TestMethod]
        public void CommandLine_BuildTwo_EscapeFirst()
        {
            string commandLine = CommandLine.Build("a x", "b");
            Assert.AreEqual("\"a x\" b", commandLine);
        }

        [TestMethod]
        public void CommandLine_BuildTwo_EscapeSecond()
        {
            string commandLine = CommandLine.Build("a", "b y");
            Assert.AreEqual("a \"b y\"", commandLine);
        }

        [TestMethod]
        public void CommandLine_BuildTwo_EscapeAll()
        {
            string commandLine = CommandLine.Build("a x", "b y");
            Assert.AreEqual("\"a x\" \"b y\"", commandLine);
        }

        [TestMethod]
        public void CommandLine_BuildTwo_FirstIsEmpty()
        {
            string commandLine = CommandLine.Build("", "b");
            Assert.AreEqual("b", commandLine);
        }

        [TestMethod]
        public void CommandLine_BuildTwo_SecondIsEmpty()
        {
            string commandLine = CommandLine.Build("a", "");
            Assert.AreEqual("a", commandLine);
        }

        [TestMethod]
        public void CommandLine_BuildTwo_AllAreEmpty()
        {
            string commandLine = CommandLine.Build("", "");
            Assert.AreEqual("", commandLine);
        }

        [TestMethod]
        public void CommandLine_BuildTwo_FirstIsNull()
        {
            string commandLine = CommandLine.Build(null, "b");
            Assert.AreEqual("b", commandLine);
        }

        [TestMethod]
        public void CommandLine_BuildTwo_SecondIsNull()
        {
            string commandLine = CommandLine.Build("a", null);
            Assert.AreEqual("a", commandLine);
        }

        [TestMethod]
        public void CommandLine_BuildTwo_AllAreNull()
        {
            string commandLine = CommandLine.Build(null, null);
            Assert.AreEqual("", commandLine);
        }

        [TestMethod]
        public void CommandLine_BuildThree_SecondIsEmpty()
        {
            string commandLine = CommandLine.Build("a", "", "c");
            Assert.AreEqual("a c", commandLine);
        }

        [TestMethod]
        public void CommandLine_BuildThree_SecondIsNull()
        {
            string commandLine = CommandLine.Build("a", null, "c");
            Assert.AreEqual("a c", commandLine);
        }
    }
}
