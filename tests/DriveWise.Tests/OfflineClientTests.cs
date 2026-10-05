using DriveWise.Core.FuelEconomy;
using static DriveWise.Tests.TestData;

namespace DriveWise.Tests;

public class OfflineClientTests
{
    private readonly OfflineFuelEconomyClient _client = new([Gas(1), Hybrid(2), Ev(3)]);

    [Fact]
    public async Task Menus_follow_year_make_model_options_flow()
    {
        var years = await _client.GetYearsAsync();
        var makes = await _client.GetMakesAsync(2025);
        var models = await _client.GetModelsAsync(2025, "Make2");
        var options = await _client.GetOptionsAsync(2025, "Make2", "Model2");

        Assert.Equal("2025", Assert.Single(years).Value);
        Assert.Equal(3, makes.Count);
        Assert.Equal("Model2", Assert.Single(models).Value);
        Assert.Equal("2", Assert.Single(options).Value);
    }

    [Fact]
    public async Task Unknown_vehicle_returns_null()
    {
        Assert.Null(await _client.GetVehicleAsync(999));
    }

    [Fact]
    public void Snapshot_file_loads()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "data", "vehicles-snapshot.json");
        var client = OfflineFuelEconomyClient.FromFile(path);
        Assert.True(client.All.Count >= 10);
    }
}
