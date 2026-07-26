namespace GildedRoseKata.Interfaces;

public interface IItemUpdater
{
    /// <summary>The exact item name this strategy handles ("default" for the fallback).</summary>
    string Name { get; }

    /// <summary>Applies one full day: quality change + SellIn change + clamping.</summary>
    void AdvanceOneDay(Item item);
}
