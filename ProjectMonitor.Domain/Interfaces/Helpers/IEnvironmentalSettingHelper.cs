using ProjectMonitor.Domain.Enums;

namespace ProjectMonitor.Domain.Interfaces.Helpers
{
    public interface IEnvironmentalSettingHelper
    {
        Task LoadEnvironmentalSettings();
        string? TryGetEnvironmentalSettingValue(EnvironmentalSettingEnum key);
    }
}
