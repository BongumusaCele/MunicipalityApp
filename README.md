# Midvaal Citizen Services

Midvaal Citizen Services is a C# Windows Forms application for reporting and tracking municipal issues.

## Run with Visual Studio

### Requirements

- Windows 10 or later
- Visual Studio with the **.NET desktop development** workload
- .NET Framework 4.7.2 Developer Pack

### Steps

1. Open `MunicipalityApp.slnx` in Visual Studio.
2. Wait for Visual Studio to load the project.
3. Select **Debug** and **Any CPU** from the toolbar.
4. Select **Build > Build Solution**, or press `Ctrl+Shift+B`.
5. Press `F5` to run with debugging, or `Ctrl+F5` to run without debugging.

## Alternative: Run from PowerShell

Open PowerShell in the folder containing `MunicipalityApp.slnx`, then run:

```powershell
dotnet build MunicipalityApp.slnx --configuration Debug
.\MunicipalityApp\bin\Debug\MunicipalityApp.exe
```

To create and run a Release build instead:

```powershell
dotnet build MunicipalityApp.slnx --configuration Release
.\MunicipalityApp\bin\Release\MunicipalityApp.exe
```

No third-party libraries or NuGet packages are required.

## How to Use the Application

1. Start the application. The Home page displays the available municipal services and issue totals.
2. Select **Report Issues** from the sidebar, or select **View Reported Issues**, to open the reported issues list.
3. Use the search box to find issues by reference, location, category, description or status.
4. Use the category dropdown to filter the displayed issues.
5. Double-click an issue in the table to view its full details, attachment and progress updates.
6. Select **Report New Issue** to create a report.
7. Enter the issue location, select a category and provide a description.
8. Optionally select **Choose File** to attach a JPG, JPEG, PNG, PDF or DOCX file no larger than 10 MB.
9. Select **Submit Report**. The application displays a unique reference number and adds the report to the issues list.
10. Open a resolved issue by double-clicking it to submit a one-time rating and optional feedback comment.
11. Use **Home** to return to the main page, **Back to Reported Issues** to return to the list and **Exit** to close the application.

## Local Events and Announcements

Select **Local Events** from the sidebar to browse upcoming events and municipal announcements. Search by title, venue or keyword, filter by category, or enable the date filter to find events on a specific date. Double-click a result to view its full details.

Recommendations adapt to the categories and keywords used in recent searches. Each suggestion explains why it was selected. The feature uses a `SortedDictionary` to organise events by date, dictionaries for event lookup and preference scores, `HashSet` collections for unique categories and dates, and a bounded `Queue` for recent search history.

**Service Request Status** is implemented in Part 3; see the guide below.

## Part 3: Service Request Status

Select **Service Request Status** from Home or any sidebar. The page uses the same reports saved by Report Issues.

1. Enter a full reference such as `MDV-1004` and click **Track** (or press Enter). Matching ignores case and surrounding spaces. An unknown reference displays a message.
2. Select a status or display order to return to the list. **Priority (active first)** lists Open, In Progress, then Resolved; oldest first within each status. This is a display policy, not emergency triage.
3. Select a row and click **View progress**, or double-click it, to inspect its saved timeline. Resolved reports retain the existing feedback form.
4. Expand **Category and status** and double-click a reference leaf to track it.
5. Select **Related requests and coordination plan** to inspect the selected report's breadth-first related component and a minimum spanning forest over all reports. Links mean shared exact category/location; displayed costs are illustrative coordination scores, not geographic distances. Seed reports have different categories and locations, so they may be isolated.
6. Click **Refresh / clear** to reload saved reports and rebuild the indexes. It clears reference text while retaining the selected filter/order. Data does not update from a live municipal system.

### Data and recovery

Reports and copied attachments normally live in `%LOCALAPPDATA%\MidvaalCitizenServices`. Set `MIDVAAL_DATA_DIRECTORY` before launch to use another folder. Four example reports are created on first use. Back up the entire data folder, including `Attachments`, before moving or replacing data. Never edit XML while the application is running. The repository preserves malformed XML in a `.corrupt-*` copy; if recovery is needed, close the app and restore a known-good backup. This is local coursework storage, not a synchronised municipal database.

### Build directly from the project

If your Visual Studio does not support `.slnx`, open `MunicipalityApp\MunicipalityApp.csproj`. From PowerShell with a .NET SDK/MSBuild and the .NET Framework 4.7.2 targeting pack installed:

```powershell
dotnet build .\MunicipalityApp\MunicipalityApp.csproj --configuration Release
.\MunicipalityApp\bin\Release\MunicipalityApp.exe
```

If a running app locks the output executable, close that instance yourself or use a separate output folder:

```powershell
dotnet build .\MunicipalityApp\MunicipalityApp.csproj --configuration Debug -p:OutputPath=bin\VerificationDebug\
```

Run the executable together with its `Assets` folder and generated configuration file. No network connection or NuGet dependency is required for the app.

### Repeatable verification

From the repository root on Windows:

```powershell
dotnet build .\tests\Part3Checks.csproj --configuration Release
$env:MIDVAAL_DATA_DIRECTORY = Join-Path (Get-Location) 'tests\runtime-data'
.\tests\bin\Part3Checks.exe
Remove-Item Env:MIDVAAL_DATA_DIRECTORY
```

The separate executable compiles the app sources with a test entry point, checks tree/heap/graph invariants and service queries, and constructs the main and status forms. It writes a status preview to `tests/part3-status.png`; data stays in the isolated test folder. This is a custom assertion harness, so run it directly rather than `dotnet test`. Generated binaries, preview and test data are excluded from version control.

### Manual acceptance walkthrough

- Start Home and visit each menu feature; confirm totals and the earlier events/search/reporting functions remain usable.
- Create two test reports with the same category and location in an isolated data directory. Save their unique references; confirm both appear in Service Request Status and in the grouped tree.
- Track each reference, including lower-case input and leading/trailing spaces. Try an unknown reference. Confirm the saved timeline and current status agree.
- Test every status filter, both date orders and priority order. Resolved example MDV-1002 should offer feedback; an open report should not.
- Inspect the related tab: the two identical-category/location reports should be connected with cost 1 and appear in a BFS component. Unrelated examples may be isolated. Check that the plan is labelled illustrative.
- Restart with the same data folder and confirm new reports retain their references and histories. Refresh the page and check there are no duplicate rows.
- From Report New Issue enter draft text and select Service Request Status. Declining discard should retain the draft; accepting should reach status. Check Home, Events and Exit navigation too.
- Resize to the minimum supported window size and check that toolbar controls wrap, the grid and tabs remain usable, and keyboard reference entry works.

### Submission documents

- [Implementation report and rubric mapping](docs/IMPLEMENTATION_REPORT.md): each required structure, its actual role, examples and complexity.
- [Project completion report](docs/PROJECT_COMPLETION_REPORT.md): Parts 1–3, challenges, solutions, learnings and limits.
- [Technology recommendations](docs/TECHNOLOGY_RECOMMENDATIONS.md): benefits, compatibility, costs and official sources.
- [Feedback updates](docs/FEEDBACK_UPDATES.md): implemented improvements and the student-reported 100% results for Parts 1 and 2.

The supplied rubric contains conflicting subtotal/band labels. The implementation report maps every visible requirement without predicting a mark. Parts 1 and 2 each received 100%, as confirmed by the student. Personalise the reflection before submission and include any accompanying written lecturer comments if available.
