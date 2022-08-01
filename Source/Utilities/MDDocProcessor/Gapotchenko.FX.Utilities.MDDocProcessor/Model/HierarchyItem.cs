namespace Gapotchenko.FX.Utilities.MDDocProcessor.Model;

abstract class HierarchyItem
{
    protected HierarchyItem(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override string ToString() => Name;
}
