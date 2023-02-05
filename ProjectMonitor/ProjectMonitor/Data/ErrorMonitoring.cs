using BreganUtils;
using Humanizer;
using ProjectMonitor.Api.Data.Enums;
using ProjectMonitor.Api.Database.Context;
using ProjectMonitor.Api.Helpers;
using Serilog;
using System.Diagnostics;

namespace ProjectMonitor.Api.Data
{
    public class ErrorMonitoring
    {
        public static void CheckProjectAndSystemHealth()
        {
            //Check if the system has recently rebooted, if so then don't process stuff and let the data update
            var projectUptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();

            if (projectUptime.Minutes <= 1)
            {
                return;
            }

            using (var context = new DatabaseContext())
            {
                //get all the projects that haven't sent a health check in 30 seconds
                var downProjects = context.ProjectHealth.Where(x => (DateTime.UtcNow - x.LastUpdate).TotalSeconds > 120).ToList();
                var downServers = context.SystemHealth.Where(x => (DateTime.UtcNow - x.LastUpdate).TotalSeconds > 120).ToList();

                //Loop through each project and add into the errors if needed
                foreach (var project in downProjects)
                {
                    project.ProjectRunning = false;
                    //Add in the project to the errors
                    AddNewError(ErrorTypes.ProjectDown, "Project has not sent an update within the last 30 seconds", project.ProjectName);
                }

                foreach (var server in downServers)
                {
                    server.SystemRunning = false;

                    //Add in the project to the errors
                    AddNewError(ErrorTypes.SystemDown, "System has not sent an update within the last 30 seconds", server.SystemName);
                }

                context.ProjectHealth.UpdateRange(downProjects);
                context.SystemHealth.UpdateRange(downServers);
            }
        }

