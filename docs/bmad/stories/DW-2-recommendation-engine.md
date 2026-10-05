# DW-2: Recommendation engine
Status: **Ready** | Developer: Bob | Tester: Stuart | Points: 5

As a buyer, I want my shortlist ranked against how I drive so that I can see the best fit.

## Acceptance criteria
1. Given a shortlist and a `DrivingProfile`, `RecommendationEngine.Recommend` returns a
   `RecommendationResult` with `Ranked` (rank 1 first) and `Excluded`.
2. Score = 100 x (we x efficiency + wc x cost + wg x emissions), each criterion min-max
   normalized within the candidates; higher MPG is better, lower cost and CO2 are better.
3. Weights are normalized to sum 1; if all weights are 0, use equal weights.
4. If all candidates tie on a criterion, that criterion scores 1.0 for everyone.
5. Annual cost = `fuelCost08 x AnnualMiles / 15000`, rounded; ownership cost = annual x years.
6. Exclude with a reason: no MPG data, powertrain mismatch, class mismatch, over budget,
   EV range below `MinElectricRangeMiles`.
7. Duplicates (same id) are counted once. Empty shortlist returns empty result.
8. Invalid profile (see `DrivingProfile.Validate`) throws `ArgumentOutOfRangeException`.
9. Each ranked vehicle has at least one human-readable reason.
10. Ties are broken by lower annual cost, then lower id.

## Dev notes
* Files: `src/DriveWise.Core/Recommendation/Recommendation.cs`, `RecommendationEngine.cs`.
* Pure C#; no I/O; register as singleton in `Program.cs`.

## Test notes (Stuart)
* One test per acceptance criterion minimum; use `TestData` builders.
