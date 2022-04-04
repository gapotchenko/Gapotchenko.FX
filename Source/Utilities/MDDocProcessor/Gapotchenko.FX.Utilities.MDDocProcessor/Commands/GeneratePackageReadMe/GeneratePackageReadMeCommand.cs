namespace Gapotchenko.FX.Utilities.MDDocProcessor.Commands.GeneratePackageReadMe
{
    static class GeneratePackageReadMeCommand
    {
        public static void Run(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine(
                    "Usage: {0} generate-package-readme <input markdown file path> <output markdown file path>",
                    Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));

                throw new ProgramExitException(1);
            }

            string inputFilePath = args[0];
            string outputFilePath = args[1];

            var outputDirectoryPath = Path.GetDirectoryName(outputFilePath);
            if (outputDirectoryPath != null)
                Directory.CreateDirectory(outputDirectoryPath);

            File.WriteAllText(outputFilePath, "This is a demo README.md for NuGet:\n- One\n- Two\n- Three");
        }
    }
}
