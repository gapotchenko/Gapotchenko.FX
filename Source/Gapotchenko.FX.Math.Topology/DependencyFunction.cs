namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// Dependency function indicates how its arguments relate to each other in terms of dependency.
    /// </summary>
    /// <typeparam name="T">The type of arguments the function works with.</typeparam>
    /// <param name="a">The argument A.</param>
    /// <param name="b">The argument B.</param>
    /// <returns>
    /// <see langword="true"/> if the argument <paramref name="a"/> depends on <paramref name="b"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public delegate bool DependencyFunction<in T>(T a, T b);
}
