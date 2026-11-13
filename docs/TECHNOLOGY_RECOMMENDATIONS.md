# Technology recommendations

These are proposed extensions, not installed dependencies. Benefits and suitability below are design judgements for this codebase. Official documentation was checked on 13 November 2026.

| Recommendation | Project need and expected benefit | Compatibility and implementation approach | Tradeoffs and acceptance evidence |
| --- | --- | --- | --- |
| SQLite persistence, with EF Core after a supported modern .NET migration | XML rewrites every report and lacks database transactions/constraints. Tables for requests, history and attachments would support indexed searches and atomic updates. | First isolate persistence behind an interface. Migrate a copy of XML, preserving references, histories and attachments. Microsoft documents WinForms with SQLite/EF Core. Current EF Core should not be assumed to plug directly into this .NET Framework 4.7.2 project: migrate and verify target/package support first, or select a Framework-compatible SQLite provider separately. | Adds schema migrations and deployment work. Compare counts and histories before/after migration; verify rollback after failed writes and retain a backup. SQLite remains a local store, not by itself a multi-user municipal service. |
| ASP.NET Core service plus SignalR notifications | A shared backend could let authenticated municipal staff update requests and push progress changes to residents, replacing manual local refresh. | Keep WinForms as a client and move authoritative persistence and status validation to a separate supported .NET service. Use HTTPS API calls, authenticated roles and a compatible SignalR client after target verification. Never expose unrestricted status editing in the resident UI. | Adds hosting, offline reconciliation and identity work. Demonstrate that an authorised staff update reaches two clients once; unauthorised changes fail; reconnect refreshes from the server. SignalR transports updates, while the API/database remain authoritative. |
| OpenTelemetry diagnostics | Slow index rebuilds and storage failures currently have little operational visibility. Timings and error counts would help identify when graph construction or XML persistence becomes a bottleneck. | Instrument repository save/load, snapshot construction and graph planning. OpenTelemetry publishes a .NET SDK; validate package target support against 4.7.2 before adding it. Start with a local exporter and aggregate timings. | Adds packages and telemetry overhead. Never export descriptions, attachment paths, personal locations or feedback content. Measure disabled/enabled overhead and confirm a forced save error produces a useful diagnostic without sensitive fields. |

Suggested sequence: instrument performance first, introduce transactional storage second, then develop a shared authenticated service when multi-user operation is actually required. Optimise candidate grouping and cache spanning forests if measurements identify graph work as a bottleneck.

## Official sources

- [Microsoft: Windows Forms with EF Core and SQLite](https://learn.microsoft.com/en-us/ef/core/get-started/winforms) demonstrates the desktop/database combination; it is not a promise of drop-in compatibility with this Framework target.
- [Microsoft: SQLite EF Core provider](https://learn.microsoft.com/en-us/ef/core/providers/sqlite) describes the maintained provider and links its limitations.
- [Microsoft: ASP.NET Core SignalR overview](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction) describes server-to-client real-time functionality.
- [OpenTelemetry .NET documentation](https://opentelemetry.io/docs/languages/dotnet/) describes metrics, logs, traces and supported runtime policy.

