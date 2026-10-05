# DW-3: Recommendation and comparison view
Status: **Ready** | Developer: Bob | Tester: Stuart | Points: 3

As a buyer, I want to see the winner and a side-by-side table so that I can explain my choice.

## Acceptance criteria
1. A **Recommend** button is enabled only when the shortlist is not empty.
2. A winner card shows the best vehicle, its score out of 100 and its reasons.
3. A table shows rank, vehicle, class, combined MPG or MPGe, L/100 km (n/a for EVs),
   fuel per year, fuel over ownership years, CO2 g/mi and a score bar.
4. Excluded vehicles appear in a collapsible list with reasons.
5. Validation errors from the engine are shown in the page, not as an exception.
6. `/?sample=true` preloads the sample shortlist and shows the recommendation (demo checkpoint).
