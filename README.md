# DriveWise: Compare before you buy

Demo project for the ICoSEIT 2026 keynote
**"From AI-Assisted to AI-Native: Engineering Lean, Agentic Software Lifecycles with BMAD"**.

A Blazor web app that ranks a car shortlist using official EPA data from
[fueleconomy.gov web services](https://www.fueleconomy.gov/feg/ws/index.shtml).

## Team (fictional case study)
| Role | Type | BMAD role |
|---|---|---|
| PM | Human | Scrum Master, flow and priorities |
| Product Owner | Human | Accepts briefs, PRD and stories |
| Dev Lead | Human | Architect, code review, final sign-off |
| **Kevin** | Agent | Analyst: brief, PRD, stories |
| **Bob** | Agent | Developer: implements one story at a time |
| **Stuart** | Agent | Tester: xUnit tests and QA gates |

Agents are defined twice so both tools share the same team:
* GitHub Copilot: `.github/agents/*.agent.md` (custom agents) and `.github/prompts/*.prompt.md`
* Claude Code: `.claude/agents/*.md` (subagents) and `CLAUDE.md`

## Run
Requirements: .NET 10 SDK, Visual Studio 2026 or VS Code with C# Dev Kit.

```bash
dotnet build DriveWise.sln
dotnet test
dotnet run --project src/DriveWise.Web
```
Open http://localhost:5038. Demo checkpoint: http://localhost:5038/?sample=true

Data mode (`appsettings.json`, key `FuelEconomy:Mode`):
* `Resilient` (default): live API, falls back to `data/vehicles-snapshot.json`
* `Live`: API only
* `Offline`: snapshot only (safest on stage)

The included snapshot has **approximate values and synthetic ids**. Before quoting numbers, run
`tools/refresh-snapshot.ps1` (PowerShell) or `tools/refresh-snapshot.sh` (bash + jq) to pull live EPA records.

## Structure
```
docs/bmad/        brief, PRD, architecture, stories DW-1..DW-4, QA gates
docs/lean/        value stream (illustrative metrics)
src/DriveWise.Core  FuelEconomy client, parser, offline and resilient clients, recommendation engine
src/DriveWise.Web   Blazor Server UI
tests/DriveWise.Tests  xUnit tests (29 cases)
data/             offline snapshot
tools/            snapshot refresh scripts
```
