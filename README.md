# Project Monitor

One dashboard for all my projects: is everything up, is CI green, are there vulnerabilities, and what needs my attention.

Built with Blazor Server (.NET 10) and MudBlazor. It **pulls** everything itself (HTTP checks and the GitHub API), so projects don't need any monitoring code.

## Pages

- **At a glance** (`/`): summary tiles (healthy projects, services up, critical/high vulnerabilities, running workflows), a table of every project, a "Needs attention" list and what's running in GitHub Actions right now.
- **Per project** (`/projects/{slug}`): tabs for Issues, Services (uptime, response time, a 24-hour strip), Actions (latest run of each workflow on `main` plus recent runs), Pull requests and Security (Dependabot alerts).

Pages update live: no refresh needed.

## What counts as a problem

| Level | When |
|---|---|
| 🔴 Critical | A service is down (2 failed checks in a row), a **critical** vulnerability, or a Docker deploy or Renovate run failing on `main` |
| 🟠 Warning | A **high** vulnerability, 24h uptime under 99%, another workflow failing on `main`, or GitHub data more than 15 minutes stale |
| ℹ️ Info | Medium/low vulnerabilities, Renovate PRs waiting for review, an open "Expo updates needed" issue |

Critical problems send a Telegram message through CommsSender when they start and when they clear (if alerts are configured).

## Configuration

Projects and their URLs live in `ProjectMonitor.Core/appsettings.json` under `Monitor:Projects`. To add a project, add an entry and redeploy:

```json
{
  "Slug": "orbit",
  "Name": "Orbit",
  "Description": "Finance manager",
  "Repository": "Orbit",
  "Endpoints": [
    { "Name": "API", "Url": "https://orbitapi.bregan.me" },
    { "Name": "Website", "Url": "https://orbit.bregan.me", "ExpectedStatusCodes": [200] }
  ]
}
```

By default, any HTTP response below 500 counts as "up" (an API answering 401 or 404 on its root is still running). Set `ExpectedStatusCodes` to be stricter.

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
| `PruneUptimeChecks` | hourly | Deletes checks older than `UptimeRetentionDays` |

## Deployment

Pushing to `main` builds the image on the Unraid runner and pushes `192.168.1.100:5000/projectmonitor:latest`. Then force-update the container on Unraid. It listens on port **8080**, and `/healthz` returns 200 for health checks.

## Project layout

Same structure as the other projects:

- **ProjectMonitor.Core**: the Blazor front end. Pages only call `IDashboardService` and render the DTOs it returns.
- **ProjectMonitor.Domain**: everything else.
  - `Database/`: context, models and migrations
  - `DTOs/`: `Configuration`, `GitHub` (API responses), `Monitoring` (in-memory state), `Dashboard` (what pages receive)
  - `Services/`: `Dashboard` (for the front end), plus `Uptime`, `GitHub` and `Alerts` (Hangfire jobs)
  - `Helpers/`: API clients, settings, health rules and job setup
  - `Interfaces/`: `Api`, `Jobs` and `Helpers`
- **ProjectMonitor.Tests**: NUnit tests

## Development

Run `ProjectMonitor.Core` in Debug (Visual Studio or `dotnet run --project ProjectMonitor.Core`). Like Orbit, Debug starts a **Testcontainers PostgreSQL** database (Docker needs to be running) and Hangfire uses in-memory storage. The test database is seeded with the password `password`. To see real security alerts locally, set the `PMGitHubToken` environment variable before running.

Tests: `dotnet test`. After changing `AppDbContext`, add a migration:

```bash
dotnet tool restore
dotnet ef migrations add <Name> --project ProjectMonitor.Domain --startup-project ProjectMonitor.Domain --output-dir Database/Migrations
```
