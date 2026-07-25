using Xunit;
using System.Collections.Generic;
using GildedRoseKata;

namespace GildedRoseTests;

public class GildedRoseTest
{
    private const string AgedBrie = "Aged Brie";
    private const string Sulfuras = "Sulfuras, Hand of Ragnaros";
    private const string BackstagePass = "Backstage passes to a TAFKAL80ETC concert";

    private static Item UpdateOne(string name, int sellIn, int quality)
    {
        var item = new Item { Name = name, SellIn = sellIn, Quality = quality };
        var app = new GildedRose(new List<Item> { item });
        app.UpdateQuality();
        return item;
    }

    [Fact]
    public void UpdateQuality_DoesNotChangeItemName()
    {
        var item = UpdateOne("foo", 0, 0);
        Assert.Equal("foo", item.Name);
    }

    // ---------- Normal items ----------

    [Fact]
    public void NormalItem_BeforeSellBy_QualityDropsByOne()
    {
        var item = UpdateOne("+5 Dexterity Vest", 10, 20);
        Assert.Equal(9, item.SellIn);
        Assert.Equal(19, item.Quality);
    }

    [Fact]
    public void NormalItem_OnSellByDate_QualityDropsByTwo()
    {
        var item = UpdateOne("+5 Dexterity Vest", 0, 20);
        Assert.Equal(-1, item.SellIn);
        Assert.Equal(18, item.Quality);
    }

    [Fact]
    public void NormalItem_AfterSellBy_QualityDropsByTwo()
    {
        var item = UpdateOne("+5 Dexterity Vest", -1, 20);
        Assert.Equal(-2, item.SellIn);
        Assert.Equal(18, item.Quality);
    }

    [Fact]
    public void NormalItem_QualityNeverNegative()
    {
        var item = UpdateOne("+5 Dexterity Vest", 5, 0);
        Assert.Equal(0, item.Quality);
    }

    [Fact]
    public void NormalItem_AfterSellByWithQualityOne_QualityStopsAtZero()
    {
        var item = UpdateOne("+5 Dexterity Vest", 0, 1);
        Assert.Equal(0, item.Quality);
    }

    // ---------- Aged Brie ----------

    [Fact]
    public void AgedBrie_BeforeSellBy_QualityIncreasesByOne()
    {
        var item = UpdateOne(AgedBrie, 2, 0);
        Assert.Equal(1, item.SellIn);
        Assert.Equal(1, item.Quality);
    }

    [Fact]
    public void AgedBrie_AfterSellBy_QualityIncreasesByTwo()
    {
        var item = UpdateOne(AgedBrie, 0, 10);
        Assert.Equal(-1, item.SellIn);
        Assert.Equal(12, item.Quality);
    }

    [Fact]
    public void AgedBrie_QualityNeverExceedsFifty()
    {
        var item = UpdateOne(AgedBrie, 5, 50);
        Assert.Equal(50, item.Quality);
    }

    [Fact]
    public void AgedBrie_AfterSellByAtFortyNine_QualityCapsAtFifty()
    {
        var item = UpdateOne(AgedBrie, 0, 49);
        Assert.Equal(50, item.Quality);
    }

    // ---------- Sulfuras ----------

    [Fact]
    public void Sulfuras_AtSellInZero_NeverChanges()
    {
        var item = UpdateOne(Sulfuras, 0, 80);
        Assert.Equal(0, item.SellIn);
        Assert.Equal(80, item.Quality);
    }

    [Fact]
    public void Sulfuras_AtSellInMinusOne_NeverChanges()
    {
        var item = UpdateOne(Sulfuras, -1, 80);
        Assert.Equal(-1, item.SellIn);
        Assert.Equal(80, item.Quality);
    }

    // ---------- Backstage passes ----------

    [Fact]
    public void BackstagePass_MoreThanTenDays_QualityIncreasesByOne()
    {
        var item = UpdateOne(BackstagePass, 11, 20);
        Assert.Equal(10, item.SellIn);
        Assert.Equal(21, item.Quality);
    }

    [Fact]
    public void BackstagePass_TenDaysOrLess_QualityIncreasesByTwo()
    {
        var item = UpdateOne(BackstagePass, 10, 20);
        Assert.Equal(9, item.SellIn);
        Assert.Equal(22, item.Quality);
    }

    [Fact]
    public void BackstagePass_SixDays_QualityIncreasesByTwo()
    {
        var item = UpdateOne(BackstagePass, 6, 20);
        Assert.Equal(22, item.Quality);
    }

    [Fact]
    public void BackstagePass_FiveDaysOrLess_QualityIncreasesByThree()
    {
        var item = UpdateOne(BackstagePass, 5, 20);
        Assert.Equal(4, item.SellIn);
        Assert.Equal(23, item.Quality);
    }

    [Fact]
    public void BackstagePass_AfterConcert_QualityDropsToZero()
    {
        var item = UpdateOne(BackstagePass, 0, 20);
        Assert.Equal(-1, item.SellIn);
        Assert.Equal(0, item.Quality);
    }

    [Fact]
    public void BackstagePass_AtFortyNineCloseToConcert_QualityCapsAtFifty()
    {
        var item = UpdateOne(BackstagePass, 5, 49);
        Assert.Equal(50, item.Quality);
    }
}
