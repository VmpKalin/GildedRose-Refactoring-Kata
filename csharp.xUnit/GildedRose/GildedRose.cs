using System.Collections.Generic;
using GildedRoseKata.Factories;
using GildedRoseKata.Interfaces;

namespace GildedRoseKata;

public class GildedRose
{
    private readonly IList<Item> _items;
    private readonly IItemUpdaterFactory _factory;

    public GildedRose(IList<Item> items) : this(items, ItemUpdaterFactory.Default())
    {
    }

    public GildedRose(IList<Item> items, IItemUpdaterFactory factory)
    {
        _items = items;
        _factory = factory;
    }

    public void UpdateQuality()
    {
        foreach (var item in _items)
            _factory.Get(item).AdvanceOneDay(item);
    }
}
