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

**Service Request Status** remains marked **Coming Soon** for Part 3.
