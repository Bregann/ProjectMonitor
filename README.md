# Project Monitor

One dashboard for all my projects: is everything up, is CI green, are there vulnerabilities, and what needs my attention.

Built with Blazor Server (.NET 10) and MudBlazor. It **pulls** everything itself (HTTP checks and the GitHub API), so projects don't need any monitoring code.

## Pages

- **At a glance** (`/`): summary tiles (healthy projects, services up, critical/high vulnerabilities, running workflows), a table of every project, a "Needs attention" list and what's running in GitHub Actions right now.
- **Per project** (`/projects/{slug}`): tabs for Issues, Services (uptime, response time, a 24-hour strip), Actions (latest run of each workflow on `main` plus recent runs), Pull requests and Security (Dependabot alerts).

Pages update live: no refresh needed.

## Projects and checks

Projects are managed on the site (**Manage projects**), not in config. Each project has an optional GitHub repository, its own **API key**, and any number of **checks**:

| Check type | How it works | Example |
|---|---|---|
| **Website / API** | The monitor requests a URL every minute | `https://orbitapi.bregan.me` answers |
| **Heartbeat** | The project must report something at least every N minutes | Twitch bot reports at least every 5 min |
| **Reported value** | A rule on a value the project reports | `discordConnected` is true, `queueLength` < 50, `lastLeaderboardUpdate` within 1445 min |

Each check fails as a **Warning** or **Critical**. Critical failures send a Telegram message through CommsSender when they start and when they clear. On first start, the existing projects are added automatically (with their API checks), so nothing has to be typed in.

### Reporting values from a project

Send any values you like to `POST /api/report` with the project's key (copy it from **Manage projects**). Values can be strings, numbers or true/false. Every value is stored with a timestamp, the project's checks are re-evaluated straight away, and the latest values show on the project's **Checks** tab.

```bash
curl -X POST https://<monitor>/api/report \
  -H "Content-Type: application/json" \
  -H "X-ProjectMonitor-ApiKey: <key>" \
  -d '{"values": {"discordConnected": true, "queueLength": 3, "lastLeaderboardUpdate": "2026-11-09T10:00:00Z"}}'
```

From a .NET project, e.g. as a Hangfire job every minute:

```csharp
await httpClient.PostAsJsonAsync("https://<monitor>/api/report", new
{
    values = new Dictionary<string, object>
    {
        ["discordConnected"] = discordClient.ConnectionState == ConnectionState.Connected,
        ["lastLeaderboardUpdate"] = lastLeaderboardUpdate.ToString("O")
    }
});
```

(Set the `X-ProjectMonitor-ApiKey` header on the client.) The response is `200` when the values are recorded, `401` for a wrong key, and `400` for values that aren't strings, numbers or true/false (max 100 values, 1000 characters each). An empty report still counts as a heartbeat.

### Adding a new check type

1. Add a subclass of `ProjectCheck` in `Database/Models/ProjectCheck.cs` and a value to `CheckTypeEnum`. Register it in the discriminator in `AppDbContext`, then add a migration.
2. Add an `ICheckEvaluator` in `Helpers/Checks/` that turns the check into a `CheckStatus`, and register it in `Program.cs`.
3. Add its fields to `SaveCheckRequest`, `ProjectsService` (validate/apply) and `Components/Dialogs/CheckDialog.razor`, plus a line in `CheckDescriptionHelper`.

## What counts as a problem

| Level | When |
|---|---|
| 🔴 Critical | A Website/API check is down (2 failures in a row), a Critical check fails, a **critical** vulnerability, or a Docker deploy or Renovate run failing on `main` |
| 🟠 Warning | A Warning check fails, a **high** vulnerability, 24h uptime under 99%, another workflow failing on `main`, or GitHub data more than 15 minutes stale |
| ℹ️ Info | Medium/low vulnerabilities, Renovate PRs waiting for review, an open "Expo updates needed" issue |

## Configuration

