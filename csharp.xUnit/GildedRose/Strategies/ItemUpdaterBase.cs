using System;

using GildedRoseKata.Interfaces;

namespace GildedRoseKata.Strategies;

public abstract class ItemUpdaterBase : IItemUpdater
{
    public const int MinQuality = 0;
    public const int MaxQuality = 50;

    public abstract string Name { get; }

    public abstract void Update(Item item);

    protected static int ClampQuality(int quality) => Math.Clamp(quality, MinQuality, MaxQuality);
}
