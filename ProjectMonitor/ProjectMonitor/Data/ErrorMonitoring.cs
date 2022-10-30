using Humanizer;
using ProjectMonitor.Api.Data.Enums;
using ProjectMonitor.Api.Database.Context;
using ProjectMonitor.Api.Helpers;

namespace ProjectMonitor.Api.Data
{
    public class ErrorMonitoring
    {
        public static void CheckProjectAndSystemHealth()
        {
            using (var context = new DatabaseContext())
            {
                //get all the projects that haven't sent a health check in 30 seconds
                var downProjects = context.ProjectHealth.Where(x => (DateTime.UtcNow - x.LastUpdate).TotalSeconds > 30).ToList();
                var downServers = context.SystemHealth.Where(x => (DateTime.UtcNow - x.LastUpdate).TotalSeconds > 30).ToList();

                //Loop through each project and add into the errors if needed
                foreach (var project in downProjects)
                {
                    //Add in the project to the errors
                    AddNewError(ErrorTypes.ProjectDown, "Project has not sent an update within the last 30 seconds", project.ProjectName);
                }

                foreach (var server in downServers)
                {
                    //Add in the project to the errors
                    AddNewError(ErrorTypes.SystemDown, "System has not sent an update within the last 30 seconds", server.SystemName);
                }
            }
        }

        public static void CheckForProjectErrors()
        {
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
                if (!twitchBotData.DailyPointsEnabled && (DateTime.UtcNow - twitchBotData.StreamLiveTime).TotalMinutes >= 32)
                {
                    AddNewError(ErrorTypes.DailyPointsNotEnabled, "Daily points not enabled after the stream has been live for over 30 minutes", "twitchbot");
                }

                //Check if the discord leaderboards haven't been updated 5 mins after they have meant to be
                if ((new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 2, 0, 0) - twitchBotData.LastDiscordLeaderboardsUpdate).TotalMinutes >= 5)
                {
                    AddNewError(ErrorTypes.DiscordLbNotUpdated, "Discord leaderboards have not been updated at the set time", "twitchbot");
                }

                //Check if the last hours update hasn't happened within the last 2 mins
                if (twitchBotData.StreamStatus && (DateTime.UtcNow - twitchBotData.LastHoursUpdate).TotalMinutes >= 2)
                {
                    AddNewError(ErrorTypes.UserHoursNotUpdated, "User hours have not updated within the last 2 minutes", "twitchbot");
                }
                #endregion

                #region RA
                //Check if the game update has taken over an hour
                if ((new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 4, 0, 0) - raData.LastGameUpdate).TotalMinutes >= 60)
                {
                    AddNewError(ErrorTypes.RAGamesNotUpdated, "RA games did not update in a timely manner", "ratracker");
                }
                #endregion

                #region FM
                //Check if the API has not refreshed within the hour
                if ((DateTime.UtcNow - fmData.LastAPIRefresh).TotalMinutes > 61)
                {
                    AddNewError(ErrorTypes.MonzoApiRefreshError, "Monzo API has not refreshed in the last hour", "financemanager");
                }

                //Check if the status is not success
                if (fmData.LastAPIRefreshStatusCode != "Success")
                {
                    AddNewError(ErrorTypes.MonzoApiRefreshError, "Monzo API has not refreshed successfully", "financemanager");
                }
                #endregion

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
                #endregion
            }
        }

        public static async Task CheckAndEmailOutErrors()
        {
            using (var context = new DatabaseContext())
            {
                var errorsNotSent = context.Errors.Where(x => !x.AlertSent).ToList();
                var errorsResolvedNotSent = context.Errors.Where(x => !x.ResolvedAlertSent && x.AlertSent && x.DateEnded != null).ToList();

                foreach (var error in errorsNotSent)
                {
                    var result = await Alerts.SendEmail(new
                    {
                        projectName = error.ProjectName,
                        projectError = error.ErrorDescription,
                        projectErrorStartDate = TimezoneHelper.ConvertDateTimeToLocalTime(error.DateStarted).Humanize()

                    }, Config.PMErrorsTemplateId);

                    if (result)
                    {
                        error.AlertSent = true;
                    }
                }

                foreach (var resolvedError in errorsResolvedNotSent)
                {
                    var result = await Alerts.SendEmail(new
                    {
                        projectName = resolvedError.ProjectName,
                        projectError = resolvedError.ErrorDescription,
                        projectErrorStartDate = TimezoneHelper.ConvertDateTimeToLocalTime(resolvedError.DateStarted).Humanize(),
                        projectErrorEndDate = TimezoneHelper.ConvertDateTimeToLocalTime(resolvedError.DateEnded.Value).Humanize(),
                        projectDowntimeTotal = resolvedError.DowntimeDuration
                    }, Config.PMErrorsResolvedTemplateId);

                    if (result)
                    {
                        resolvedError.ResolvedAlertSent = true;
                    }
                }

                context.Errors.UpdateRange(errorsNotSent);
                context.Errors.UpdateRange(errorsResolvedNotSent);
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
                    switch (error.ErrorType)
                    {
                        case ErrorTypes.SystemDown:
                            if ((DateTime.UtcNow - context.SystemHealth.Where(x => x.SystemName == error.ProjectName).First().LastUpdate).TotalSeconds < 30)
                            {
                                UpdateErrorAsCompleted(error.ErrorId);
                            }
                            break;
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
                            if ((new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 2, 0, 0) - twitchBotData.LastDiscordLeaderboardsUpdate).TotalMinutes <= 5)
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
                            if ((new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 4, 0, 0) - raData.LastGameUpdate).TotalMinutes <= 60)
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
                            if (fmData.LastAPIRefreshStatusCode == "Success")
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
                        AlertSent = true,
                        ResolvedAlertSent = true,
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
                    AlertSent = false,
                    ResolvedAlertSent = false,
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
                    error.ResolvedAlertSent = true;
                }

                context.Update(error);
                context.SaveChanges();
            }
        }
    }
}
