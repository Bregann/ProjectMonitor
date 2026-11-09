using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectMonitor.Domain.Database.Models;

namespace ProjectMonitor.Domain.Database.Context
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IDataProtectionKeyContext
    {
        public DbSet<EnvironmentalSetting> EnvironmentalSettings { get; set; } = null!;
        public DbSet<UptimeCheck> UptimeChecks { get; set; } = null!;

        /// <summary>Keys that sign login cookies, kept in the database so redeploys don't log you out.</summary>
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EnvironmentalSetting>().HasIndex(x => x.Key).IsUnique();
            modelBuilder.Entity<UptimeCheck>().HasIndex(x => new { x.EndpointKey, x.CheckedAtUtc });
            modelBuilder.Entity<UptimeCheck>().HasIndex(x => x.CheckedAtUtc);
        }
    }
}
