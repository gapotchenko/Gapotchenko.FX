namespace Gapotchenko.FX.Data.Dot.Dom
{
    interface IDotSyntaxSlotProvider
    {
        int SlotCount { get; }
        DotSyntaxSlot GetSlot(int index);
    }
}
