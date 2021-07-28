namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// Dependency function specifies how its arguments relate to each other in terms of dependency.
    /// </summary>
    /// <typeparam name="T">The type of arguments the function works with.</typeparam>
    /// <param name="a">The argument A.</param>
    /// <param name="b">The argument B.</param>
    /// <returns><c>true</c> if the argument <paramref name="a"/> depends on <paramref name="b"/>; otherwise, <c>false</c>.</returns>
    public delegate bool DependencyFunction<in T>(T a, T b);
}
