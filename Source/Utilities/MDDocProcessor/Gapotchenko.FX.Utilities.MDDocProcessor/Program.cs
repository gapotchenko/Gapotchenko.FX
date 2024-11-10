using Gapotchenko.FX.Utilities.MDDocProcessor.Commands.GeneratePackageReadMe;
using Gapotchenko.FX.Utilities.MDDocProcessor.Commands.GenerateToc;

namespace Gapotchenko.FX.Utilities.MDDocProcessor;

static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            RunCore(args);
            return 0;
        }
        catch (ProgramExitException e)
        {
            return e.ExitCode;
        }
        catch (Exception e)
        {
            Console.Write("Error: ");
            Console.WriteLine(e.Message);
            return 1;
        }
    }

    static void RunCore(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine(
                "Usage: {0} <command>",
                Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));
            Console.WriteLine();
            Console.WriteLine(
"""
Options:
  <command>  The command to run. Possible values: generate-toc,
             generate-package-readme.

Commands:
  generate-toc             Generate table of contents in all README.md files.
  generate-package-readme  Generate NuGet README.md file from a GitHub
                           README.md.
""");

            throw new ProgramExitException(1);
        }

        string command = args[0];
        switch (command)
        {
            case "generate-toc":
                GenerateTocCommand.Run(args.Skip(1).ToArray());
                break;
            case "generate-package-readme":
                GeneratePackageReadMeCommand.Run(args.Skip(1).ToArray());
                break;
            default:
                throw new Exception(string.Format("Unknown command \"{0}\".", command));
        }
    }

}
