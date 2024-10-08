using Gapotchenko.FX.Utilities.MDDocProcessor.Model;
using Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;
using Gapotchenko.FX.Utilities.MDDocProcessor.Visualization;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Commands.GenerateToc;

static class GenerateTocCommand
{
    public static void Run(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine(
                "Usage: {0} generate-toc <project root directory>",
                Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));

            throw new ProgramExitException(1);
        }

        string projectRootDirectory = args[0];
        if (!Directory.Exists(projectRootDirectory))
            throw new DirectoryNotFoundException(string.Format("Project root directory \"{0}\" does not exist.", projectRootDirectory));

        projectRootDirectory = Path.GetFullPath(projectRootDirectory);

        var projects = EnumerateProjectDirectories(projectRootDirectory).Select(ProjectSerializer.ReadProject);
        var toc = new TocDocument();

        var catalog = new TocCatalogNode(
            new Catalog("Gapotchenko.FX", projectRootDirectory)
            {
                ReadMeFilePath = Path.Combine(projectRootDirectory, "Modules/README.md")
            });
        toc.Root.Children.Add(catalog);
        catalog.Parent = toc.Root;

        TocService.BuildToc(catalog, projects);

        Console.WriteLine("Table of Contents:");
        Console.WriteLine();
        TocVisualizer.Visualize(catalog, Console.Out);

        Console.WriteLine();
        ProcessProjects(toc);

        Console.WriteLine();
        ProcessCatalogs(toc);
    }

    static IEnumerable<string> EnumerateProjectDirectories(string rootPath)
    {
        var hierarchies = new (string Path, bool Recursive)[]
        {
            ("Modules/Catalog", true),
            ("Profiles", false)
        };

        var directories = Enumerable.Empty<string>();

        foreach (var hierarchy in hierarchies)
        {
            var query =
                Directory.EnumerateDirectories(
                    Path.Combine(rootPath, hierarchy.Path),
                    "*",
                    hierarchy.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Where(x => Path.GetFileName(x).Contains('.'));

            directories = directories.Concat(query);
        }

        return directories.Where(ProjectSerializer.IsProjectDirectory);
    }

    static void ProcessProjects(TocDocument toc)
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

    static void ProcessCatalogs(TocDocument toc)
    {
        foreach (var node in toc.Root.Descendants().OfType<TocCatalogNode>())
        {
            Console.WriteLine("Processing catalog \"{0}\"...", node.Catalog.Name);

            var catalogProcessor = new CatalogProcessor(node);
            catalogProcessor.Run();
        }
    }
}
