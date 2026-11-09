using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectMonitor.Domain.Database.Models;
using ProjectMonitor.Domain.Enums;

namespace ProjectMonitor.Domain.Database.Context
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IDataProtectionKeyContext
    {
        public DbSet<EnvironmentalSetting> EnvironmentalSettings { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<ProjectCheck> ProjectChecks { get; set; } = null!;
        public DbSet<HttpCheck> HttpChecks { get; set; } = null!;
        public DbSet<HeartbeatCheck> HeartbeatChecks { get; set; } = null!;
        public DbSet<ValueCheck> ValueChecks { get; set; } = null!;
        public DbSet<UptimeCheck> UptimeChecks { get; set; } = null!;
        public DbSet<ReportedValue> ReportedValues { get; set; } = null!;

        /// <summary>Keys that sign login cookies, kept in the database so redeploys don't log you out.</summary>
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EnvironmentalSetting>().HasIndex(x => x.Key).IsUnique();

            modelBuilder.Entity<Project>().HasIndex(x => x.Slug).IsUnique();
            modelBuilder.Entity<Project>().HasIndex(x => x.ApiKey).IsUnique();

            // All check types share one table, with a Type column saying which one each row is
            modelBuilder.Entity<ProjectCheck>()
                .HasDiscriminator<CheckTypeEnum>("Type")
                .HasValue<HttpCheck>(CheckTypeEnum.Http)
                .HasValue<HeartbeatCheck>(CheckTypeEnum.Heartbeat)
                .HasValue<ValueCheck>(CheckTypeEnum.Value);
            modelBuilder.Entity<ProjectCheck>().Property("Type").HasConversion<string>().HasMaxLength(50);
            modelBuilder.Entity<ProjectCheck>().Property(x => x.FailureLevel).HasConversion<string>().HasMaxLength(20);
            modelBuilder.Entity<ValueCheck>().Property(x => x.Condition).HasConversion<string>().HasMaxLength(30);

            modelBuilder.Entity<ProjectCheck>()
                .HasOne(x => x.Project)
                .WithMany(x => x.Checks)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UptimeCheck>().HasIndex(x => new { x.HttpCheckId, x.CheckedAtUtc });
            modelBuilder.Entity<UptimeCheck>().HasIndex(x => x.CheckedAtUtc);
            modelBuilder.Entity<UptimeCheck>()
                .HasOne(x => x.HttpCheck)
                .WithMany()
                .HasForeignKey(x => x.HttpCheckId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReportedValue>().HasIndex(x => new { x.ProjectId, x.Key, x.ReportedAtUtc });
            modelBuilder.Entity<ReportedValue>().HasIndex(x => x.ReportedAtUtc);
            modelBuilder.Entity<ReportedValue>()
                .HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
