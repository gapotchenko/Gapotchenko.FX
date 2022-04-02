using Gapotchenko.FX.Utilities.MDDocProcessor.Model;
using Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;
using Gapotchenko.FX.Utilities.MDDocProcessor.Visualization;

namespace Gapotchenko.FX.Utilities.MDDocProcessor
{
    static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                _Run(args);
                return Environment.ExitCode;
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

        static void _Run(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(
                    "Usage: {0} <command>",
                    Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));
                Console.WriteLine();
                Console.WriteLine("where <command> is one of the following:");
                Console.WriteLine("  - generate-toc <project root folder> | Generate table of contents in all markdown files");
                throw new ProgramExitException(1);
            }

            switch (args[0])
            {
                case "generate-toc":
                    _GenerateToc(args.Skip(1).ToArray());
                    return;
                default:
                    throw new Exception("Unknown command.");
            }
        }

        static void _GenerateToc(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine(
                    "Usage: {0} generate-toc <project root folder>",
                    Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));

                throw new ProgramExitException(1);
            }

            string projectRootFolder = args[0];
            if (!Directory.Exists(projectRootFolder))
                throw new DirectoryNotFoundException(string.Format("Project root folder \"{0}\" does not exist.", projectRootFolder));

            projectRootFolder = Path.GetFullPath(projectRootFolder);

            var projects = _EnumerateProjectFolders(projectRootFolder).Select(ProjectSerializer.ReadProject);
            var toc = new TocDocument();

            var catalog = new TocCatalogNode(
                new Catalog("Gapotchenko.FX", projectRootFolder)
                {
                    ReadMeFilePath = Path.Combine(projectRootFolder, "README.md")
                });
            toc.Root.Children.Add(catalog);
            catalog.Parent = toc.Root;

            TocService.BuildToc(catalog, projects);

            Console.WriteLine("Table of Contents:");
            Console.WriteLine();
            TocVisualizer.Visualize(catalog, Console.Out);

            Console.WriteLine();
            _ProcessProjects(toc);

            Console.WriteLine();
            _ProcessCatalogs(toc);
        }

        static IEnumerable<string> _EnumerateProjectFolders(string rootPath)
        {
            var projectHierarchies = new (string Path, bool Recursive)[]
            {
                (".", false),
                ("Data", true),
            };

            var projectFolders = Enumerable.Empty<string>();

            foreach (var h in projectHierarchies)
            {
                var q =
                    Directory.EnumerateDirectories(
                        Path.Combine(rootPath, h.Path),
                        "*",
                        h.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Where(x => Path.GetFileName(x).Contains('.'));

                projectFolders = projectFolders.Concat(q);
            }

            return projectFolders.Where(ProjectSerializer.IsProjectFolder);
        }

        static void _ProcessProjects(TocDocument toc)
        {
            foreach (var node in toc.Root.Descendants().OfType<TocProjectNode>())
            {
                if (node.Project == null)
                    continue;

                Console.WriteLine("Processing project \"{0}\"...", node);

                var processor = new ProjectProcessor(node);
                processor.Run();
            }
        }

        static void _ProcessCatalogs(TocDocument toc)
        {
            foreach (var node in toc.Root.Descendants().OfType<TocCatalogNode>())
            {
                Console.WriteLine("Processing catalog \"{0}\"...", node.Catalog.Name);

                var catalogProcessor = new CatalogProcessor(node);
                catalogProcessor.Run();
            }
        }
    }
}
