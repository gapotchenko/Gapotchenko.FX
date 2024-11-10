using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: TestCategory("threading")]

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.ClassLevel)]
