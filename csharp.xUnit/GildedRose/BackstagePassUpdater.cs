namespace GildedRoseKata;

public class BackstagePassUpdater : ItemUpdaterBase
{
    public override string Name => "Backstage passes to a TAFKAL80ETC concert";

    public override void Update(Item item)
    {
        var increase = item.SellIn <= 5 ? 3 : item.SellIn <= 10 ? 2 : 1;
        item.Quality = ClampQuality(item.Quality + increase);
        item.SellIn -= 1;
        if (item.SellIn < 0)
        {
            item.Quality = 0;
        }
    }
}