        public static void CheckForProjectErrors()
        {
            //Check if the system has recently rebooted, if so then don't process stuff and let the data update
            var projectUptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();

            if (projectUptime.Minutes <= 1)
            {
                return;
            }

            using (var context = new DatabaseContext())
            {
                var twitchBotData = context.BreganTwitchBot.Where(x => x.Mode == "release").First();
                var raData = context.RetroAchievementsTracker.Where(x => x.Mode == "release").First();
                var fmData = context.FinanceManager.Where(x => x.Mode == "release").First();
                var catBotData = context.CatBot.Where(x => x.Mode == "release").First();

                #region TwitchBot

                //Check if it's still fully connected
                if (!twitchBotData.TwitchIRCConnectionStatus)
                {
                    AddNewError(ErrorTypes.TwitchIRCError, "Twitch IRC connection is down", "twitchbot");
                }

                if (!twitchBotData.TwitchPubSubConnectionStatus)
                {
                    AddNewError(ErrorTypes.TwitchPubSubError, "Twitch PubSub connection is down", "twitchbot");
                }

                if (!twitchBotData.DiscordConnectionStatus)
                {
                    AddNewError(ErrorTypes.DiscordConnectionError, "Discord connection is down", "twitchbot");
                }

                //Check if the Twitch API has not refreshed within the last 2 hours
                if ((DateTime.UtcNow - twitchBotData.TwitchApiKeyLastRefreshTime).TotalMinutes >= 120)
                {
                    AddNewError(ErrorTypes.TwitchAPIRefreshError, "Twitch API not refreshing", "twitchbot");
                }

                //Check if the bot has not announced the stream within 3 mins of it being live
                if (twitchBotData.StreamStatus && !twitchBotData.StreamAnnounced && (DateTime.UtcNow - twitchBotData.StreamLiveTime).TotalMinutes >= 3)
                {
                    AddNewError(ErrorTypes.StreamNotAnnounced, "Stream not announced after going live", "twitchbot");
                }

                //Check if daily points hasn't been enabled after 32 mins of the stream being live
                if (!twitchBotData.DailyPointsEnabled && twitchBotData.StreamStatus && twitchBotData.StreamAnnounced && (DateTime.UtcNow - twitchBotData.StreamLiveTime).TotalMinutes >= 32)
                {
                    AddNewError(ErrorTypes.DailyPointsNotEnabled, "Daily points not enabled after the stream has been live for over 30 minutes", "twitchbot");
                }

                //Check if the discord leaderboards haven't been updated 5 mins after they have meant to be
                //1440 mins = 1 day, if it goes past that then it's not updated
                if ((DateTime.UtcNow - twitchBotData.LastDiscordLeaderboardsUpdate).TotalMinutes >= 1445)
                {
                    AddNewError(ErrorTypes.DiscordLbNotUpdated, "Discord leaderboards have not been updated at the set time", "twitchbot");
                }

                //Check if the last hours update hasn't happened within the last 2 mins
                if (twitchBotData.StreamStatus && (DateTime.UtcNow - twitchBotData.LastHoursUpdate).TotalMinutes >= 2)
                {
                    AddNewError(ErrorTypes.UserHoursNotUpdated, "User hours have not updated within the last 2 minutes", "twitchbot");
                }

                #endregion TwitchBot

                #region RA

                //Check if the game update has taken over an hour
                //1440 mins = 1 day, if it goes past that then it's not updated
                if ((DateTime.UtcNow - raData.LastGameUpdate).TotalMinutes >= 1500)
                {
                    AddNewError(ErrorTypes.RAGamesNotUpdated, "RA games did not update in a timely manner", "ratracker");
                }

                #endregion RA

                #region FM

                //Check if the API has not refreshed within the hour
                if ((DateTime.UtcNow - fmData.LastAPIRefresh).TotalMinutes > 61)
                {
                    AddNewError(ErrorTypes.MonzoApiRefreshError, "Monzo API has not refreshed in the last hour", "financemanager");
                }

                //Check if the status is not success
                if (fmData.LastAPIRefreshStatusCode != "OK")
                {
                    AddNewError(ErrorTypes.MonzoApiRefreshError, "Monzo API has not refreshed successfully", "financemanager");
                }

                #endregion FM

                #region CatBot

                //Check if discord is not connected
                if (!catBotData.DiscordConnectionStatus)
                {
                    AddNewError(ErrorTypes.CatBotDiscordConnectionError, "cat bot Discord has disconnected", "catbot");
                }

                //Check if the tweet hasnt send in the last 20 mins
                if ((DateTime.UtcNow - catBotData.LastTweet).TotalMinutes > 22)
                {
                    AddNewError(ErrorTypes.LastTweetNotSent, "latest cat bot tweet has not sent", "catbot");
                }

                //
                if ((DateTime.UtcNow - catBotData.LastDiscordPost).TotalMinutes > 22)
                {
                    AddNewError(ErrorTypes.LastDiscordPostNotSent, "latest cat bot discord post has not sent", "catbot");
                }

                #endregion CatBot
            }
        }

        public static void CheckAndSendOutErrors()
        {
            using (var context = new DatabaseContext())
            {
                var errorsNotSent = context.Errors.Where(x => !x.EmailAlertSent || !x.TextAlertSent).ToList();
                var errorsResolvedNotSent = context.Errors.Where(x => !x.EmailResolvedAlertSent && x.DateEnded != null || !x.TextResolvedAlertSent && x.DateEnded != null).ToList();

                foreach (var error in errorsNotSent)
                {
                    if (!error.EmailAlertSent)
                    {
                        var emailContent = new
                        {
                            projectName = error.ProjectName,
                            projectError = error.ErrorDescription,
                            projectErrorStartDate = DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", error.DateStarted).Humanize()
                        };

                        var result = MessageHelper.SendEmail(AppConfig.MMSApiKey, AppConfig.ToEmailAddress, AppConfig.ToEmailAddressName, AppConfig.FromEmailAddress, AppConfig.FromEmailAddressName, emailContent, AppConfig.PMErrorsTemplateId);

                        if (result)
                        {
                            error.EmailAlertSent = true;
                        }
                    }

                    if (!error.TextAlertSent)
                    {
                        var messageContent = $"Project Errored: {error.ProjectName} \n Description: {error.ErrorDescription} \n Date started: {DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", error.DateStarted).Humanize()}";
                        var result = MessageHelper.SendTextMessage(AppConfig.MMSApiKey, AppConfig.ChatId, messageContent);

                        if (result)
                        {
                            error.TextAlertSent = true;
                        }
                    }
                }

