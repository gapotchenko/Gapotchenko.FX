namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model
{
    sealed class Project
    {
        public string Name { get; init; } = "";

        public string FolderPath { get; init; } = "";

        public string? ReadMeFilePath { get; init; }

        public ProjectComplexity Complexity { get; init; }

        public override string ToString() => Name;
    }
}
