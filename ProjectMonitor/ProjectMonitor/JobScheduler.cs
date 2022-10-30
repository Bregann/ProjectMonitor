using ProjectMonitor.Api.Data;
using Quartz;
using Quartz.Impl;

namespace ProjectMonitor.Api
{
    public class JobScheduler
    {
        private static StdSchedulerFactory _factory;
        private static IScheduler _scheduler;

        public static async Task SetupJobScheduler()
        {
            //construct the scheduler factory
            _factory = new StdSchedulerFactory();
            _scheduler = await _factory.GetScheduler();
            await _scheduler.Start();

            //Check services up
            var checkServiceHealth = JobBuilder.Create<CheckServiceHealthJob>().WithIdentity("checkServiceHealth").Build();
            var checkServiceHealthTrigger = TriggerBuilder.Create().WithIdentity("checkServiceHealthTrigger").WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever()).Build();

            //Check for project errors
            var checkProjectErrors = JobBuilder.Create<CheckProjectErrorsJob>().WithIdentity("checkProjectErrors").Build();
            var checkProjectErrorsTrigger = TriggerBuilder.Create().WithIdentity("checkProjectErrorsTrigger").WithSimpleSchedule(x => x.WithIntervalInSeconds(15).RepeatForever()).Build();

            //Check for any email alerts to send
            var checkForEmailsToSend = JobBuilder.Create<CheckForEmailsToSend>().WithIdentity("checkForEmailsToSend").Build();
            var checkForEmailsToSendTrigger = TriggerBuilder.Create().WithIdentity("checkForEmailsToSendTrigger").WithSimpleSchedule(x => x.WithIntervalInSeconds(20).RepeatForever()).Build();

            await _scheduler.ScheduleJob(checkServiceHealth, checkServiceHealthTrigger);
            await _scheduler.ScheduleJob(checkProjectErrors, checkProjectErrorsTrigger);
            await _scheduler.ScheduleJob(checkForEmailsToSend, checkForEmailsToSendTrigger);
        }
    }

    internal class CheckServiceHealthJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            ErrorMonitoring.CheckProjectAndSystemHealth();
            return Task.CompletedTask;
        }
    }

    internal class CheckProjectErrorsJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            ErrorMonitoring.CheckForProjectErrors();
            ErrorMonitoring.CheckErrorsAndCompleteIfFixed();
            return Task.CompletedTask;
        }
    }

    internal class CheckForEmailsToSend : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await ErrorMonitoring.CheckAndEmailOutErrors();
        }
    }
}
