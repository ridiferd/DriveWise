using DriveWise.Core.Models;

namespace DriveWise.Tests;

internal static class TestData
{
    public static Vehicle Car(int id, int mpg, int cost, double co2,
        string atv = "", string fuel = "Regular", string vclass = "Midsize Cars", int range = 0) => new()
    {
        Id = id, Year = 2025, Make = "Make" + id, Model = "Model" + id,
        CombinedMpg = mpg, CityMpg = mpg, HighwayMpg = mpg,
        AnnualFuelCostUsd = cost, Co2GramsPerMile = co2,
        AlternativeFuelType = atv, FuelType = fuel, VehicleClass = vclass, ElectricRangeMiles = range
    };

    public static Vehicle Gas(int id = 1) => Car(id, 30, 1600, 290);
    public static Vehicle Hybrid(int id = 2) => Car(id, 52, 950, 170, atv: "Hybrid");
    public static Vehicle Ev(int id = 3, int range = 300) => Car(id, 120, 650, 0, atv: "EV", fuel: "Electricity", range: range);
    public static Vehicle Truck(int id = 4) => Car(id, 22, 2250, 400, vclass: "Standard Pickup Trucks 2WD");
}
