namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model;

abstract class HierarchyItem(string name)
{
    public string Name { get; } = name;

    public override string ToString() => Name;
}
