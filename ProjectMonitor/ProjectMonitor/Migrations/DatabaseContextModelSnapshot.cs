﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ProjectMonitor.Api.Database.Context;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ProjectMonitor.Api.Database.Models.BreganTwitchBot", b =>
                {
                    b.Property<string>("Mode")
                        .HasColumnType("text");

                    b.Property<bool>("DailyPointsEnabled")
                        .HasColumnType("boolean");

                    b.Property<bool>("DiscordConnectionStatus")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastDiscordLeaderboardsUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastHoursUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("StreamAnnounced")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("StreamLiveTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("StreamStatus")
                        .HasColumnType("boolean");

                    b.Property<TimeSpan>("StreamUptime")
                        .HasColumnType("interval");

                    b.Property<DateTime>("TwitchApiKeyLastRefreshTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("TwitchIRCConnectionStatus")
                        .HasColumnType("boolean");

                    b.Property<bool>("TwitchPubSubConnectionStatus")
                        .HasColumnType("boolean");

                    b.Property<long>("UsersInStream")
                        .HasColumnType("bigint");

                    b.HasKey("Mode");

                    b.ToTable("BreganTwitchBot");

                    b.HasData(
                        new
                        {
                            Mode = "debug",
                            DailyPointsEnabled = false,
                            DiscordConnectionStatus = true,
                            LastDiscordLeaderboardsUpdate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            LastHoursUpdate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            StreamAnnounced = false,
                            StreamLiveTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            StreamStatus = false,
                            StreamUptime = new TimeSpan(0, 0, 0, 0, 0),
                            TwitchApiKeyLastRefreshTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            TwitchIRCConnectionStatus = true,
                            TwitchPubSubConnectionStatus = true,
                            UsersInStream = 0L
                        },
                        new
                        {
                            Mode = "release",
                            DailyPointsEnabled = false,
                            DiscordConnectionStatus = true,
                            LastDiscordLeaderboardsUpdate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            LastHoursUpdate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            StreamAnnounced = false,
                            StreamLiveTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            StreamStatus = false,
                            StreamUptime = new TimeSpan(0, 0, 0, 0, 0),
                            TwitchApiKeyLastRefreshTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            TwitchIRCConnectionStatus = true,
                            TwitchPubSubConnectionStatus = true,
                            UsersInStream = 0L
                        });
                });

            modelBuilder.Entity("ProjectMonitor.Api.Database.Models.CatBot", b =>
                {
                    b.Property<string>("Mode")
                        .HasColumnType("text");

                    b.Property<bool>("DiscordConnectionStatus")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastDiscordPost")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastTweet")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Mode");

                    b.ToTable("CatBot");

                    b.HasData(
                        new
                        {
                            Mode = "debug",
                            DiscordConnectionStatus = true,
                            LastDiscordPost = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            LastTweet = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                        },
                        new
                        {
                            Mode = "release",
                            DiscordConnectionStatus = true,
                            LastDiscordPost = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            LastTweet = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                        });
                });

            modelBuilder.Entity("ProjectMonitor.Api.Database.Models.Config", b =>
                {
                    b.Property<int>("RowId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RowId"));

                    b.Property<string>("ApiKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ChatId")
                        .HasColumnType("integer");

                    b.Property<string>("FromEmailAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FromEmailAddressName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HFConnectionString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HangfirePassword")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HangfireUsername")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MMSApiKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PMErrorsResolvedTemplateId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PMErrorsTemplateId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ToEmailAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ToEmailAddressName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("RowId");

                    b.ToTable("Config");

                    b.HasData(
                        new
                        {
                            RowId = 1,
                            ApiKey = "",
                            ChatId = 0,
                            FromEmailAddress = "",
                            FromEmailAddressName = "",
                            HFConnectionString = "",
                            HangfirePassword = "",
                            HangfireUsername = "",
                            MMSApiKey = "",
                            PMErrorsResolvedTemplateId = "",
                            PMErrorsTemplateId = "",
                            ToEmailAddress = "",
                            ToEmailAddressName = ""
                        });
                });

            modelBuilder.Entity("ProjectMonitor.Api.Database.Models.Errors", b =>
                {
                    b.Property<int>("ErrorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ErrorId"));

                    b.Property<DateTime?>("DateEnded")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateStarted")
                        .HasColumnType("timestamp with time zone");

                    b.Property<TimeSpan>("DowntimeDuration")
                        .HasColumnType("interval");

                    b.Property<bool>("EmailAlertSent")
                        .HasColumnType("boolean");

                    b.Property<bool>("EmailResolvedAlertSent")
                        .HasColumnType("boolean");

                    b.Property<string>("ErrorDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ErrorType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("TextAlertSent")
                        .HasColumnType("boolean");

                    b.Property<bool>("TextResolvedAlertSent")
                        .HasColumnType("boolean");

                    b.HasKey("ErrorId");

                    b.ToTable("Errors");
                });

            modelBuilder.Entity("ProjectMonitor.Api.Database.Models.FinanceManager", b =>
                {
                    b.Property<string>("Mode")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastAPIRefresh")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastAPIRefreshStatusCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("LastTransactionUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Mode");

                    b.ToTable("FinanceManager");

                    b.HasData(
                        new
                        {
                            Mode = "debug",
                            LastAPIRefresh = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            LastAPIRefreshStatusCode = "Success",
                            LastTransactionUpdate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                        },
                        new
                        {
                            Mode = "release",
                            LastAPIRefresh = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            LastAPIRefreshStatusCode = "Success",
                            LastTransactionUpdate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                        });
                });

            modelBuilder.Entity("ProjectMonitor.Api.Database.Models.ProjectHealth", b =>
                {
                    b.Property<string>("ProjectName")
                        .HasColumnType("text");

                    b.Property<double>("CPUUsage")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("ProjectRunning")
                        .HasColumnType("boolean");

                    b.Property<TimeSpan>("ProjectUptime")
                        .HasColumnType("interval");

                    b.Property<long>("RAMUsage")
                        .HasColumnType("bigint");

                    b.HasKey("ProjectName");

                    b.ToTable("ProjectHealth");
                });

            modelBuilder.Entity("ProjectMonitor.Api.Database.Models.RetroAchievementsTracker", b =>
                {
                    b.Property<string>("Mode")
                        .HasColumnType("text");

                    b.Property<long>("GamesUpdated")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("LastGameUpdate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("TotalGames")
                        .HasColumnType("bigint");

                    b.Property<long>("TotalUsers")
                        .HasColumnType("bigint");

                    b.HasKey("Mode");

                    b.ToTable("RetroAchievementsTracker");

                    b.HasData(
                        new
                        {
                            Mode = "debug",
                            GamesUpdated = 0L,
                            LastGameUpdate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            TotalGames = 0L,
                            TotalUsers = 0L
                        },
                        new
                        {
                            Mode = "release",
                            GamesUpdated = 0L,
                            LastGameUpdate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            TotalGames = 0L,
                            TotalUsers = 0L
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