                foreach (var resolvedError in errorsResolvedNotSent)
                {
                    if (!resolvedError.EmailResolvedAlertSent)
                    {
                        var emailContent = new
                        {
                            projectName = resolvedError.ProjectName,
                            projectError = resolvedError.ErrorDescription,
                            projectErrorStartDate = DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", resolvedError.DateStarted).Humanize(),
                            projectErrorEndDate = DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", resolvedError.DateEnded.Value).Humanize(),
                            projectDowntimeTotal = resolvedError.DowntimeDuration.Humanize()
                        };

                        var result = MessageHelper.SendEmail(AppConfig.MMSApiKey, AppConfig.ToEmailAddress, AppConfig.ToEmailAddressName, AppConfig.FromEmailAddress, AppConfig.FromEmailAddressName, emailContent, AppConfig.PMErrorsResolvedTemplateId);

                        if (result)
                        {
                            resolvedError.EmailResolvedAlertSent = true;
                        }
                    }

                    if (!resolvedError.TextResolvedAlertSent)
                    {
                        var messageContent = $"Error Resolved for project \n {resolvedError.ProjectName} \n Description: {resolvedError.ErrorDescription} \n Date ended: {DateTimeHelper.ConvertDateTimeToLocalTime("GMT Standard Time", resolvedError.DateEnded.Value).Humanize()} \n Downtime: {resolvedError.DowntimeDuration.Humanize()}";
                        var result = MessageHelper.SendTextMessage(AppConfig.MMSApiKey, AppConfig.ChatId, messageContent);

                        if (result)
                        {
                            resolvedError.TextResolvedAlertSent = true;
                        }
                    }
                }

