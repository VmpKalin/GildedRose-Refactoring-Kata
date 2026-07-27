# Gilded Rose starting position in C# xUnit

Solution for the classic [Gilded Rose refactoring kata](https://github.com/emilybache/GildedRose-Refactoring-Kata):
the legacy `UpdateQuality` method was refactored into a Strategy + Factory design,
and the new **"Conjured" items** feature was added on top of the cleaned-up code.

## Requirements

- [.NET SDK](https://dotnet.microsoft.com/download) 6.0+ (any recent SDK works)

## How to run

Build the solution:

```bash
dotnet build
```

Run the console simulation (prints the inventory state day by day):

```bash
dotnet run --project GildedRose            # default: 2 days
dotnet run --project GildedRose -- 30      # simulate 30 days
```

Run all tests (unit tests + approval test):

```bash
dotnet test
```

The test suite contains two layers:

- **Approval test** (`ApprovalTest.ThirtyDays`) — captures the full console output of a
  30-day simulation and compares it against the approved snapshot (`*.verified.txt`).
  It acts as a safety net: any unintended behaviour change in any item type breaks it.
- **Unit tests** (`GildedRoseTest`) — specification-based tests that pin every business
  rule with explicit numbers (degradation rates, the 0/50 boundaries, Backstage
  thresholds, Sulfuras immutability, Conjured double degradation).

## Project structure

```
GildedRose/
├── GildedRose.cs            # thin orchestrator: one loop, no business rules
├── Item.cs                  # untouched (per kata constraints)
├── Program.cs               # untouched (its output is the approval baseline)
├── Interfaces/
│   ├── IItemUpdater.cs      # strategy contract: applies one full day to one item
│   └── IItemUpdaterFactory.cs
├── Factories/
│   └── ItemUpdaterFactory.cs  # resolves item -> strategy (exact name, then prefix, then fallback)
└── Strategies/
    ├── ItemUpdaterBase.cs     # shared [0, 50] quality clamp
    ├── DefaultItemUpdater.cs   # -1 per day, -2 after sell-by (fallback strategy)
    ├── AgedBrieUpdater.cs     # +1 per day, +2 after sell-by
    ├── SulfurasUpdater.cs     # legendary: no-op (Quality 80, SellIn frozen)
    ├── BackstagePassUpdater.cs# +1 / +2 (<=10) / +3 (<=5), 0 after the concert
    └── ConjuredItemUpdater.cs # -2 per day, -4 after sell-by
```

## Design decisions and rationale

### 1. Tests before refactoring (characterization first)

The legacy code had no meaningful tests, so the very first step was to lock down the
*current* behaviour, not the desired one:

1. The 30-day approval snapshot was approved **as-is**, including the still-broken
   Conjured behaviour. This gave a byte-exact safety net for the whole refactoring.
2. Specification unit tests were added for every existing item type; they all passed
   against the legacy code, confirming my reading of it.
3. Failing (red) unit tests were written for the Conjured rule as the target.

Only then was the code refactored — under green tests all the way. The approval
snapshot was re-approved exactly once, at the very end, after verifying the diff
contained **only** the intended Conjured lines.

### 2. Strategy pattern instead of the nested-if block

Each item category gets its own small class implementing `IItemUpdater`. One call
applies one full in-game day to one item (quality change, SellIn decrement, clamping) —
so `UpdateQuality` collapses to a single readable loop with zero business logic in it.

Why not polymorphic `Item` subclasses (`AgedBrie : Item`)? The kata forbids touching
the `Item` class, and `Program.cs` instantiates plain `Item` objects by name. All
behaviour therefore lives *outside* `Item`, keyed by `Item.Name`.

### 3. Stateless, shared strategy instances

Strategies hold no mutable state — all data lives in the `Item` passed as an argument.
Two instances of the same strategy are indistinguishable, so each strategy is created
once and reused for every matching item, instead of being allocated per item/per day.
This is safe exactly as long as strategies stay stateless (documented as a contract).

### 4. Factory with two-tier resolution

The factory maps an item to its strategy in a fixed order:

1. **Exact name match** — dictionary lookup keyed by the strategy's `Name`
   (e.g. `"Aged Brie"`, `"Sulfuras, Hand of Ragnaros"`).
2. **Prefix match for Conjured** — `Name.StartsWith("Conjured")`. This is required
   because Conjured is a *family* of items ("Conjured Mana Cake", potentially
   "Conjured Sword", …) with no single fixed full name, so it cannot be a dictionary key.
3. **Fallback** — the normal-item strategy handles everything unmatched.

The prefix knowledge deliberately lives in the factory: "how to find a strategy" is
routing logic, which is the factory's single responsibility. Strategies know *what to
do*, the factory knows *who handles whom*. Generalising the prefix mechanism (e.g. an
`IPrefixMatchingUpdater` interface) was considered and rejected as speculative
generality — there is exactly one prefix-matched family in the requirements. If a
second family appears, that is the moment to generalise.

### 5. Strategy list is injected into the factory

The factory takes `IEnumerable<IItemUpdater>` via its constructor; the canonical set is
assembled in one place (`ItemUpdaterFactory.Default()`). There is no DI container in a
console kata — constructor injection by hand achieves the same principle: the factory
is closed for modification but the strategy set is defined externally and testable.

### 6. Backwards-compatible wiring

`GildedRose` keeps its original one-argument constructor (used by `Program.cs` and the
approval test) and delegates to a new overload that accepts a factory:

```csharp
public GildedRose(IList<Item> items) : this(items, ItemUpdaterFactory.Default()) { }
public GildedRose(IList<Item> items, IItemUpdaterFactory factory) { ... }
```

Nothing existing breaks; unit tests can inject a custom factory when isolation is needed.

### 7. Shared quality clamp in one place

The `[0, 50]` boundary lives in `ItemUpdaterBase.ClampQuality` and nowhere else, so no
strategy can forget the floor or the cap. Sulfuras never goes through the clamp — it is
a no-op strategy, which is precisely how it legitimately stays at Quality 80.

### 8. Preserving legacy ordering semantics

The original code changes quality **before** decrementing SellIn, and applies the
"expired" acceleration only when SellIn is negative *after* the decrement. Every
strategy reproduces this exact ordering. This matters at the boundaries: an item with
`SellIn = 0` is on its **last valid day** (the concert is *today*), and the doubled
rates / backstage zeroing kick in only on the transition to `SellIn = -1`. Reordering
these steps would shift every boundary by one day and break the approval snapshot.

### 9. Conjured rule interpretation

"Degrades twice as fast" is applied in **both** modes: −2 per day before the sell-by
date, −4 after it (normal items lose 1 and 2 respectively). Quality still never drops
below 0.

Two deliberate edge-case decisions, both pinned by tests:

- **Case-insensitive matching**: `"conjured broom"` is treated as Conjured
  (`OrdinalIgnoreCase`). The legacy code was case-sensitive everywhere, but Conjured is
  a brand-new feature, so this is a new contract, chosen for robustness against sloppy
  inventory data.
- **Prefix precision**: only names starting with the exact word `"Conjured"` match —
  `"Conjurer's Hat"` is a normal item. Verified by a negative test.
