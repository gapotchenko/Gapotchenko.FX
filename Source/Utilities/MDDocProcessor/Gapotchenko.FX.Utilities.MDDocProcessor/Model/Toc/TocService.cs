using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Math.Graphs;
using Gapotchenko.FX.Utilities.MDDocProcessor.Framework;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc;

static class TocService
{
    public static void BuildToc(ITocNode root, IEnumerable<HierarchyItem> projects)
    {
        var items = projects.ToDictionary(x => x.Name, StringComparer.InvariantCulture);

        CreateNamespacesForOrphanProjects(items);

        var graph = new Graph<HierarchyItem>(
            items.Values,
            (a, b) => a.Name.StartsWith(b.Name + "."));

        graph.ReduceTransitions();

        var rootProjects = Sort(graph.Vertices.Where(x => !graph.OutgoingVerticesAdjacentTo(x).Any()));

        foreach (var node in rootProjects.Select(ConvertHierarchyItemToTocNode))
        {
            root.Children.Add((TocNode)node);
            AddSubProjects(node);
        }

        void AddSubProjects(ITocHierarchyItemNode node)
        {
            foreach (var subProject in Sort(graph.IncomingVerticesAdjacentTo(node.HierarchyItem)))
            {
                var subNode = ConvertHierarchyItemToTocNode(subProject);
                node.Children.Add((TocNode)subNode);

                AddSubProjects(subNode);
            }
        }

        BuildRelations(root);

        static IEnumerable<HierarchyItem> Sort(IEnumerable<HierarchyItem> source) => source.OrderNaturallyBy(x => x.Name);
    }

    static void CreateNamespacesForOrphanProjects(Dictionary<string, HierarchyItem> items)
    {
        var orphanNamespaces = new HashSet<string>(StringComparer.InvariantCulture);

        foreach (var project in items.Values.ToList())
        {
            string? parentName = GetParentName(project.Name);
            if (parentName == null)
                continue;

            if (items.ContainsKey(parentName))
                continue;

            if (!orphanNamespaces.Add(parentName))
                items.Add(parentName, new Namespace(parentName));
        }
    }

    static string? GetParentName(string name)
    {
        int j = name.LastIndexOf('.');
        if (j == -1)
            return null;

        var parentName = name.AsSpan(0, j);
        if (!parentName.Contains('.'))
            return null;

        return parentName.ToString();
    }

    static ITocHierarchyItemNode ConvertHierarchyItemToTocNode(HierarchyItem item) =>
        item switch
        {
            Project p => new TocProjectNode(p),
            Namespace ns => new TocNamespaceNode(ns),
            _ => throw new InvalidOperationException()
        };

    static void BuildRelations(ITocNode node)
    {
        foreach (var i in node.Children)
            BuildRelations(i, node);

        static void BuildRelations(ITocNode node, ITocNode? parent)
        {
            node.Parent = parent;
            foreach (var i in node.Children)
                BuildRelations(i, node);
        }
    }
}
