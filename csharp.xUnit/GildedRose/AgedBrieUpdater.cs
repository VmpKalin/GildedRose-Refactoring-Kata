namespace GildedRoseKata;

public class AgedBrieUpdater : ItemUpdaterBase
{
    public override string Name => "Aged Brie";

    public override void Update(Item item)
    {
        item.Quality = ClampQuality(item.Quality + 1);
        item.SellIn -= 1;
        if (item.SellIn < 0)
        {
            item.Quality = ClampQuality(item.Quality + 1);
        }
    }
}
