namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model;

sealed class Project : HierarchyItem
{
    public Project(string name, string directoryPath) :
        base(name)
    {
        DirectoryPath = directoryPath;
    }

    public string DirectoryPath { get; }

    public string? ReadMeFilePath { get; init; }

    public ProjectComplexity Complexity { get; init; }
}