`ProjectMonitor.Core/appsettings.json` (`Monitor` section) only holds general settings: the GitHub owner, the uptime timeout, how long history is kept, and how many failed checks in a row count as down.

### Settings (EnvironmentalSettings table)

Like the other projects, secrets live in the `EnvironmentalSettings` table (`Key` / `Value`) and are loaded at startup. **Restart the container after changing them.**

| Key | Required | What |
|---|---|---|
| `DashboardPassword` | ✅ | The password for the login page |
| `GitHubToken` | ✅ | GitHub token (see below) |
| `CommsSenderApiKey` | for alerts | CommsSender's **server** key |
| `TelegramChatId` | for alerts | Telegram chat to send alerts to |

```sql
INSERT INTO "EnvironmentalSettings" ("Key", "Value") VALUES
  ('DashboardPassword', '...'),
  ('GitHubToken', 'github_pat_...');
```

The only environment variable is `PMConnString` (the PostgreSQL connection string). Tables, including Hangfire's, are created automatically on startup.

### GitHub token

Create a **fine-grained** token at https://github.com/settings/personal-access-tokens/new with access to every repo in the project list, and **read-only** permissions:

- Actions
- Contents
- Issues
- Pull requests
- Dependabot alerts
- Metadata (automatic)

This is separate from Renovate's token and can only read.

## Background jobs

All background work is Hangfire recurring jobs (`Helpers/HangfireJobSetup.cs`), visible at `/hangfire` (behind the same login):

| Job | Schedule | What |
|---|---|---|
| `CheckUptime` | every minute | HTTP check of every endpoint |
| `PollGitHub` | every 2 minutes | Actions, PRs, security alerts and issues for every repo |
| `CheckAlerts` | every 30 seconds | Telegram alert when a critical problem starts or clears |
| `EvaluateChecks` | every 30 seconds | Re-evaluates heartbeat and value checks (also runs straight after each report) |
| `PruneUptimeChecks` | hourly | Deletes uptime results older than `UptimeRetentionDays` |
| `PruneReportedValues` | hourly | Deletes reported values older than `ReportedValueRetentionDays` |

## Deployment

Pushing to `main` builds the image on the Unraid runner and pushes `192.168.1.100:5000/projectmonitor:latest`. Then force-update the container on Unraid. It listens on port **8080**, and `/healthz` returns 200 for health checks.

## Project layout

Same structure as the other projects:

- **ProjectMonitor.Core**: the Blazor front end. Pages only call `IDashboardService` and render the DTOs it returns.
- **ProjectMonitor.Domain**: everything else.
  - `Database/`: context, models and migrations
  - `DTOs/`: `Configuration`, `GitHub` (API responses), `Monitoring` (in-memory state), `Dashboard` (what pages receive)
  - `Services/`: `Dashboard`, `Projects` and `Reports` (for the front end and API), plus `Uptime`, `GitHub`, `Checks` and `Alerts` (Hangfire jobs)
  - `Helpers/`: API clients, settings, health rules, check evaluators (`Helpers/Checks`) and job setup
  - `Interfaces/`: `Api`, `Jobs` and `Helpers`
- **ProjectMonitor.Tests**: NUnit tests

## Development

Run `ProjectMonitor.Core` in Debug (Visual Studio or `dotnet run --project ProjectMonitor.Core`). Like Orbit, Debug starts a **Testcontainers PostgreSQL** database (Docker needs to be running) and Hangfire uses in-memory storage. The test database is seeded with the password `password`. To see real security alerts locally, set the `PMGitHubToken` environment variable before running.

Tests: `dotnet test`. After changing `AppDbContext`, add a migration through Core (the Release configuration skips starting the test container; the connection string isn't used):

```bash
dotnet tool restore
PMConnString="Host=localhost" dotnet ef migrations add <Name> --project ProjectMonitor.Domain --startup-project ProjectMonitor.Core --configuration Release --output-dir Database/Migrations
```
