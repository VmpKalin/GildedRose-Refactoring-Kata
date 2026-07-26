namespace GildedRoseKata.Strategies;

public class BackstagePassUpdater : ItemUpdaterBase
{
    public override string Name => "Backstage passes to a TAFKAL80ETC concert";

    public override void AdvanceOneDay(Item item)
    {
        item.Quality = ClampQuality(item.Quality + IncreaseFor(item.SellIn));
        item.SellIn -= 1;
        if (item.SellIn < 0)
        {
            item.Quality = 0;
        }
    }
    
    private static int IncreaseFor(int sellIn)
    {
        if (sellIn <= 5)  return 3;
        if (sellIn <= 10) return 2;
        return 1;
    }
}
