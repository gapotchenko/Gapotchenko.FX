using Gapotchenko.FX.Utilities.MDDocProcessor.Model;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Visualization
{
    static class ProjectComplexityVisualizer
    {
        public static int GetStarCount(ProjectComplexity complexity) =>
            complexity switch
            {
                ProjectComplexity.Normal => 0,
                ProjectComplexity.Advanced => 1,
                ProjectComplexity.Expert => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(complexity))
            };

        public static char StarSymbol => '✱';
    }
}
