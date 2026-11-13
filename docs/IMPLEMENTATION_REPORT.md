# Part 3 implementation report



## Scope and traceability



The final feature is `Forms/ServiceRequestStatusForm.cs`, using `Data/ServiceRequestIndex.cs` and the custom implementations in `Structures/RequestStructures.cs`. The Home menu and every feature sidebar expose Service Request Status. Report Issues and Local Events remain available. The page reads the same persisted MunicipalIssue records as Part 1: reference numbers, current status, dates and saved history are not duplicated into a separate data store.



| Requirement | Implementation and evidence |

| --- | --- |

| Well-organised submitted requests and status | Read-only grid with reference, location, category, status and date; status filter and three display orders |

| Track unique identifier and progress | Case-insensitive exact reference lookup; View progress opens the existing saved timeline and resolved-report feedback |

| Basic tree | Category/status/request hierarchy rendered in the lower tab; double-click a request leaf to track it |

| Binary tree and BST | Explicit binary nodes, ordered insertion and lookup; status buckets selected through a BST |

| AVL | Balanced reference index used by Track |

| Red-black | Balanced chronological index used by date views |

| Heap | Priority view and candidate edges for Prim |

| Graph and traversal | Undirected adjacency lists and BFS related components |

| Minimum spanning tree | Prim minimum spanning forest shown in coordination tab; handles disconnected components |

| Readme and structure explanations | README.md and this report |

| Completion, challenges and learnings | PROJECT_COMPLETION_REPORT.md |

| Technology suggestions and justification | TECHNOLOGY_RECOMMENDATIONS.md |

| Updates based on feedback | FEEDBACK_UPDATES.md; student-reported 100% for Parts 1 and 2, with no specific corrective comments provided |



## Data structures and efficiency



### Basic tree



`TreeNode<T>` stores a value and an arbitrary list of children. The root represents all requests, with category children, status grandchildren, and reference leaves. For example, a Roads/Open/MDV-1004 branch lets a resident browse a meaningful grouping without knowing a reference. Rendering visits every node in O(n + c + s) time and space for n requests, c categories and s category/status groups. Building uses linear searches across the small category/status lists; with unbounded distinct categories it can reach O(n²). This is a grouping index, not a reference search index.



### Binary trees and binary search tree



`BinaryNode<K,V>` has at most two children and forms the explicit binary representation used by all three search trees. `SearchTree<K,V>` implements ordinary BST insertion, iterative lookup and in-order traversal. The status index inserts Open, InProgress and Resolved keys and stores lists of matching requests. Selecting Open retrieves that bucket rather than scanning status strings. Search and insertion cost O(h), where h is tree height; an ordinary BST can degrade to O(n). Here there are only three status keys, so its unbalanced shape is bounded. The binary tree requirement is demonstrated by these explicit binary nodes and their traversal, rather than an unused extra tree.



### AVL tree



`AvlTree<K,V>` stores references under an ordinal, case-insensitive comparer. Heights are maintained after insertion. Single or double rotations restore a balance factor between -1 and 1. Track normalises whitespace and searches this tree; `mdv-1004` and `MDV-1004` find the same report. Lookup and insertion are O(log n) worst case, important because generated references arrive sequentially and would produce a chain in a plain BST. Duplicate keys replace the value. Repository-generated references remain unique.



### Red-black tree



`RedBlackTree<K,V>` uses left-leaning red-black insertion, colour flips and rotations. The root is black; red links lean left, no red node has a red child, and all root-to-null paths have equal black height. Keys combine fixed-width timestamp ticks with the unique reference, preserving requests reported at the same instant. In-order traversal produces oldest-first records in O(n); newest-first reverses that sequence in O(n), without sorting again. Insert and search are O(log n). With a status filter, the BST bucket builds a reference set and the chronological traversal retains only matching records in O(n + k), with O(k) extra set space. The returned rows still require O(k) space.



### Binary min-heap



