using System.Text.Json;
using DriveWise.Core.Models;

namespace DriveWise.Core.FuelEconomy;

/// <summary>
/// Serves menus and vehicles from a local JSON snapshot so the demo works without internet.
/// The snapshot uses the same field names as the live API.
/// </summary>
public sealed class OfflineFuelEconomyClient : IFuelEconomyClient
{
    private readonly IReadOnlyList<Vehicle> _vehicles;

    public OfflineFuelEconomyClient(IEnumerable<Vehicle> vehicles) => _vehicles = vehicles.ToList();

    public static OfflineFuelEconomyClient FromFile(string path)
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(path));
        var vehicles = doc.RootElement.GetProperty("vehicle").EnumerateArray()
            .Select(FuelEconomyParser.ParseVehicle)
            .OfType<Vehicle>();
        return new OfflineFuelEconomyClient(vehicles);
    }

    public IReadOnlyList<Vehicle> All => _vehicles;

    public Task<IReadOnlyList<MenuItem>> GetYearsAsync(CancellationToken ct = default) =>
        Menu(_vehicles.Select(v => v.Year.ToString()).Distinct().OrderDescending());

    public Task<IReadOnlyList<MenuItem>> GetMakesAsync(int year, CancellationToken ct = default) =>
        Menu(_vehicles.Where(v => v.Year == year).Select(v => v.Make).Distinct().Order());

    public Task<IReadOnlyList<MenuItem>> GetModelsAsync(int year, string make, CancellationToken ct = default) =>
        Menu(_vehicles.Where(v => v.Year == year && v.Make == make).Select(v => v.Model).Distinct().Order());

    public Task<IReadOnlyList<MenuItem>> GetOptionsAsync(int year, string make, string model, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<MenuItem>>(_vehicles
            .Where(v => v.Year == year && v.Make == make && v.Model == model)
            .Select(v => new MenuItem($"{v.Transmission}, {v.Drive}, {v.FuelType}", v.Id.ToString()))
            .ToList());

    public Task<Vehicle?> GetVehicleAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_vehicles.FirstOrDefault(v => v.Id == id));

    private static Task<IReadOnlyList<MenuItem>> Menu(IEnumerable<string> values) =>
        Task.FromResult<IReadOnlyList<MenuItem>>(values.Select(v => new MenuItem(v, v)).ToList());
}
