using DriveWise.Core.Models;

namespace DriveWise.Core.Recommendation;

/// <summary>
/// Ranks a shortlist of vehicles against a driving profile (Story DW-2).
/// Score = weighted sum of min-max normalized efficiency, cost and emissions, scaled to 0..100.
/// </summary>
public sealed class RecommendationEngine
{
    public RecommendationResult Recommend(IEnumerable<Vehicle> shortlist, DrivingProfile profile)
    {
        ArgumentNullException.ThrowIfNull(shortlist);
        ArgumentNullException.ThrowIfNull(profile);
        profile.Validate();

        var candidates = new List<Vehicle>();
        var excluded = new List<ExcludedVehicle>();

        foreach (var v in shortlist.DistinctBy(v => v.Id))
        {
            var reason = ExclusionReason(v, profile);
            if (reason is null) candidates.Add(v); else excluded.Add(new ExcludedVehicle(v, reason));
        }

        if (candidates.Count == 0) return new RecommendationResult([], excluded);

        var (we, wc, wg) = NormalizeWeights(profile);
        var costs = candidates.ToDictionary(v => v.Id, v => AnnualCost(v, profile));

        var mpgRange = Range(candidates.Select(v => (double)v.CombinedMpg));
        var costRange = Range(costs.Values.Select(c => (double)c));
        var co2Range = Range(candidates.Select(v => v.Co2GramsPerMile));

        var scored = candidates.Select(v =>
        {
            var efficiency = Normalize(v.CombinedMpg, mpgRange, higherIsBetter: true);
            var cost = Normalize(costs[v.Id], costRange, higherIsBetter: false);
            var emissions = Normalize(v.Co2GramsPerMile, co2Range, higherIsBetter: false);
            var score = Math.Round(100 * (we * efficiency + wc * cost + wg * emissions), 1);
            return (Vehicle: v, Score: score, Cost: costs[v.Id]);
        })
        .OrderByDescending(x => x.Score)
        .ThenBy(x => x.Cost)
        .ThenBy(x => x.Vehicle.Id)
        .ToList();

        var bestMpg = candidates.Max(v => v.CombinedMpg);
        var lowestCost = costs.Values.Min();
        var lowestCo2 = candidates.Min(v => v.Co2GramsPerMile);

        var ranked = scored.Select((x, i) => new Recommendation(
            Rank: i + 1,
            Vehicle: x.Vehicle,
            Score: x.Score,
            AnnualFuelCostUsd: x.Cost,
            OwnershipFuelCostUsd: x.Cost * profile.OwnershipYears,
            Reasons: Reasons(x.Vehicle, x.Cost, bestMpg, lowestCost, lowestCo2, profile)))
            .ToList();

        return new RecommendationResult(ranked, excluded);
    }

    /// <summary>EPA fuelCost08 is based on 15,000 miles; rescale to the buyer's mileage.</summary>
    public static int AnnualCost(Vehicle v, DrivingProfile p) =>
        (int)Math.Round(v.AnnualFuelCostUsd * (double)p.AnnualMiles / DrivingProfile.EpaBaselineMiles);

    private static string? ExclusionReason(Vehicle v, DrivingProfile p)
    {
        if (v.CombinedMpg <= 0) return "No EPA fuel economy data.";

        var powertrainOk = p.Powertrain switch
        {
            PowertrainPreference.GasolineOnly => !v.IsElectric && !v.IsHybrid,
            PowertrainPreference.HybridOrElectric => v.IsElectric || v.IsHybrid,
            PowertrainPreference.ElectricOnly => v.IsElectric,
            _ => true
        };
        if (!powertrainOk) return $"Powertrain does not match preference ({p.Powertrain}).";

        if (!string.IsNullOrWhiteSpace(p.VehicleClassContains) &&
            !v.VehicleClass.Contains(p.VehicleClassContains, StringComparison.OrdinalIgnoreCase))
            return $"Vehicle class '{v.VehicleClass}' does not match '{p.VehicleClassContains}'.";

        if (p.MaxAnnualFuelCostUsd is { } max && AnnualCost(v, p) > max)
            return $"Annual fuel cost ${AnnualCost(v, p):N0} exceeds budget ${max:N0}.";

        if (p.MinElectricRangeMiles is { } minRange && v.IsElectric && v.ElectricRangeMiles < minRange)
            return $"Electric range {v.ElectricRangeMiles} mi is below {minRange} mi.";

        return null;
    }

    private static (double e, double c, double g) NormalizeWeights(DrivingProfile p)
    {
        var sum = p.EfficiencyWeight + p.CostWeight + p.EmissionsWeight;
        return sum <= 0
            ? (1.0 / 3, 1.0 / 3, 1.0 / 3)
            : (p.EfficiencyWeight / sum, p.CostWeight / sum, p.EmissionsWeight / sum);
    }

    private static (double Min, double Max) Range(IEnumerable<double> values)
    {
        var list = values.ToList();
        return (list.Min(), list.Max());
    }

    private static double Normalize(double value, (double Min, double Max) r, bool higherIsBetter)
    {
        if (r.Max - r.Min < 1e-9) return 1.0; // everyone ties, nobody is penalized
        var t = (value - r.Min) / (r.Max - r.Min);
        return higherIsBetter ? t : 1 - t;
    }

    private static IReadOnlyList<string> Reasons(
        Vehicle v, int cost, int bestMpg, int lowestCost, double lowestCo2, DrivingProfile p)
    {
        var reasons = new List<string>();
        var unit = v.IsElectric ? "MPGe" : "MPG";
        if (v.CombinedMpg == bestMpg) reasons.Add($"Best combined efficiency in shortlist ({v.CombinedMpg} {unit}).");
        if (cost == lowestCost) reasons.Add($"Lowest fuel cost: ${cost:N0}/yr at {p.AnnualMiles:N0} mi.");
        if (v.Co2GramsPerMile == 0) reasons.Add("Zero tailpipe CO2.");
        else if (Math.Abs(v.Co2GramsPerMile - lowestCo2) < 1e-9) reasons.Add($"Lowest tailpipe CO2 ({v.Co2GramsPerMile:N0} g/mi).");
        if (v.FiveYearSaveSpendUsd > 0) reasons.Add($"EPA: saves ${v.FiveYearSaveSpendUsd:N0} over 5 years vs. average new vehicle.");
        if (v.IsElectric && v.ElectricRangeMiles > 0) reasons.Add($"Electric range {v.ElectricRangeMiles} mi.");
        if (reasons.Count == 0) reasons.Add($"{v.CombinedMpg} {unit} combined, ${cost:N0}/yr fuel.");
        return reasons;
    }
}
