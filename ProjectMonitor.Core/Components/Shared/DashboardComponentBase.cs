using Microsoft.AspNetCore.Components;
using ProjectMonitor.Domain.State;

namespace ProjectMonitor.Core.Components.Shared
{
    /// <summary>
    /// Base for pages that show live data: re-renders whenever the background jobs publish new data,
    /// and every 30 seconds so "5 min ago" style times stay accurate.
    /// </summary>
    public abstract class DashboardComponentBase : ComponentBase, IDisposable
    {
        private Timer? _clock;

        [Inject]
        protected DashboardState State { get; set; } = default!;

        protected DateTime NowUtc { get; private set; } = DateTime.UtcNow;

        /// <summary>Called before each live refresh, and once on first load. Recalculate view data here.</summary>
        protected virtual void OnDataChanged()
        {
        }

        protected override void OnInitialized()
        {
            OnDataChanged();
            State.Changed += OnStateChanged;
            _clock = new Timer(_ => OnStateChanged(), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        private void OnStateChanged()
        {
            _ = InvokeAsync(() =>
            {
                NowUtc = DateTime.UtcNow;
                OnDataChanged();
                StateHasChanged();
            });
        }

        public void Dispose()
        {
            State.Changed -= OnStateChanged;
            _clock?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
