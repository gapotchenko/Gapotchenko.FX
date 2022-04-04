namespace Gapotchenko.FX.Utilities.MDDocProcessor.Commands.GenerateNuGetReadMe
{
    static class GenerateNuGetReadMeCommand
    {
        public static void Run(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine(
                    "Usage: {0} generate-nuget-readme <input markdown file path> <output markdown file path>",
                    Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));

                throw new ProgramExitException(1);
            }

            string inputFilePath = args[0];
            string outputFilePath = args[1];

            File.WriteAllText(outputFilePath, "This is a demo README.md for NuGet:\n- One\n- Two\n- Three");
        }
    }
}
