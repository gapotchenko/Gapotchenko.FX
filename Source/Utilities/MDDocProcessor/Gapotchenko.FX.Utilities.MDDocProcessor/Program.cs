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
                Console.WriteLine(e);
                return 1;
            }
        }

        static void _Run(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine(
                    "Usage: {0} <project root folder>",
                    Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));

                Environment.ExitCode = 1;
                return;
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
            _ProcessCatalog(toc, projectRootFolder);

            Console.WriteLine();
            _ProcessProjects(toc, projectRootFolder);
        }

        static void _ProcessCatalog(TocDocument toc, string rootFolder)
        {
            Console.WriteLine("Processing catalog...");
            var catalogProcessor = new CatalogProcessor(rootFolder, toc);
            catalogProcessor.Run();
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

        static void _ProcessProjects(TocDocument toc, string baseDirectory)
        {
            foreach (var node in toc.Root.Descendants().OfType<TocProjectNode>())
            {
                if (node.Project == null)
                    continue;

                Console.WriteLine("Processing project \"{0}\"...", node);

                var processor = new ProjectProcessor(node, baseDirectory);
                processor.Run();
            }
        }
    }
}
