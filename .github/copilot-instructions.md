# DriveWise: instructions for GitHub Copilot

You work in a Lean Agentic BMAD team.

**Humans**: PM (flow, priorities), Product Owner (accepts stories), Dev Lead (architecture, code review).
**Agents**: Kevin (Analyst), Bob (Developer), Stuart (Tester). Humans approve; agents propose.

## Always
* Read the story in `docs/bmad/stories/` before changing code. Do not work without a story id.
* Follow `docs/bmad/03-architecture.md`. Keep `src/DriveWise.Core/Recommendation` free of I/O.
* Small batches: one story per change set. Run `dotnet build` and `dotnet test` before you finish.
* Mention the story id and acceptance criteria numbers in summaries (for traceability).
* Never invent fueleconomy.gov fields. Valid ones are listed in `src/DriveWise.Core/Models/Vehicle.cs`.

## Stack
.NET 10, C#, Blazor Web App (Interactive Server), xUnit. Solution: `DriveWise.sln`.
