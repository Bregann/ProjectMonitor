export interface DashboardData {
    servers?:                  Server[];
    projectStatus?:            ProjectStatus[];
    twitchBot?:                TwitchBot;
    retroAchievementsTracker?: RetroAchievementsTracker;
    financeManager?:           FinanceManager;
    catBot?:                   CatBot;
    lastUpdate?:               string;
    error:                     string;
}

export interface CatBot {
    discordConnectionStatus: boolean;
    lastTweet:               string;
    lastDiscordPost:         string;
}

export interface FinanceManager {
    lastTransactionUpdate:    string;
    lastAPIRefresh:           string;
    lastAPIRefreshStatusCode: string;
}

export interface ProjectStatus {
    projectName:    string;
    cpuUsage:       number;
    ramUsage:       number;
    projectUptime:  string;
    projectRunning: boolean;
}

export interface RetroAchievementsTracker {
    totalGames:     number;
    totalUsers:     number;
    lastGameUpdate: string;
    gamesUpdated:   number;
}

export interface Server {
    serverName:   string;
    systemUptime: string;
}

export interface TwitchBot {
    usersInStream:                 number;
    twitchIRCConnectionStatus:     boolean;
    twitchPubSubConnectionStatus:  boolean;
    twitchApiKeyLastRefreshTime:   string;
    discordConnectionStatus:       boolean;
    streamAnnounced:               boolean;
    streamStatus:                  boolean;
    streamUptime:                  string;
    dailyPointsEnabled:            boolean;
    lastDiscordLeaderboardsUpdate: string;
    lastHoursUpdate:               string;
}