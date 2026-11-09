using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjectMonitor.Domain.Data
{
    /// <summary>Lets `dotnet ef migrations add` generate PostgreSQL migrations without a running app or database.</summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MonitorDbContext>
    {
        public MonitorDbContext CreateDbContext(string[] args)
        {
            DbContextOptions<MonitorDbContext> options = new DbContextOptionsBuilder<MonitorDbContext>()
                .UseNpgsql("Host=localhost;Database=projectmonitor;Username=postgres;Password=postgres")
                .Options;

            return new MonitorDbContext(options);
        }
    }
}
