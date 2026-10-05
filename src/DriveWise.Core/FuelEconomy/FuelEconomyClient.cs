using DriveWise.Core.Models;

namespace DriveWise.Core.FuelEconomy;

/// <summary>
/// Typed HttpClient for https://www.fueleconomy.gov/ws/rest/ (no API key required).
/// Register with: services.AddHttpClient&lt;IFuelEconomyClient, FuelEconomyClient&gt;().
/// </summary>
public sealed class FuelEconomyClient(HttpClient http) : IFuelEconomyClient
{
    public const string BaseAddress = "https://www.fueleconomy.gov/ws/rest/";

    public async Task<IReadOnlyList<MenuItem>> GetYearsAsync(CancellationToken ct = default) =>
        FuelEconomyParser.ParseMenu(await GetJsonAsync("vehicle/menu/year", ct));

    public async Task<IReadOnlyList<MenuItem>> GetMakesAsync(int year, CancellationToken ct = default) =>
        FuelEconomyParser.ParseMenu(await GetJsonAsync($"vehicle/menu/make?year={year}", ct));

    public async Task<IReadOnlyList<MenuItem>> GetModelsAsync(int year, string make, CancellationToken ct = default) =>
        FuelEconomyParser.ParseMenu(await GetJsonAsync(
            $"vehicle/menu/model?year={year}&make={Uri.EscapeDataString(make)}", ct));

    public async Task<IReadOnlyList<MenuItem>> GetOptionsAsync(int year, string make, string model, CancellationToken ct = default) =>
        FuelEconomyParser.ParseMenu(await GetJsonAsync(
            $"vehicle/menu/options?year={year}&make={Uri.EscapeDataString(make)}&model={Uri.EscapeDataString(model)}", ct));

    public async Task<Vehicle?> GetVehicleAsync(int id, CancellationToken ct = default) =>
        FuelEconomyParser.ParseVehicle(await GetJsonAsync($"vehicle/{id}", ct));

    private async Task<string> GetJsonAsync(string relativeUrl, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
        request.Headers.Accept.ParseAdd("application/json");
        using var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct);
    }
}
