namespace GildedRoseKata.Strategies;

public class DefaultItemUpdater : ItemUpdaterBase
{
    public override string Name => "default";

    public override void AdvanceOneDay(Item item)
    {
        item.Quality = ClampQuality(item.Quality - 1);
        item.SellIn -= 1;
        if (item.SellIn < 0)
        {
            item.Quality = ClampQuality(item.Quality - 1);
        }
    }
}
