using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;

[assembly: TestCategory("collections")]
[assembly: AssemblyTrait("Category", "collections")]

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.ClassLevel)]
