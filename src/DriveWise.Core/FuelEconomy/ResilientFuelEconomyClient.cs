using DriveWise.Core.Models;
using Microsoft.Extensions.Logging;

namespace DriveWise.Core.FuelEconomy;

/// <summary>
/// Tries the live API first and falls back to the offline snapshot on any failure.
/// Keeps a live demo alive when conference Wi-Fi is not.
/// </summary>
public sealed class ResilientFuelEconomyClient(
    FuelEconomyClient live,
    OfflineFuelEconomyClient offline,
    ILogger<ResilientFuelEconomyClient> logger) : IFuelEconomyClient
{
    public bool LastCallUsedFallback { get; private set; }

    public Task<IReadOnlyList<MenuItem>> GetYearsAsync(CancellationToken ct = default) =>
        Try(c => c.GetYearsAsync(ct), "years");

    public Task<IReadOnlyList<MenuItem>> GetMakesAsync(int year, CancellationToken ct = default) =>
        Try(c => c.GetMakesAsync(year, ct), "makes");

    public Task<IReadOnlyList<MenuItem>> GetModelsAsync(int year, string make, CancellationToken ct = default) =>
        Try(c => c.GetModelsAsync(year, make, ct), "models");

    public Task<IReadOnlyList<MenuItem>> GetOptionsAsync(int year, string make, string model, CancellationToken ct = default) =>
        Try(c => c.GetOptionsAsync(year, make, model, ct), "options");

    public Task<Vehicle?> GetVehicleAsync(int id, CancellationToken ct = default) =>
        Try(c => c.GetVehicleAsync(id, ct), $"vehicle {id}");

    private async Task<T> Try<T>(Func<IFuelEconomyClient, Task<T>> call, string what)
    {
        try
        {
            var result = await call(live);
            LastCallUsedFallback = false;
            return result;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            logger.LogWarning(ex, "fueleconomy.gov unavailable for {What}; using offline snapshot", what);
            LastCallUsedFallback = true;
            return await call(offline);
        }
    }
}
