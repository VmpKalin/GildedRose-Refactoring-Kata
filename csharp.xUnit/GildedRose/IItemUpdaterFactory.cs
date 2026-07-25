namespace GildedRoseKata;

public interface IItemUpdaterFactory
{
    IItemUpdater Get(Item item);
}
