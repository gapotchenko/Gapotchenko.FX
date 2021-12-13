namespace Gapotchenko.FX.Data.Dot.Dom
{
    interface IDotSyntaxSlotProvider
    {
        int SlotCount { get; }
        IDotSyntaxSlotProvider? GetSlot(int index);
    }
}
