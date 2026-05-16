namespace Gapotchenko.FX.Diagnostics;

partial class CommandLine
{
    /// <summary>
    /// Builds a command line from a specified argument.
    /// </summary>
    /// <param name="arg">The argument.</param>
    /// <returns>The command line.</returns>
    public static string Build(string? arg) =>
        new CommandLineBuilder()
            .AppendArgument(arg)
            .ToString();

    /// <summary>
    /// Builds a command line from two specified arguments.
    /// </summary>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <returns>The command line.</returns>
    public static string Build(string? arg1, string? arg2) =>
        new CommandLineBuilder()
            .AppendArgument(arg1)
            .AppendArgument(arg2)
            .ToString();

    /// <summary>
    /// Builds a command line from three specified arguments.
    /// </summary>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <returns>The command line.</returns>
    public static string Build(string? arg1, string? arg2, string? arg3) =>
        new CommandLineBuilder()
            .AppendArgument(arg1)
            .AppendArgument(arg2)
            .AppendArgument(arg3)
            .ToString();

    /// <summary>
    /// Builds a command line from four specified arguments.
    /// </summary>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <returns>The command line.</returns>
    public static string Build(string? arg1, string? arg2, string? arg3, string? arg4) =>
        new CommandLineBuilder()
            .AppendArgument(arg1)
            .AppendArgument(arg2)
            .AppendArgument(arg3)
            .AppendArgument(arg4)
            .ToString();

    /// <summary>
    /// Builds a command line from a specified sequence of arguments.
    /// </summary>
    /// <param name="args">The sequence of arguments.</param>
    /// <returns>The built command line.</returns>
    public static string Build(params IEnumerable<string?> args)
    {
        ArgumentNullException.ThrowIfNull(args);

        var clb = new CommandLineBuilder();
        foreach (string? arg in args)
            clb.AppendArgument(arg);
        return clb.ToString();
    }

#if BINARY_COMPATIBILITY // 2026
    /// <inheritdoc cref="Build(IEnumerable{string?})"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string Build(params string?[] args) => Build((IEnumerable<string?>)args);
#endif
}
