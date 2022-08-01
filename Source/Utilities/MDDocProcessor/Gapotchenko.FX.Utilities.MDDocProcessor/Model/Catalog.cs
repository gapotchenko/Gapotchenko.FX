namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model;

sealed class Catalog
{
    public Catalog(string name, string directoryPath)
    {
        Name = name;
        DirectoryPath = directoryPath;
    }

    public string Name { get; }

    public string DirectoryPath { get; }

    public string? ReadMeFilePath { get; init; }

    public override string ToString() => Name;
}
