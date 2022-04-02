using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Commands.GenerateNuGetReadMe
{
    static class GenerateNuGetReadMeCommand
    {
        public static void Run(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine(
                    "Usage: {0} generate-nuget-readme <input markdown file path> <output markdown file path>",
                    Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));

                throw new ProgramExitException(1);
            }

            throw new NotImplementedException("TODO");
        }
    }
}
