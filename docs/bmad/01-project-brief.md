# Project Brief: DriveWise

| Field | Value |
|---|---|
| Owner (human) | Product Owner |
| Drafted by (agent) | Kevin, Analyst |
| Status | Approved by PO |

## Problem
First-time car buyers compare vehicles using ads and dealer claims. Official EPA data exists
(fueleconomy.gov) but is spread across menus and hard to compare against a buyer's own driving.

## Target users
* **Rina, first-time buyer**: wants the cheapest car to run, drives 12,000 miles per year.
* **Arief, family buyer**: needs an SUV, cares about fuel cost and emissions equally.

## Value hypothesis
If buyers can shortlist vehicles and get a ranked, explained recommendation based on their mileage
and priorities, they will decide faster and trust the choice more than with ads alone.

## Scope (MVP, 2 sprints)
1. Build a shortlist from EPA menus (year, make, model, option).
2. Capture a driving profile (miles, ownership years, powertrain, class, weights, budget).
3. Rank and explain the shortlist; show a side-by-side comparison.

## Out of scope
Prices, financing, dealer inventory, user accounts.

## Constraints
* Data source: https://www.fueleconomy.gov/ws/rest/ (public, no API key).
* Must work offline for demos (local snapshot).
* .NET 10, Blazor Server, C#.

## Success metrics
* Time to a recommendation under 60 seconds from landing.
* 90% of test users can explain why the top vehicle won.
