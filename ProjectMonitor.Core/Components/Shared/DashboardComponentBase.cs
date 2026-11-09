using Microsoft.AspNetCore.Components;
using ProjectMonitor.Domain.Interfaces.Api.Dashboard;

namespace ProjectMonitor.Core.Components.Shared
{
    /// <summary>
    /// Base for pages that show live data: reloads from <see cref="IDashboardService"/> whenever new data arrives,
    /// and every 30 seconds so "5 min ago" style times stay accurate.
    /// </summary>
    public abstract class DashboardComponentBase : ComponentBase, IDisposable
    {
        private Timer? _clock;

        [Inject]
        protected IDashboardService Dashboard { get; set; } = default!;

        protected DateTime NowUtc { get; private set; } = DateTime.UtcNow;

        /// <summary>Load the page's data from <see cref="Dashboard"/>. Called on first load and on every refresh.</summary>
        protected abstract void LoadData();

        protected override void OnInitialized()
        {
            LoadData();
            Dashboard.DataChanged += Refresh;
            _clock = new Timer(_ => Refresh(), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        private void Refresh()
        {
            _ = InvokeAsync(() =>
            {
                NowUtc = DateTime.UtcNow;
                LoadData();
                StateHasChanged();
            });
        }

        public void Dispose()
        {
            Dashboard.DataChanged -= Refresh;
            _clock?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
