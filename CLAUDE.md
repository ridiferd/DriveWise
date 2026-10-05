# DriveWise: Claude Code guide

Shared rules live in `.github/copilot-instructions.md`; read and follow them. Summary:

* Team: humans PM, Product Owner, Dev Lead. Subagents `kevin` (Analyst), `bob` (Developer), `stuart` (Tester) in `.claude/agents/`.
* Source of truth: `docs/bmad/` (brief, PRD, architecture, stories, QA gates).
* One story per change. Always run `dotnet build DriveWise.sln` and `dotnet test` before finishing.
* In this repo Claude Code is used mainly as **reviewer**: check a diff against the story's
  acceptance criteria and the architecture, and report gaps. GitHub Copilot agent mode does most implementation.

## Commands
```
dotnet build DriveWise.sln
dotnet test
dotnet run --project src/DriveWise.Web      # http://localhost:5038
```
