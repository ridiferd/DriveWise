using System.Globalization;
using System.Text.Json;
using DriveWise.Core.Models;

namespace DriveWise.Core.FuelEconomy;

/// <summary>
/// Tolerant JSON parser for fueleconomy.gov responses.
/// The service returns numbers as strings, may return empty strings,
/// and returns a single object instead of an array when a menu has one item.
/// </summary>
public static class FuelEconomyParser
{
    public static IReadOnlyList<MenuItem> ParseMenu(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json.Trim() == "null") return [];
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("menuItem", out var items)) return [];

        var list = new List<MenuItem>();
        if (items.ValueKind == JsonValueKind.Array)
            foreach (var item in items.EnumerateArray()) list.Add(ToMenuItem(item));
        else if (items.ValueKind == JsonValueKind.Object)
            list.Add(ToMenuItem(items));
        return list;
    }

    public static Vehicle? ParseVehicle(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json.Trim() == "null") return null;
        using var doc = JsonDocument.Parse(json);
        return ParseVehicle(doc.RootElement);
    }

    public static Vehicle? ParseVehicle(JsonElement e)
    {
        var id = Int(e, "id");
        if (id <= 0) return null;
        return new Vehicle
        {
            Id = id,
            Year = Int(e, "year"),
            Make = Str(e, "make"),
            Model = Str(e, "model"),
            VehicleClass = Str(e, "VClass"),
            Drive = Str(e, "drive"),
            Transmission = Str(e, "trany"),
            FuelType = Str(e, "fuelType"),
            AlternativeFuelType = Str(e, "atvType"),
            Cylinders = NullableDouble(e, "cylinders"),
            DisplacementLiters = NullableDouble(e, "displ"),
            CityMpg = Int(e, "city08"),
            HighwayMpg = Int(e, "highway08"),
            CombinedMpg = Int(e, "comb08"),
            AnnualFuelCostUsd = Int(e, "fuelCost08"),
            Co2GramsPerMile = Double(e, "co2TailpipeGpm"),
            FuelEconomyScore = Int(e, "feScore", -1),
            GreenhouseGasScore = Int(e, "ghgScore", -1),
            FiveYearSaveSpendUsd = Int(e, "youSaveSpend"),
            ElectricRangeMiles = Int(e, "range"),
        };
    }

    private static MenuItem ToMenuItem(JsonElement item) => new(Str(item, "text"), Str(item, "value"));

    private static string Str(JsonElement e, string name) =>
        e.TryGetProperty(name, out var p) && p.ValueKind != JsonValueKind.Null
            ? (p.ValueKind == JsonValueKind.String ? p.GetString() ?? "" : p.GetRawText())
            : "";

    private static double? NullableDouble(JsonElement e, string name) =>
        double.TryParse(Str(e, name), NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : null;

    private static double Double(JsonElement e, string name) => NullableDouble(e, name) ?? 0;

    private static int Int(JsonElement e, string name, int fallback = 0) =>
        NullableDouble(e, name) is { } d ? (int)Math.Round(d) : fallback;
}
