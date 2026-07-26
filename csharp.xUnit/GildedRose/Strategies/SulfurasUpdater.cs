namespace GildedRoseKata.Strategies;

public class SulfurasUpdater : ItemUpdaterBase
{
    public override string Name => "Sulfuras, Hand of Ragnaros";

    public override void Update(Item item)
    {
        // Legendary item: Quality stays at 80 and SellIn never changes.
    }
}
