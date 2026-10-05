# Lean value stream: DriveWise (illustrative case study)

Fictional project used in the ICoSEIT 2026 keynote. Numbers are illustrative, not measured.

| Stage | Baseline (AI assisted, chat copy paste) | Lean Agentic BMAD |
|---|---|---|
| Idea to approved story | 3.0 days | 0.5 day (Kevin drafts, PO approves) |
| Story to code in review | 4.0 days | 1.0 day (Bob, Dev Lead reviews) |
| Review and test | 3.0 days | 1.0 day (Stuart tests, QA gate) |
| Rework loops per story | 2.4 | 0.8 |
| Lead time per story | 10.0 days | 2.5 days |
| Defects escaping to demo | 6 per sprint | 2 per sprint |

## Wastes observed (TIMWOODS, AI edition)
* **Waiting**: humans waiting for context from other humans; agents waiting for approvals.
* **Over processing**: regenerating code because the prompt lacked the story context.
* **Defects**: hallucinated API fields (fixed by a tolerant parser and a snapshot).
* **Unused talent**: senior engineers typing boilerplate instead of reviewing design.
