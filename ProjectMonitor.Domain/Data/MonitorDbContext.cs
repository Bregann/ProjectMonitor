using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProjectMonitor.Domain.Data
{
    public class MonitorDbContext(DbContextOptions<MonitorDbContext> options) : DbContext(options), IDataProtectionKeyContext
    {
        public DbSet<UptimeCheck> UptimeChecks => Set<UptimeCheck>();

        /// <summary>Keys that sign login cookies, kept in the database so redeploys don't log you out.</summary>
        public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UptimeCheck>(entity =>
            {
                entity.Property(x => x.EndpointKey).HasMaxLength(200);
                entity.Property(x => x.Error).HasMaxLength(500);
                entity.HasIndex(x => new { x.EndpointKey, x.CheckedAtUtc });
                entity.HasIndex(x => x.CheckedAtUtc);
            });
        }
    }
}
