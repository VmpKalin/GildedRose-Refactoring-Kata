namespace GildedRoseKata.Interfaces;

public interface IItemUpdaterFactory
{
    IItemUpdater Get(Item item);
}
