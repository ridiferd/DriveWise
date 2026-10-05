using DriveWise.Core.FuelEconomy;

namespace DriveWise.Tests;

public class FuelEconomyParserTests
{
    [Fact]
    public void ParseMenu_reads_array_of_items()
    {
        var items = FuelEconomyParser.ParseMenu("""{"menuItem":[{"text":"2025","value":"2025"},{"text":"2024","value":"2024"}]}""");
        Assert.Equal(2, items.Count);
        Assert.Equal("2025", items[0].Value);
    }

    [Fact]
    public void ParseMenu_reads_single_object_as_one_item()
    {
        // fueleconomy.gov returns an object, not an array, when there is exactly one option
        var items = FuelEconomyParser.ParseMenu("""{"menuItem":{"text":"Auto (AV), 4 cyl, 2.0 L","value":"48001"}}""");
        Assert.Single(items);
        Assert.Equal("48001", items[0].Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("null")]
    [InlineData("{}")]
    public void ParseMenu_returns_empty_for_missing_data(string json)
    {
        Assert.Empty(FuelEconomyParser.ParseMenu(json));
    }

    [Fact]
    public void ParseVehicle_handles_numbers_as_strings_and_empty_values()
    {
        var v = FuelEconomyParser.ParseVehicle("""
            {"id":"48001","year":"2025","make":"Toyota","model":"Prius","comb08":"57","city08":"57",
             "highway08":"56","fuelCost08":"900","co2TailpipeGpm":"156.0","cylinders":"","displ":"2.0",
             "atvType":"Hybrid","fuelType":"Regular","feScore":"10","ghgScore":"","range":"0"}
            """);
        Assert.NotNull(v);
        Assert.Equal(57, v!.CombinedMpg);
        Assert.Equal(156.0, v.Co2GramsPerMile);
        Assert.Null(v.Cylinders);
        Assert.Equal(-1, v.GreenhouseGasScore);
        Assert.True(v.IsHybrid);
    }

    [Fact]
    public void ParseVehicle_accepts_native_json_numbers()
    {
        var v = FuelEconomyParser.ParseVehicle("""{"id":1,"year":2025,"make":"Tesla","model":"Model 3","comb08":132,"atvType":"EV","range":272}""");
        Assert.True(v!.IsElectric);
        Assert.Equal(272, v.ElectricRangeMiles);
    }

    [Fact]
    public void ParseVehicle_returns_null_without_id()
    {
        Assert.Null(FuelEconomyParser.ParseVehicle("""{"make":"Nope"}"""));
    }
}
