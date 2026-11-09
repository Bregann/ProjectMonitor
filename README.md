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

### Environment variables (Unraid container)

| Variable | Required | What |
|---|---|---|
| `PMConnString` | ✅ | PostgreSQL connection string. Tables are created automatically on startup. |
| `Monitor__DashboardPassword` | ✅ | The password for the login page |
| `Monitor__GitHubToken` | ✅ | GitHub token (see below) |
| `Monitor__Alerts__CommsSenderApiKey` | for alerts | CommsSender's **server** key (`CommsSenderApiKey`) |
| `Monitor__Alerts__TelegramChatId` | for alerts | Telegram chat to send alerts to |

Any setting in `appsettings.json` can be overridden the same way, e.g. `Monitor__UptimeCheckSeconds=30`.

### GitHub token

Create a **fine-grained** token at https://github.com/settings/personal-access-tokens/new with access to every repo in the project list, and **read-only** permissions:

- Actions
- Contents
- Issues
- Pull requests
- Dependabot alerts
- Metadata (automatic)

This is separate from Renovate's token and can only read.

## Deployment

Pushing to `main` builds the image on the Unraid runner and pushes `192.168.1.100:5000/projectmonitor:latest`. Then force-update the container on Unraid. It listens on port **8080**, and `/healthz` returns 200 for health checks.

## Development

```bash
dotnet run --project ProjectMonitor.Core
```

Without `PMConnString` it uses an in-memory database, and without a dashboard password any password works (Development only). Tests: `dotnet test`. After changing `MonitorDbContext`, add a migration:

```bash
dotnet tool restore
dotnet ef migrations add <Name> --project ProjectMonitor.Domain --startup-project ProjectMonitor.Domain --output-dir Data/Migrations
```
