namespace DriveWise.Core.Models;

/// <summary>
/// A vehicle record mapped from the fueleconomy.gov web service
/// (https://www.fueleconomy.gov/feg/ws/index.shtml).
/// Fuel economy values are EPA MPG (or MPGe for electric vehicles).
/// </summary>
public sealed record Vehicle
{
    public required int Id { get; init; }
    public required int Year { get; init; }
    public required string Make { get; init; }
    public required string Model { get; init; }
    public string VehicleClass { get; init; } = "";   // VClass
    public string Drive { get; init; } = "";          // drive
    public string Transmission { get; init; } = "";   // trany
    public string FuelType { get; init; } = "";       // fuelType
    public string AlternativeFuelType { get; init; } = ""; // atvType (EV, Hybrid, Plug-in Hybrid, ...)
    public double? Cylinders { get; init; }           // cylinders
    public double? DisplacementLiters { get; init; }  // displ
    public int CityMpg { get; init; }                 // city08
    public int HighwayMpg { get; init; }              // highway08
    public int CombinedMpg { get; init; }             // comb08
    public int AnnualFuelCostUsd { get; init; }       // fuelCost08 (EPA basis: 15,000 mi, 55% city)
    public double Co2GramsPerMile { get; init; }      // co2TailpipeGpm
    public int FuelEconomyScore { get; init; }        // feScore (1 to 10, -1 if n/a)
    public int GreenhouseGasScore { get; init; }      // ghgScore (1 to 10, -1 if n/a)
    public int FiveYearSaveSpendUsd { get; init; }    // youSaveSpend (vs. average new vehicle)
    public int ElectricRangeMiles { get; init; }      // range (EVs only)

    public bool IsElectric =>
        AlternativeFuelType.Equals("EV", StringComparison.OrdinalIgnoreCase) ||
        FuelType.Equals("Electricity", StringComparison.OrdinalIgnoreCase);

    public bool IsHybrid =>
        AlternativeFuelType.Contains("Hybrid", StringComparison.OrdinalIgnoreCase);

    /// <summary>Combined consumption in L/100 km (for audiences outside the US).</summary>
    public double LitersPer100Km => CombinedMpg > 0 ? Math.Round(235.215 / CombinedMpg, 1) : 0;

    public string DisplayName => $"{Year} {Make} {Model}";
}
