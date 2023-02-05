using BreganUtils;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using ProjectMonitor.Api.Database.Context;
using ProjectMonitor.Api.Models.GetData;

namespace ProjectMonitor.Api.Controllers.GetData
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetDataController : ControllerBase
    {
        [HttpGet("GetDashboardData")]
        public ActionResult<GetDashboardDataResponseDto> GetDashboardData()
        {
            using (var context = new DatabaseContext())
            {
                //Get the project statuses
                var projects = context.ProjectHealth.OrderBy(x => x.ProjectName);
                var projectList = new List<ProjectStatus>();

                foreach (var project in projects)
                {
                    projectList.Add(new ProjectStatus
                    {
                        ProjectName = project.ProjectName,
                        CPUUsage = Math.Round(project.CPUUsage, 2),
                        RAMUsage = project.RAMUsage,
                        ProjectRunning = project.ProjectRunning,
                        ProjectUptime = project.ProjectUptime.Humanize(3, minUnit: Humanizer.Localisation.TimeUnit.Second)
                    });
                }

                //Get the data for each project
                var twitchBot = context.BreganTwitchBot.Where(x => x.Mode == "release").FirstOrDefault();
                var retroAchievements = context.RetroAchievementsTracker.Where(x => x.Mode == "release").FirstOrDefault();
                var financeManger = context.FinanceManager.Where(x => x.Mode == "release").FirstOrDefault();
                var catBot = context.CatBot.Where(x => x.Mode == "release").FirstOrDefault();

                //define the objects to add into the dto
                var twitchBotData = new TwitchBot
                {
                    UsersInStream = twitchBot.UsersInStream,
                    TwitchIRCConnectionStatus = twitchBot.TwitchIRCConnectionStatus,
                    TwitchPubSubConnectionStatus = twitchBot.TwitchPubSubConnectionStatus,
                    TwitchApiKeyLastRefreshTime = DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", twitchBot.TwitchApiKeyLastRefreshTime).Humanize(),
                    DiscordConnectionStatus = twitchBot.DiscordConnectionStatus,
                    StreamAnnounced = twitchBot.StreamAnnounced,
                    StreamStatus = twitchBot.StreamStatus,
                    StreamUptime = twitchBot.StreamUptime.Humanize(3, minUnit: Humanizer.Localisation.TimeUnit.Second),
                    DailyPointsEnabled = twitchBot.DailyPointsEnabled,
                    LastDiscordLeaderboardsUpdate = DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", twitchBot.TwitchApiKeyLastRefreshTime).Humanize(),
                    LastHoursUpdate = DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", twitchBot.LastHoursUpdate).Humanize()
                };

                var retroAchievmentsTrackerData = new RetroAchievementsTracker
                {
                    TotalGames = retroAchievements.TotalGames,
                    TotalUsers = retroAchievements.TotalUsers,
                    LastGameUpdate = DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", retroAchievements.LastGameUpdate).Humanize(),
                    GamesUpdated = retroAchievements.GamesUpdated
                };

                var financeManagerData = new FinanceManager
                {
                    LastTransactionUpdate = DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", financeManger.LastTransactionUpdate).Humanize(),
                    LastAPIRefresh = DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", financeManger.LastAPIRefresh).Humanize(),
                    LastAPIRefreshStatusCode = financeManger.LastAPIRefreshStatusCode
                };

                var catBotData = new CatBot
                {
                    DiscordConnectionStatus = catBot.DiscordConnectionStatus,
                    LastTweet = DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", catBot.LastTweet).Humanize(),
                    LastDiscordPost = DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", catBot.LastDiscordPost).Humanize()
                };

                return new GetDashboardDataResponseDto
                {
                    ProjectStatus = projectList,
                    TwitchBot = twitchBotData,
                    RetroAchievementsTracker = retroAchievmentsTrackerData,
                    FinanceManager = financeManagerData,
                    CatBot = catBotData,
                    LastUpdate = DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", DateTime.UtcNow).ToString("dd/MM @ HH:mm:ss")
                };
            }
        }

        [HttpGet("GetActiveErrors")]
        public ActionResult<List<GetActiveErrorsResponseDto>> GetActiveErrors()
        {
            var activeErrors = new List<GetActiveErrorsResponseDto>();

            using (var conntext = new DatabaseContext())
            {
                var errorsFromDb = conntext.Errors.Where(x => x.DateEnded == null).ToList();

                if (errorsFromDb.Count == 0)
                {
                    return NotFound();
                }

                foreach (var error in errorsFromDb)
                {
                    activeErrors.Add(new GetActiveErrorsResponseDto
                    {
                        ErrorId = error.ErrorId,
                        DateStarted = DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", error.DateStarted).Humanize(),
                        ProjectName = error.ProjectName,
                        ErrorDescription = error.ErrorDescription
                    });
                }
            }

            return Ok(activeErrors);
        }
    }
}