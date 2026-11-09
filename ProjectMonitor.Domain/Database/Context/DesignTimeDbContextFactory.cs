using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjectMonitor.Domain.Database.Context
{
    /// <summary>Lets `dotnet ef migrations add` generate PostgreSQL migrations without a running app or database.</summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql("Host=localhost;Database=projectmonitor;Username=postgres;Password=postgres")
                .Options;

            return new AppDbContext(options);
        }
    }
}
