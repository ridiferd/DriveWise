# DW-1: Fuel economy data access with offline fallback
Status: **Done** (Sprint 1)

As a buyer, I want vehicle data from the official EPA source so that I can trust the comparison.

## Acceptance criteria
- [x] Menus for year, make, model, options are read from `/ws/rest/vehicle/menu/*`.
- [x] Vehicle records are read from `/ws/rest/vehicle/{id}` with `Accept: application/json`.
- [x] Single object menus and string numbers are parsed correctly.
- [x] When the API fails, the offline snapshot is used and the UI shows a badge.