`MinHeap<T>` uses an array-backed list with parent (i-1)/2 and children 2i+1, 2i+2. Push bubbles up, Pop replaces the root with the final item and sifts down. Both take O(log n); peeking is implicit in Pop and empty Pop raises a clear exception. Priority order is Open, then In Progress, then Resolved; oldest report first inside each group, then reference as a deterministic tie-break. This is a transparent display policy, not an assessment of emergency severity. Displaying all n rows by repeated Pop is O(n log n), with O(n) storage. Its benefit is an explicit, extensible priority policy and efficient next-item extraction; a complete heap-sorted display does not outperform the O(n) chronological traversal.



### Weighted graph



Each request is a vertex, including isolated reports. Two requests are connected when their category or location matches exactly, ignoring case. Undirected edges carry coordination costs: 1 for both matches, 2 for location only, 3 for category only. Shared categories can join different locations and indirect chains; these are related reports, not proven causal dependencies. Adjacency storage costs O(V + E). Current construction compares each pair, O(V²), deliberately suitable for this local coursework data set. Broad categories can create dense graphs; larger deployments should group candidates by normalised category/location and reconsider dense edge materialisation.



### Breadth-first traversal



BFS starts from the selected report, uses a queue and a visited set, and returns its connected component in O(Vc + Ec) time and O(Vc) working space. The selected report appears first. A chain MDV-A (Roads, A), MDV-B (Roads, B), MDV-C (Water, B) reaches C through B even though A and C have no direct edge. Cycles do not repeat vertices. Missing references return an empty result; an isolated request returns itself.



### Minimum spanning tree / forest



Prim starts at each unvisited component. A custom min-heap holds candidate edges; the lowest-cost edge whose destination is unvisited is selected. Each connected component receives a minimum spanning tree. Their union is a minimum spanning forest, with no artificial links between unrelated reports. An isolated report contributes zero edges. The view displays chosen references, each weight, and total weight. For the A/B/C example above, A-B costs 3 and B-C costs 2, totalling 5. A triangle with costs 5, 1, 2 selects the 1 and 2 edges. The lazy-edge implementation takes O(V + E log E) time and O(V + E) auxiliary storage, including the candidate heap. The UI recomputes the forest when selection changes, acceptable at this scale but a caching opportunity. It is an illustrative minimal coordination network, not geographic routing, shortest paths, emergency allocation or a live municipal dispatch service.



## Integration and consistency



Opening or refreshing the page takes one repository snapshot and rebuilds every index together, preventing disagreement between independently updated views. Existing XML schemas and attachments remain unchanged. Index rebuild costs include O(n log n) balanced insertions and the O(n²) pairwise graph construction. The reference lookup after rebuilding remains O(log n); rebuilding the entire page is not O(log n). There is no background service; changes made elsewhere are reflected by reopening or Refresh / clear. Status changes are not exposed as a resident permission. Seeded examples and existing saved histories demonstrate progress without pretending to connect to a municipality.



The new navigation result is propagated through the nested Report New Issue dialog. Its existing discard confirmation also applies when leaving for Service Request Status. Closing the status page uses the existing exit confirmation.



## Verification



Run the repeatable checks in README.md. `tests/Part3Checks.cs` verifies ascending, descending and random insertions; AVL heights and balance; red-black root, red links and equal black heights; duplicate replacement; sorted traversal; missing lookups; duplicate heap priorities; empty heap handling; BFS cycles and isolates; disconnected minimum forests; case-insensitive reference tracking; status filtering; priority order; indirect relationships and empty data. It also instantiates the status and main forms and renders the status page to `tests/part3-status.png`. This is a smoke check, not exhaustive UI automation. Manual acceptance steps are listed in README.md.



### Recorded results — 13 November 2026

- Release application build: passed, 0 warnings, 0 errors.
- Debug application build using `bin\VerificationDebug\`: passed, 0 warnings, 0 errors. The normal Debug output was locked by an existing running app; it was not terminated.
- Repeatable test harness: passed 6,046 assertions, including form-level tracking normalisation, missing-reference messaging and refresh, plus same-timestamp retention.
- Default-size status-page bitmap: visually inspected for layout. Full manual acceptance walkthrough remains available in README.md and is not represented as automated coverage.

