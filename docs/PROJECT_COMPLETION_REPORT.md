# Project completion report

## Overview

The application now provides all three menu features for the municipal services coursework: report issues, browse local events and announcements, and track service requests. It remains a Windows Forms application targeting .NET Framework 4.7.2, with no third-party runtime packages. This report describes the delivered code, not a claim that a real municipality has deployed or approved it.

### Part 1: reporting and tracking foundations

The existing reporting workflow collects a location, category and description, validates entries, accepts an optional supported attachment, and returns a unique MDV reference. The repository saves XML locally and copies attachments into its managed directory. The reported-issues page searches and filters reports; details show saved progress and allow one-time feedback for resolved reports. The Home page summarises totals. These existing facilities were retained and reused for Part 3.

The code addresses common reporting challenges through field validation, attachment size/type checks and reference generation from stored reports. This assessment is based on the existing implementation; the student reports 100% for both earlier tasks, but no specific lecturer comments or development diary were supplied, so it does not assert who encountered particular historical problems.

### Part 2: events and announcements

The existing events page supports text search, category/date filtering, details and recommendations. Its date index, dictionaries, sets and bounded search-history queue organise events and derive recommendations from recent interests. The challenge is combining several filters with understandable suggestions; the implementation keeps the event repository separate from the form and explains recommendation reasons. This feature remains available through the same navigation shell.

### Part 3: service request status

The new page reads existing reports and displays references, locations, categories, current statuses and reported dates. Residents can track a complete reference, filter by status, change display order, browse grouped requests, and open saved progress. Custom tree, heap and graph structures actively drive those operations. Related components and a minimum spanning forest expose relationships and an illustrative coordination plan. Implementation details and complexity tradeoffs are in IMPLEMENTATION_REPORT.md.

## Challenges and solutions in this implementation

1. **Meaningful use of multiple trees.** Merely declaring structures would not improve the application. Each now has a distinct role: basic hierarchy for browsing, BST for status buckets, AVL for exact references and red-black for chronological ordering. Binary nodes and in-order traversal are explicit. This also exposes where a simpler structure might suffice outside the coursework.
2. **Sequential identifiers skew an ordinary BST.** AVL rotations protect reference lookup; adversarial ascending/descending test sequences check balance. The ordinary BST holds only the bounded status domain.
3. **Timestamp collisions can hide records.** Date keys append the unique reference to fixed-width ticks. Equal dates therefore retain separate requests and a deterministic order.
4. **Graph links need a defensible meaning.** Matching category/location defines a reproducible relationship. Weights describe coordination similarity, and the UI labels their limits. No fabricated distances or live dispatch claims are used.
5. **Disconnected graphs and cycles.** BFS uses a visited set. Prim restarts from each unvisited component, giving a forest and retaining independent requests rather than inventing edges.
6. **Navigation through nested forms.** The Report New Issue dialog returns a destination to its parent; the parent now propagates the status and events destinations. Discard confirmation protects entered report data when choosing the new page.
7. **Consistent indexes after changes.** A single snapshot rebuilds all indexes on entry/refresh. This trades rebuild cost for clear consistency and avoids partial index updates.
8. **Verification on the existing environment.** Release compilation succeeded. A separate test executable exercises algorithm invariants and form construction using an isolated data directory. A pre-existing running Debug executable prevented overwriting its output; a separate Debug output directory permits verification without terminating that session.

## Key learnings

- Data structure choice should follow an actual operation. Balanced trees suit ordered keys; a small bounded BST can be sufficient for status groups; a heap supports an explicit next-item policy.
- Invariants explain correctness better than example outputs alone. AVL balance, red-black black-height and heap ordering can be checked across adversarial inputs, exposing errors that a four-row demonstration would miss.
- Complexity must include setup and display. O(log n) reference lookup does not make a page with O(n²) graph construction scalable. Reversing an ordered traversal avoids an unnecessary sort.
- BFS connectivity, shortest paths and minimum spanning trees answer different questions. A spanning forest reduces total chosen edge weight but does not give travel routes or prove service dependencies.
- Keeping persistence, indexing and presentation separate lets existing report history serve new views while protecting the saved format. Snapshot rebuilding is a simple consistency strategy for a small desktop application.
- Clear UI language is part of correctness: the priority policy and coordination costs must be explained so residents do not mistake them for official urgency decisions.
- Integration includes return navigation and unsaved-data handling, not just a working new form.

These are technical reflections supported by the implementation. The student reports 100% for Parts 1 and 2; this outcome is recorded in FEEDBACK_UPDATES.md. The student should personalise the learning reflections and add any accompanying written comments if available.

## Validation and limits

See README.md for commands and manual acceptance steps. The repeatable suite checks structure invariants, query behaviour, graph algorithms and form construction. A rendered default-size status form was inspected for layout. This does not establish exhaustive interactive regression coverage or municipal production readiness.

The app stores local XML and sample events; it has no authenticated staff workflow, live municipal backend, multi-user transaction control or emergency escalation integration. Status histories originate in local saved data; residents can view but cannot authorise official status changes. Exact location matching misses spelling variants, and broad categories may produce dense graphs. Recommendations for improving these limitations are documented separately. No marks are guaranteed; the rubric has inconsistent subtotal labels, so every visible criterion is mapped explicitly rather than relying on those totals.

