using DriveWise.Core.Models;

namespace DriveWise.Core.FuelEconomy;

/// <summary>
/// Read-only access to the fueleconomy.gov vehicle menus and records.
/// Menu flow: year, make, model, options (options return vehicle ids).
/// </summary>
public interface IFuelEconomyClient
{
    Task<IReadOnlyList<MenuItem>> GetYearsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<MenuItem>> GetMakesAsync(int year, CancellationToken ct = default);
    Task<IReadOnlyList<MenuItem>> GetModelsAsync(int year, string make, CancellationToken ct = default);
    Task<IReadOnlyList<MenuItem>> GetOptionsAsync(int year, string make, string model, CancellationToken ct = default);
    Task<Vehicle?> GetVehicleAsync(int id, CancellationToken ct = default);
}
