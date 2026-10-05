# DW-4: EV minimum range filter
Status: **Draft** (created live by Kevin during the keynote) | Points: 2

As an EV shopper, I want to set a minimum electric range so that I only see EVs that fit my commute.

## Acceptance criteria
1. Profile form has an optional "Minimum EV range (mi)" input (0 to 600).
2. EVs with `range` below the minimum are excluded with reason "Electric range X mi is below Y mi".
3. Non electric vehicles are not affected by this filter.
4. Winner reasons mention the electric range for EVs.

## Open questions for PO
* Should plug-in hybrids use `rangeA` (electric range on gasoline cars)?

## Traceability
PRD FR3, FR6. Engine already supports `MinElectricRangeMiles`; UI work only.
