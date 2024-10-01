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
    /// <param name="args">A sequence of arguments.</param>
    /// <returns>The command line.</returns>
    public static string Build(IEnumerable<string?> args)
    {
        if (args == null)
            throw new ArgumentNullException(nameof(args));

        var clb = new CommandLineBuilder();
        foreach (var arg in args)
            clb.AppendArgument(arg);
        return clb.ToString();
    }

    /// <summary>
    /// Builds a command line from a specified array of arguments.
    /// </summary>
    /// <param name="args">An array of arguments.</param>
    /// <returns>The command line.</returns>
    public static string Build(params string?[] args) => Build((IEnumerable<string?>)args);
}
