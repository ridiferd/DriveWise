using DriveWise.Core.Recommendation;
using static DriveWise.Tests.TestData;

namespace DriveWise.Tests;

public class RecommendationEngineTests
{
    private readonly RecommendationEngine _engine = new();

    [Fact]
    public void Ranks_most_efficient_cheapest_cleanest_vehicle_first()
    {
        var result = _engine.Recommend([Gas(), Hybrid(), Truck()], new DrivingProfile());

        Assert.Equal(2, result.Best!.Vehicle.Id);
        Assert.Equal(100, result.Best.Score);
        Assert.Equal(new[] { 2, 1, 4 }, result.Ranked.Select(r => r.Vehicle.Id));
    }

    [Fact]
    public void Scores_are_between_0_and_100_and_ranks_are_sequential()
    {
        var result = _engine.Recommend([Gas(), Hybrid(), Ev(), Truck()], new DrivingProfile());

        Assert.All(result.Ranked, r => Assert.InRange(r.Score, 0, 100));
        Assert.Equal(Enumerable.Range(1, 4), result.Ranked.Select(r => r.Rank));
    }

    [Fact]
    public void Rescales_EPA_fuel_cost_to_annual_miles_and_ownership_years()
    {
        var profile = new DrivingProfile { AnnualMiles = 30_000, OwnershipYears = 3 };
        var r = _engine.Recommend([Gas()], profile).Best!;

        Assert.Equal(3200, r.AnnualFuelCostUsd);    // 1600 * 30k / 15k
        Assert.Equal(9600, r.OwnershipFuelCostUsd); // 3 years
    }

    [Fact]
    public void Single_vehicle_or_full_tie_scores_100()
    {
        var result = _engine.Recommend([Gas(1), Gas(5)], new DrivingProfile());
        Assert.All(result.Ranked, r => Assert.Equal(100, r.Score));
    }

    [Theory]
    [InlineData(PowertrainPreference.ElectricOnly, new[] { 3 })]
    [InlineData(PowertrainPreference.HybridOrElectric, new[] { 3, 2 })]
    [InlineData(PowertrainPreference.GasolineOnly, new[] { 1, 4 })]
    public void Filters_by_powertrain_preference(PowertrainPreference pref, int[] expectedIds)
    {
        var result = _engine.Recommend([Gas(), Hybrid(), Ev(), Truck()], new DrivingProfile { Powertrain = pref });

        Assert.Equal(expectedIds, result.Ranked.Select(r => r.Vehicle.Id));
        Assert.Equal(4 - expectedIds.Length, result.Excluded.Count);
    }

    [Fact]
    public void Excludes_vehicles_over_fuel_budget_with_reason()
    {
        var result = _engine.Recommend([Gas(), Truck()], new DrivingProfile { MaxAnnualFuelCostUsd = 2000 });

        Assert.Single(result.Ranked);
        Assert.Contains("exceeds budget", result.Excluded[0].Reason);
    }

    [Fact]
    public void Excludes_electric_vehicles_below_minimum_range()
    {
        var result = _engine.Recommend([Ev(3, range: 200), Hybrid()], new DrivingProfile { MinElectricRangeMiles = 250 });

        Assert.Equal(2, result.Best!.Vehicle.Id);
        Assert.Contains("range", result.Excluded[0].Reason);
    }

    [Fact]
    public void Filters_by_vehicle_class_case_insensitive()
    {
        var result = _engine.Recommend([Gas(), Truck()], new DrivingProfile { VehicleClassContains = "pickup" });
        Assert.Equal(4, result.Best!.Vehicle.Id);
    }

    [Fact]
    public void Removes_duplicate_vehicles()
    {
        var result = _engine.Recommend([Gas(), Gas(), Hybrid()], new DrivingProfile());
        Assert.Equal(2, result.Ranked.Count);
    }

    [Fact]
    public void Returns_empty_result_for_empty_shortlist()
    {
        var result = _engine.Recommend([], new DrivingProfile());
        Assert.Empty(result.Ranked);
        Assert.Null(result.Best);
    }

    [Fact]
    public void Zero_weights_fall_back_to_equal_weights()
    {
        var profile = new DrivingProfile { EfficiencyWeight = 0, CostWeight = 0, EmissionsWeight = 0 };
        var result = _engine.Recommend([Gas(), Hybrid()], profile);
        Assert.Equal(2, result.Best!.Vehicle.Id);
    }

    [Fact]
    public void Cost_only_weight_prefers_lowest_running_cost()
    {
        var profile = new DrivingProfile { EfficiencyWeight = 0, CostWeight = 1, EmissionsWeight = 0 };
        var result = _engine.Recommend([Hybrid(), Ev()], profile);
        Assert.Equal(3, result.Best!.Vehicle.Id);
    }

    [Theory]
    [InlineData(0, 5, 0.4)]
    [InlineData(15000, 0, 0.4)]
    [InlineData(15000, 5, -0.1)]
    public void Invalid_profile_throws(int miles, int years, double weight)
    {
        var profile = new DrivingProfile { AnnualMiles = miles, OwnershipYears = years, EfficiencyWeight = weight };
        Assert.ThrowsAny<ArgumentException>(() => _engine.Recommend([Gas()], profile));
    }

    [Fact]
    public void Reasons_explain_the_winner()
    {
        var best = _engine.Recommend([Gas(), Ev()], new DrivingProfile()).Best!;
        Assert.Contains(best.Reasons, r => r.Contains("Zero tailpipe CO2"));
        Assert.Contains(best.Reasons, r => r.Contains("Electric range"));
    }
}
