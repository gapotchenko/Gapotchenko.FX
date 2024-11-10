namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model;

sealed class Catalog(string name, string directoryPath)
{
    public string Name { get; } = name;

    public string DirectoryPath { get; } = directoryPath;

    public string? ReadMeFilePath { get; init; }

    public override string ToString() => Name;
}
