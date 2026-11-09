using ProjectMonitor.Domain.Database.Models;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Enums;

namespace ProjectMonitor.Domain.Interfaces.Helpers
{
    /// <summary>
    /// Turns one check into a status. One implementation per check type, registered in Program.cs;
    /// ChecksService picks the evaluator whose <see cref="CheckType"/> matches. Website/API checks are
    /// handled by the uptime job instead, because they actively send requests.
    /// </summary>
    public interface ICheckEvaluator
    {
        CheckTypeEnum CheckType { get; }

        CheckStatus Evaluate(ProjectCheck check, CheckEvaluationContext context);
    }
}
