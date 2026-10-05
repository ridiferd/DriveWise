# PRD: DriveWise MVP

Owner: Product Owner. Facilitated by: PM. Drafted by: Kevin (Analyst agent).

## Epics and stories
| ID | Story | Owner agent | Status |
|---|---|---|---|
| DW-1 | Fuel economy data access with offline fallback | Bob | Done (Sprint 1) |
| DW-2 | Recommendation engine | Bob, tested by Stuart | Ready |
| DW-3 | Recommendation and comparison view | Bob, tested by Stuart | Ready |
| DW-4 | EV minimum range filter | Kevin drafts live | Idea |

## Functional requirements
* **FR1** User selects year, make, model and option from EPA menus.
* **FR2** User keeps a shortlist of at most 6 vehicles, no duplicates.
* **FR3** User sets annual miles (1 to 100,000), ownership years (1 to 20), powertrain
  (Any, GasolineOnly, HybridOrElectric, ElectricOnly), optional class filter and fuel budget.
* **FR4** User sets relative weights for efficiency, cost and emissions (0 to 1).
* **FR5** System ranks the shortlist with a 0 to 100 score and at least one reason per vehicle.
* **FR6** System lists excluded vehicles with the reason for exclusion.
* **FR7** System shows annual and ownership fuel cost scaled to the user's mileage.

## Non functional requirements
* **NFR1** Falls back to the offline snapshot within 8 seconds if the API fails.
* **NFR2** Engine is pure C# with no I/O, fully unit tested (line coverage 90% or more).
* **NFR3** No secrets, no personal data stored.
