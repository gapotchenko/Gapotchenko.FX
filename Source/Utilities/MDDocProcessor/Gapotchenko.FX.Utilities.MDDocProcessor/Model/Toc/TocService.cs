using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Math.Topology;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc
{
    static class TocService
    {
        static IEnumerable<T> OrderNaturallyBy<T>(IEnumerable<T> source, Func<T, string> keySelector)
        {
            source = source.Memoize();

            if (!source.Any())
                return Enumerable.Empty<T>();

            int totalWidth = source.Select(value => keySelector(value).Length).Max();

            static char PaddingChar(string s) => char.IsDigit(s[0]) ? ' ' : char.MaxValue;

            return source
                .Select(value =>
                    (
                        Value: value,
                        Key: Regex.Replace(
                            keySelector(value),
                            @"(\d+)",
                            m => m.Value.PadLeft(totalWidth, PaddingChar(m.Value)))
                    ))
                .OrderBy(x => x.Key)
                .Select(x => x.Value);
        }

        public static void BuildToc(ITocNode root, IEnumerable<Project> projects)
        {
            if (projects == null)
                throw new ArgumentNullException(nameof(projects));

            var items = projects.Cast<HierarchyItem>().ToDictionary(x => x.Name, StringComparer.InvariantCulture);

            CreateNamespacesForOrphanProjects(items);

            static IEnumerable<HierarchyItem> Sort(IEnumerable<HierarchyItem> source) => OrderNaturallyBy(source, x => x.Name);

            var graph = new Graph<HierarchyItem>(items.Values, (a, b) => a.Name.StartsWith(b.Name + "."));
            graph.ReduceTransitions();

            var rootProjects = Sort(graph.Vertices.Where(x => !graph.DestinationVerticesAdjacentTo(x).Any())).ToList();
            graph.Transpose();

            void AddSubProjects(ITocHierarchyItemNode node)
            {
                foreach (var subProject in Sort(graph.DestinationVerticesAdjacentTo(node.HierarchyItem)))
                {
                    var subNode = ConvertHierarchyItemToTocNode(subProject);
                    node.Children.Add((TocNode)subNode);

                    AddSubProjects(subNode);
                }
            }

            foreach (var node in rootProjects.Select(x => ConvertHierarchyItemToTocNode(x)))
            {
                root.Children.Add((TocNode)node);
                AddSubProjects(node);
            }

            BuildRelations(root);
        }

        static void CreateNamespacesForOrphanProjects(Dictionary<string, HierarchyItem> items)
        {
            var orphanNamespaces = new HashSet<string>(StringComparer.InvariantCulture);

            foreach (var project in items.Values.ToList())
            {
                string? parentName = _GetParentName(project.Name);
                if (parentName == null)
                    continue;

                if (items.ContainsKey(parentName))
                    continue;

                if (!orphanNamespaces.Add(parentName))
                    items.Add(parentName, new Namespace(parentName));
            }
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
        }

        static void BuildRelations(ITocNode node, ITocNode? parent)
        {
            node.Parent = parent;
            foreach (var i in node.Children)
                BuildRelations(i, node);
        }

        static string? _GetParentName(string name)
        {
            int j = name.LastIndexOf('.');
            if (j == -1)
                return null;

            var parentName = name.AsSpan(0, j);
            if (!parentName.Contains('.'))
                return null;

            return parentName.ToString();
        }
    }
}
