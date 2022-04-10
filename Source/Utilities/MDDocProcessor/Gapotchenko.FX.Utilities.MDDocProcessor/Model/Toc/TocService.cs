using Gapotchenko.FX.Math.Topology;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc
{
    static class TocService
    {
        static IEnumerable<T> NaturalSortBy<T>(IEnumerable<T> list, Func<T, string> keySelector)
        {
            if (!list.Any())
                return Enumerable.Empty<T>();

            int maxLen = list.Select(s => keySelector(s).Length).Max();

            static char PaddingChar(string s) => char.IsDigit(s[0]) ? ' ' : char.MaxValue;

            return list
                .Select(s =>
                    new
                    {
                        OrgStr = s,
                        SortStr = Regex.Replace(
                            keySelector(s),
                            @"(\d+)",
                            m => m.Value.PadLeft(maxLen, PaddingChar(m.Value)))
                    })
                .OrderBy(x => x.SortStr)
                .Select(x => x.OrgStr);
        }

        public static void BuildToc(TocNode root, IEnumerable<Project> projects)
        {
            if (projects == null)
                throw new ArgumentNullException(nameof(projects));

            static IEnumerable<Project> Sort(IEnumerable<Project> source) => NaturalSortBy(source, x => x.Name);

            var graph = new Graph<Project>(projects, (a, b) => a.Name.StartsWith(b.Name + "."));
            graph.ReduceTransitions();

            var rootProjects = Sort(graph.Vertices.Where(x => !graph.DestinationVerticesAdjacentTo(x).Any())).ToList();
            graph.Transpose();

            void AddSubProjects(TocProjectNode node)
            {
                foreach (var subProject in Sort(graph.DestinationVerticesAdjacentTo(node.Project)))
                {
                    var subNode = new TocProjectNode(subProject);
                    node.Children.Add(subNode);

                    AddSubProjects(subNode);
                }
            }

            foreach (var node in rootProjects.Select(x => new TocProjectNode(x)))
            {
                root.Children.Add(node);
                AddSubProjects(node);
            }

            BuildRelations(root);

            _CreateProjectGroups(root);

            BuildRelations(root);
        }

        static void BuildRelations(TocNode node)
        {
            foreach (var i in node.Children)
                BuildRelations(i, node);
        }

        static void BuildRelations(TocNode node, TocNode? parent)
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
            return name[..j];
        }

        static void _CreateProjectGroups(TocNode node)
        {
            var graph = new Graph<string>(StringComparer.Ordinal);

            var projects = new Dictionary<string, TocProjectNode>(StringComparer.Ordinal);

            foreach (var i in node.Descendants().OfType<TocProjectNode>())
            {
                string projectName = i.Project.Name;
                projects.Add(projectName, i);

                string? parentName = _GetParentName(projectName);
                if (parentName == null)
                    continue;

                graph.Edges.Add(projectName, parentName);
            }

            graph.Transpose();

            foreach (var v in graph.Vertices.ToList())
            {
                var adj = graph.DestinationVerticesAdjacentTo(v);
                if (adj.Count() > 1)
                {
                    if (!projects.ContainsKey(v))
                    {
                        var groupNode = new TocNamespaceNode(v);

                        foreach (var i in adj)
                        {
                            var projectNode = projects[i];
                            groupNode.Children.Add(projectNode);

                            var parent = projectNode.Parent;
                            if (parent != null)
                            {
                                int j = parent.Children.IndexOf(projectNode);
                                parent.Children.Remove(projectNode);

                                if (groupNode.Parent == null)
                                {
                                    parent.Children.Insert(j, groupNode);
                                    groupNode.Parent = parent;
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
