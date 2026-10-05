namespace DriveWise.Core.Recommendation;

public enum PowertrainPreference { Any, GasolineOnly, HybridOrElectric, ElectricOnly }

/// <summary>
/// What the buyer cares about. Weights are relative and are normalized by the engine.
/// EPA annual fuel cost assumes 15,000 miles per year; the engine rescales to AnnualMiles.
/// </summary>
public sealed record DrivingProfile
{
    public const int EpaBaselineMiles = 15_000;

    public int AnnualMiles { get; init; } = EpaBaselineMiles;
    public int OwnershipYears { get; init; } = 5;
    public double EfficiencyWeight { get; init; } = 0.4;
    public double CostWeight { get; init; } = 0.4;
    public double EmissionsWeight { get; init; } = 0.2;
    public PowertrainPreference Powertrain { get; init; } = PowertrainPreference.Any;
    public string? VehicleClassContains { get; init; }
    public int? MaxAnnualFuelCostUsd { get; init; }
    public int? MinElectricRangeMiles { get; init; }

    public void Validate()
    {
        if (AnnualMiles is < 1 or > 100_000)
            throw new ArgumentOutOfRangeException(nameof(AnnualMiles), "Annual miles must be between 1 and 100,000.");
        if (OwnershipYears is < 1 or > 20)
            throw new ArgumentOutOfRangeException(nameof(OwnershipYears), "Ownership years must be between 1 and 20.");
        if (EfficiencyWeight < 0 || CostWeight < 0 || EmissionsWeight < 0)
            throw new ArgumentOutOfRangeException(nameof(EfficiencyWeight), "Weights cannot be negative.");
    }
}