                context.SaveChanges();
            }
        }

        public static void CheckErrorsAndCompleteIfFixed()
        {
            using (var context = new DatabaseContext())
            {
                //get all the errors that do not have an end date
                var currentErrors = context.Errors.Where(x => x.DateEnded == null).ToList();

                //Check if there are any errors to process
                if (currentErrors.Count == 0)
                {
                    return;
                }

                var twitchBotData = context.BreganTwitchBot.Where(x => x.Mode == "release").First();
                var raData = context.RetroAchievementsTracker.Where(x => x.Mode == "release").First();
                var fmData = context.FinanceManager.Where(x => x.Mode == "release").First();
                var catBotData = context.CatBot.Where(x => x.Mode == "release").First();

                foreach (var error in currentErrors)
                {
                    try
                    {
                        switch (error.ErrorType)
                        {
                            case ErrorTypes.ProjectDown:
                                if ((DateTime.UtcNow - context.ProjectHealth.Where(x => x.ProjectName == error.ProjectName).First().LastUpdate).TotalSeconds < 30)
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            case ErrorTypes.ProjectCrashed:
                                UpdateErrorAsCompleted(error.ErrorId);
                                break;

                            case ErrorTypes.TwitchIRCError:
                                if (twitchBotData.TwitchIRCConnectionStatus)
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            case ErrorTypes.TwitchPubSubError:
                                if (twitchBotData.TwitchPubSubConnectionStatus)
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            case ErrorTypes.DiscordConnectionError:
                                if (twitchBotData.DiscordConnectionStatus)
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            case ErrorTypes.TwitchAPIRefreshError:
                                if ((DateTime.UtcNow - twitchBotData.TwitchApiKeyLastRefreshTime).TotalMinutes <= 120)
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            case ErrorTypes.StreamNotAnnounced:
                                if (twitchBotData.StreamStatus && twitchBotData.StreamAnnounced)
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            case ErrorTypes.DailyPointsNotEnabled:
                                if (twitchBotData.DailyPointsEnabled)
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            case ErrorTypes.DiscordLbNotUpdated:
                                if ((DateTime.UtcNow - twitchBotData.LastDiscordLeaderboardsUpdate).TotalMinutes <= 1445)
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            case ErrorTypes.UserHoursNotUpdated:
                                if ((twitchBotData.StreamStatus && (DateTime.UtcNow - twitchBotData.LastHoursUpdate).TotalMinutes <= 2))
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            case ErrorTypes.RAGamesNotUpdated:
                                if ((DateTime.UtcNow - raData.LastGameUpdate).TotalMinutes <= 1500)
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            case ErrorTypes.MonzoApiRefreshError:
                                if ((DateTime.UtcNow - fmData.LastAPIRefresh).TotalMinutes < 61)
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            case ErrorTypes.MonzoApiStatusError:
                                if (fmData.LastAPIRefreshStatusCode == "OK")
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            case ErrorTypes.CatBotDiscordConnectionError:
                                if (catBotData.DiscordConnectionStatus)
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            case ErrorTypes.LastTweetNotSent:
                                if ((DateTime.UtcNow - catBotData.LastTweet).TotalMinutes < 22)
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            case ErrorTypes.LastDiscordPostNotSent:
                                if ((DateTime.UtcNow - catBotData.LastDiscordPost).TotalMinutes < 22)
                                {
                                    UpdateErrorAsCompleted(error.ErrorId);
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Fatal($"[Error Checker] error - {e}");
                        continue;
                    }

                }
            }
        }

        public static void AddNewError(string errorType, string errorDescription, string projectName)
        {
            using (var context = new DatabaseContext())
            {
                //Check if the error has already been logged
                if (context.Errors.Any(x => x.ProjectName == projectName && x.ErrorDescription == errorDescription && x.DateEnded == null))
                {
                    return;
                }

                //Check if the error has already been logged in the last 10 mins, don't want to keep spamming out emails
                //This would only be the case with project down notifications
                if (context.Errors.Any(x => x.ProjectName == projectName && x.ErrorType == errorType && x.DateEnded != null && (DateTime.UtcNow - x.DateEnded.Value).TotalMinutes <= 10))
                {
                    //Still want to record it but not keep spamming out emails
                    context.Errors.Add(new Database.Models.Errors
                    {
                        EmailAlertSent = true,
                        EmailResolvedAlertSent = true,
                        TextAlertSent = true,
                        TextResolvedAlertSent = true,
                        DateEnded = DateTime.UtcNow,
                        DateStarted = DateTime.UtcNow,
                        DowntimeDuration = TimeSpan.FromSeconds(0),
                        ErrorType = errorType,
                        ErrorDescription = errorDescription,
                        ProjectName = projectName
                    });

                    context.SaveChanges();
                    return;
                }

                context.Errors.Add(new Database.Models.Errors
                {
                    EmailAlertSent = false,
                    EmailResolvedAlertSent = false,
                    TextAlertSent = false,
                    TextResolvedAlertSent = false,
                    DateEnded = null,
                    DateStarted = DateTime.UtcNow,
                    DowntimeDuration = TimeSpan.FromSeconds(0),
                    ErrorType = errorType,
                    ErrorDescription = errorDescription,
                    ProjectName = projectName
                });

                context.SaveChanges();
            }
        }

        private static void UpdateErrorAsCompleted(int errorId)
        {
            using (var context = new DatabaseContext())
            {
                var error = context.Errors.FirstOrDefault(x => x.ErrorId == errorId);

                error.DateEnded = DateTime.UtcNow;
                error.DowntimeDuration = DateTime.UtcNow - error.DateStarted;

                //Don't need to send a resolved for that one, especially if it's stuck in some crash loop
                if (error.ErrorType == ErrorTypes.ProjectCrashed)
                {
                    error.EmailResolvedAlertSent = true;
                    error.TextResolvedAlertSent = true;
                }

                context.SaveChanges();
            }
        }
    }
}