namespace Gapotchenko.FX.Linq.Tests;

static class Utils
{
    public static IEnumerable<T> NullEnumerable<T>() => null!;

    public static TResult FuncMirror<TResult>(
        Func<TResult> a,
        Func<TResult> b)
    {
        TResult? resultA = default;
        Exception? exceptionA = null;

        TResult? resultB = default;
        Exception? exceptionB = null;

        try
        {
            try
            {
                resultA = a();
            }
            catch (Exception e)
            {
                exceptionA = e;
            }

            try
            {
                resultB = b();
            }
            catch (Exception e)
            {
                exceptionB = e;
                throw;
            }
        }
        finally
        {
            Assert.AreEqual(resultA, resultB, "Function mirror result discrepancy.");
            Assert.AreEqual(exceptionA?.GetType(), exceptionB?.GetType(), "Function mirror exception discrepancy.");
        }

        return resultA!;
    }
}
