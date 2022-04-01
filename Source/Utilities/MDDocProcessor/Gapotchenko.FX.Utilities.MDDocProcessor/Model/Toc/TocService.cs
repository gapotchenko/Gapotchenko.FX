using Gapotchenko.FX.Math.Topology;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model.Toc
{
    static class TocService
    {
        public static TocDocument BuildToc(IEnumerable<Project> projects)
        {
            if (projects == null)
                throw new ArgumentNullException(nameof(projects));

            IEnumerable<Project> Sort(IEnumerable<Project> source) => source.OrderBy(x => x.Name);

            var toc = new TocDocument();

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
                toc.Root.Children.Add(node);
                AddSubProjects(node);
            }

            void BuildRelations(TocNode node, TocNode? parent)
            {
                node.Parent = parent;
                foreach (var i in node.Children)
                    BuildRelations(i, node);
            }

            BuildRelations(toc.Root, null);

            _CreateProjectGroups(toc);

            BuildRelations(toc.Root, null);

            return toc;
        }

        static string? _GetParentName(string name)
        {
            int j = name.LastIndexOf('.');
            if (j == -1)
                return null;
            return name.Substring(0, j);
        }

        static void _CreateProjectGroups(TocDocument toc)
        {
            var graph = new Graph<string>(StringComparer.Ordinal);

            var projects = new Dictionary<string, TocProjectNode>(StringComparer.Ordinal);

            foreach (var i in toc.Root.Descendants().OfType<TocProjectNode>())
            {
                string projectName = i.Project.Name;
                projects.Add(projectName, i);

                string? parentName = _GetParentName(projectName);
                if (parentName == null)
                    continue;

                graph.Edges.Add(projectName, parentName);
            }

            graph.Transpose();

            var projectGroups = new Dictionary<string, List<Project>>(StringComparer.Ordinal);

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
