using Gapotchenko.FX.Text;
using Gapotchenko.FX.Threading.Tasks;

namespace Gapotchenko.FX.Linq.Tests;

partial class EnumerableExtensionsTests
{
    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_NullSourceArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => ScalarOrDefaultCore(Utils.NullEnumerable<int>()));
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Value_NullSourceArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => ScalarOrDefaultCore(Utils.NullEnumerable<string>(), "X"));
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Empty()
    {
        string[] seq = [];
        string? result = ScalarOrDefaultCore(seq);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Single()
    {
        string[] seq = ["ABC"];
        string? result = ScalarOrDefaultCore(seq);
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_MultipleDiff()
    {
        string[] seq = ["ABC", "DEF"];
        string? result = ScalarOrDefaultCore(seq);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_MultipleSame()
    {
        string[] seq = ["ABC", "ABC"];
        string? result = ScalarOrDefaultCore(seq);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Value_Empty()
    {
        string[] seq = [];
        string result = ScalarOrDefaultCore(seq, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Value_Single()
    {
        string[] seq = ["ABC"];
        string result = ScalarOrDefaultCore(seq, "X");
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Value_MultipleDiff()
    {
        string[] seq = ["ABC", "DEF"];
        string result = ScalarOrDefaultCore(seq, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Value_MultipleSame()
    {
        string[] seq = ["ABC", "ABC"];
        string result = ScalarOrDefaultCore(seq, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_NullSeqArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => ScalarOrDefaultCore(Utils.NullEnumerable<int>(), _ => true));
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_NullPredicateArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => ScalarOrDefaultCore(Enumerable.Empty<int>(), null!));
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_NullSeqArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => ScalarOrDefaultCore(Utils.NullEnumerable<int>(), _ => true, 10));
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_NullPredicateArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => ScalarOrDefaultCore([], null!, 10));
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Empty()
    {
        string[] seq = [];
        string? result = ScalarOrDefaultCore(seq, _ => true);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Empty_NoMatch()
    {
        string[] seq = [];
        string? result = ScalarOrDefaultCore(seq, _ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_Empty()
    {
        string[] seq = [];
        string result = ScalarOrDefaultCore(seq, _ => true, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_Empty_NoMatch()
    {
        string[] seq = [];
        string result = ScalarOrDefaultCore(seq, _ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Single()
    {
        string[] seq = ["ABC"];
        string? result = ScalarOrDefaultCore(seq, _ => true);
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Single_NoMatch()
    {
        string[] seq = ["ABC"];
        string? result = ScalarOrDefaultCore(seq, _ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_Single()
    {
        string[] seq = ["ABC"];
        string result = ScalarOrDefaultCore(seq, _ => true, "X");
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_Single_NoMatch()
    {
        string[] seq = ["ABC"];
        string result = ScalarOrDefaultCore(seq, _ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleDiff()
    {
        string[] seq = ["ABC", "DEF"];
        string? result = ScalarOrDefaultCore(seq, _ => true);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleDiff_NoMatch()
    {
        string[] seq = ["ABC", "DEF"];
        string? result = ScalarOrDefaultCore(seq, _ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_MultipleDiff()
    {
        string[] seq = ["ABC", "DEF"];
        string result = ScalarOrDefaultCore(seq, _ => true, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_MultipleDiff_NoMatch()
    {
        string[] seq = ["ABC", "DEF"];
        string result = ScalarOrDefaultCore(seq, _ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleSame()
    {
        string[] seq = ["ABC", "ABC"];
        string? result = ScalarOrDefaultCore(seq, _ => true);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleSame_NoMatch()
    {
        string[] seq = ["ABC", "ABC"];
        string? result = ScalarOrDefaultCore(seq, _ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_MultipleSame()
    {
        string[] seq = ["ABC", "ABC"];
        string result = ScalarOrDefaultCore(seq, _ => true, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_MultipleSame_NoMatch()
    {
        string[] seq = ["ABC", "ABC"];
        string result = ScalarOrDefaultCore(seq, _ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_NoMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ"];
        string? result = ScalarOrDefaultCore(seq, x => x.StartsWith('Z'));
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_SingleMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ"];
        string? result = ScalarOrDefaultCore(seq, x => x.StartsWith('D'));
        Assert.AreEqual("DEF", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_MultpipleMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ", "AMBER"];
        string? result = ScalarOrDefaultCore(seq, x => x.StartsWith('A'));
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_NoMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ"];
        string result = ScalarOrDefaultCore(seq, x => x.StartsWith('Z'), "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_SingleMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ"];
        string result = ScalarOrDefaultCore(seq, x => x.StartsWith('D'), "X");
        Assert.AreEqual("DEF", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_MultipleMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ", "AMBER"];
        string result = ScalarOrDefaultCore(seq, x => x.StartsWith('A'), "X");
        Assert.AreEqual("X", result);
    }

    // ------------------------------------------------------------------------

    static T? ScalarOrDefaultCore<T>(IEnumerable<T> source)
    {
        return Utils.FuncMirror(
            () => source.ScalarOrDefault(),
            () => TaskBridge.Execute(async ct => await source.ToAsyncEnumerable().ScalarOrDefaultAsync(ct).ConfigureAwait(false)));
    }

    static T ScalarOrDefaultCore<T>(IEnumerable<T> source, T defaultValue)
    {
        return Utils.FuncMirror(
            () => source.ScalarOrDefault(defaultValue),
            () => TaskBridge.Execute(async ct => await source.ToAsyncEnumerable().ScalarOrDefaultAsync(defaultValue, ct).ConfigureAwait(false)));
    }

    static T? ScalarOrDefaultCore<T>(IEnumerable<T> source, Func<T, bool> predicate)
    {
        return Utils.FuncMirror(
            () => source.ScalarOrDefault(predicate),
            () => TaskBridge.Execute(async ct => await source.ToAsyncEnumerable().ScalarOrDefaultAsync(predicate, ct).ConfigureAwait(false)));
    }

    static T ScalarOrDefaultCore<T>(IEnumerable<T> source, Func<T, bool> predicate, T defaultValue)
    {
        return Utils.FuncMirror(
            () => source.ScalarOrDefault(predicate, defaultValue),
            () => TaskBridge.Execute(async ct => await source.ToAsyncEnumerable().ScalarOrDefaultAsync(predicate, defaultValue, ct).ConfigureAwait(false)));
    }
}
