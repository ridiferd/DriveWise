# QA Gate: DW-2 Recommendation engine
Reviewer: Stuart (Tester agent). Human sign-off: Dev Lead.

| AC | Test(s) | Result |
|---|---|---|
| 1 | Ranks_most_efficient_cheapest_cleanest_vehicle_first | Pass |
| 2 | Scores_are_between_0_and_100_and_ranks_are_sequential | Pass |
| 3 | Zero_weights_fall_back_to_equal_weights, Cost_only_weight_prefers_lowest_running_cost | Pass |
| 4 | Single_vehicle_or_full_tie_scores_100 | Pass |
| 5 | Rescales_EPA_fuel_cost_to_annual_miles_and_ownership_years | Pass |
| 6 | Filters_by_powertrain_preference (3 cases), Excludes_vehicles_over_fuel_budget_with_reason, Excludes_electric_vehicles_below_minimum_range, Filters_by_vehicle_class_case_insensitive | Pass |
| 7 | Removes_duplicate_vehicles, Returns_empty_result_for_empty_shortlist | Pass |
| 8 | Invalid_profile_throws (3 cases) | Pass |
| 9 | Reasons_explain_the_winner | Pass |
| 10 | Covered only indirectly | Concern: add an explicit tie break test |

**Gate decision: PASS with 1 concern** (AC10 needs an explicit test). Risk: low.
