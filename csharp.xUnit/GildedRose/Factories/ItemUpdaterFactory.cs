using System;
using System.Collections.Generic;
using GildedRoseKata.Interfaces;
using GildedRoseKata.Strategies;

namespace GildedRoseKata.Factories;

public class ItemUpdaterFactory : IItemUpdaterFactory
{
    public const string ConjuredPrefix = "Conjured";
    private const string DefaultName = "default";

    private readonly Dictionary<string, IItemUpdater> _updatersByName;
    private readonly IItemUpdater _fallback;

    public ItemUpdaterFactory(IEnumerable<IItemUpdater> updaters)
    {
        _updatersByName = new Dictionary<string, IItemUpdater>(StringComparer.OrdinalIgnoreCase);
        foreach (var updater in updaters)
        {
            _updatersByName[updater.Name] = updater;
        }

        if (!_updatersByName.TryGetValue(DefaultName, out _fallback))
        {
            throw new ArgumentException($"A \"{DefaultName}\" fallback updater is required.", nameof(updaters));
        }
    }

    public static ItemUpdaterFactory Default() => new([
        new DefaultItemUpdater(),
        new AgedBrieUpdater(),
        new SulfurasUpdater(),
        new BackstagePassUpdater(),
        new ConjuredItemUpdater()
    ]);

    public IItemUpdater Get(Item item)
    {
        if (_updatersByName.TryGetValue(item.Name, out var updater))
        {
            return updater;
        }

        if (item.Name.StartsWith(ConjuredPrefix, StringComparison.OrdinalIgnoreCase) &&
            _updatersByName.TryGetValue(ConjuredPrefix, out var conjured))
        {
            return conjured;
        }

        return _fallback;
    }
}
