namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model;

sealed class Project(string name, string directoryPath) : HierarchyItem(name)
{
    public string DirectoryPath { get; } = directoryPath;

    public string? ReadMeFilePath { get; init; }

    public ProjectComplexity Complexity { get; init; }
}
