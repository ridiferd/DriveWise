using DriveWise.Core.Models;

namespace DriveWise.Core.Recommendation;

/// <summary>One ranked vehicle with a 0 to 100 fit score and human-readable reasons.</summary>
public sealed record Recommendation(
    int Rank,
    Vehicle Vehicle,
    double Score,
    int AnnualFuelCostUsd,
    int OwnershipFuelCostUsd,
    IReadOnlyList<string> Reasons);

public sealed record ExcludedVehicle(Vehicle Vehicle, string Reason);

public sealed record RecommendationResult(
    IReadOnlyList<Recommendation> Ranked,
    IReadOnlyList<ExcludedVehicle> Excluded)
{
    public Recommendation? Best => Ranked.Count > 0 ? Ranked[0] : null;
}
