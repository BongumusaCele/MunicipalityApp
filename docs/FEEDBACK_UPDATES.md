# Updates and lecturer-feedback record

## Evidence status

The student confirmed on 13 November 2026 that Parts 1 and 2 each received 100%. No specific corrective lecturer comments have been provided. The earlier functionality has therefore been retained, while Part 3 extends the application to meet the final-task requirements. The changes below document those additions and proactive integration improvements.

The marks are recorded from the student's confirmation; a marked rubric has not been independently reviewed. A full mark does not establish that no written comments existed, so this record makes no such claim.

## Implemented updates

| Requirement or observed gap | Change | Files | Verification |
| --- | --- | --- | --- |
| Status feature marked Coming Soon | Enable menu and sidebar; route between all three features | MainMenuForm.cs, MainMenuForm.Designer.cs, Forms/AppSidebar.cs, Forms/CitizenServicesFormBase.cs | Build and form-construction smoke checks |
| Need efficient unique-reference tracking | AVL lookup using case-insensitive full references | Data/ServiceRequestIndex.cs, Structures/RequestStructures.cs | Reference and invariant assertions |
| Need organised status and chronological views | Basic grouping tree, status BST, chronological red-black index | Same index/structure files and Forms/ServiceRequestStatusForm.cs | Sorting, replacement, filtering and invariant assertions |
| Need priority, relationships and minimum spanning trees | Custom min-heap, adjacency graph, BFS, Prim forest; policy and score meanings visible | Same files | Heap, BFS cycle/isolate and known-weight disconnected forest checks |
| New destination must work from nested report dialog | Propagate navigation result; preserve discard confirmation | Forms/ReportedIssuesForm.cs, Forms/ReportNewIssueForm.cs | Compile; manual acceptance steps in README |
| Submission needs detailed guidance and reports | Extend README and add implementation, completion, technology and feedback documents | README.md, docs/*.md | Requirements traceability table in implementation report |

## Previous assessment outcome

| Source | Outcome | Response in Part 3 |
| --- | --- | --- |
| Student confirmation, 13 November 2026 | Part 1: 100%; Part 2: 100% | Retain reporting and events features; integrate the final status feature and document validation |
| Specific corrective lecturer comments | None provided | No corrective action is attributed to an unseen comment; the implemented updates above are driven by Part 3 requirements and integration review |

The project overview and learning reflections should be personalised by the student to match their own experience. If written lecturer comments accompany the full marks, they can be appended with the corresponding response.
