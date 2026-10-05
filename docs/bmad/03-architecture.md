# Architecture: DriveWise

Owner: Dev Lead (acts as BMAD Architect). Reviewed by agents Bob and Stuart.

```
Browser --SignalR--> DriveWise.Web (Blazor Server)
                         |  Home.razor (shortlist, profile, results)
                         v
                   DriveWise.Core
                   +- FuelEconomy/  IFuelEconomyClient
                   |    +- FuelEconomyClient          (live REST, JSON)
                   |    +- OfflineFuelEconomyClient   (data/vehicles-snapshot.json)
                   |    +- ResilientFuelEconomyClient (live, then offline)
                   +- Recommendation/
                        +- DrivingProfile        (input, validated)
                        +- RecommendationEngine  (pure, deterministic)
```

## Key decisions (ADR summary)
1. **Blazor Server** keeps all C# on the server; no API key or CORS concerns.
2. **Tolerant parser**: the API returns numbers as strings and single objects instead of arrays.
3. **Resilient client**: live first, snapshot fallback. Mode set by `FuelEconomy:Mode`.
4. **Scoring**: min-max normalization per criterion inside the shortlist, weighted sum, 0 to 100.
   EPA `fuelCost08` assumes 15,000 miles per year and is rescaled to the buyer's mileage.
5. **Determinism**: ties broken by lower cost, then vehicle id.

## Coding standards
* C# with nullable enabled, records for models, file scoped namespaces.
* No I/O in `Recommendation/`. Public members have XML doc comments.
* Tests: xUnit, one behaviour per test, names read as sentences.
