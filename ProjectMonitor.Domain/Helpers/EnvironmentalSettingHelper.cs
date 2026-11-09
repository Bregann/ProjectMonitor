using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectMonitor.Domain.Database.Context;
using ProjectMonitor.Domain.Enums;
using ProjectMonitor.Domain.Interfaces.Helpers;
using Serilog;

namespace ProjectMonitor.Domain.Helpers
{
    public class EnvironmentalSettingHelper(IServiceProvider serviceProvider) : IEnvironmentalSettingHelper
    {
        private Dictionary<EnvironmentalSettingEnum, string> _environmentalSettings = [];

        public async Task LoadEnvironmentalSettings()
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var settings = await context.EnvironmentalSettings.ToListAsync();

            _environmentalSettings = settings
                .Where(x => Enum.TryParse<EnvironmentalSettingEnum>(x.Key, out _))
                .ToDictionary(x => Enum.Parse<EnvironmentalSettingEnum>(x.Key), x => x.Value);

            Log.Information("Loaded {Count} environmental settings", _environmentalSettings.Count);
        }

        public string? TryGetEnvironmentalSettingValue(EnvironmentalSettingEnum key)
        {
            return _environmentalSettings.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value) ? value : null;
        }
    }
}
