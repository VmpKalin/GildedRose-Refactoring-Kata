using GildedRoseKata.Factories;

namespace GildedRoseKata.Strategies;

public class ConjuredItemUpdater : ItemUpdaterBase
{
    // Matched by prefix in ItemUpdaterFactory: handles every "Conjured..." item.
    public override string Name => ItemUpdaterFactory.ConjuredPrefix;

    public override void Update(Item item)
    {
        item.Quality = ClampQuality(item.Quality - 2);
        item.SellIn -= 1;
        if (item.SellIn < 0)
        {
            item.Quality = ClampQuality(item.Quality - 2);
        }
    }
}
