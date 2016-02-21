using Mocha.Common.Unity;
using Mocha.Refs.Web.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web
{
    public class ScheduleConfig
    {
        public static void Config()
        {
            var schedulerFactory = new StdSchedulerFactory();

            var scheduler = schedulerFactory.GetScheduler();

            ScheduleIndexBuild(scheduler);
            ScheduleUpdateTagStatistics(scheduler);

            scheduler.Start();
        }

        private static void ScheduleIndexBuild(IScheduler scheduler)
        {
            var taskKey = GetJobKey(typeof(IndexBuildJob));
            var task = JobBuilder.Create<IndexBuildJob>()
                .WithIdentity(taskKey)
                .Build();

            var triggerKey = GetTriggerKey(typeof(IndexBuildJob));
#if DEBUG
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .WithSimpleSchedule(builder => builder.WithIntervalInMinutes(60).RepeatForever())
                .StartNow()
                .Build();
#else
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .WithCronSchedule("0 0 3 * * ?")
                .StartNow()
                .Build();
#endif
            scheduler.ScheduleJob(task, trigger);
        }

        private static void ScheduleUpdateTagStatistics(IScheduler scheduler)
        {
            var taskKey = GetJobKey(typeof(UpdateTagStatisticsJob));
            var task = JobBuilder.Create<UpdateTagStatisticsJob>()
                .WithIdentity(taskKey)
                .Build();

            var triggerKey = GetTriggerKey(typeof(UpdateTagStatisticsJob));
#if DEBUG
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .WithSimpleSchedule(builder => builder.WithIntervalInMinutes(60).RepeatForever())
                .StartNow()
                .Build();
#else
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .WithCronSchedule("0 15 2 * * ?")
                .StartNow()
                .Build();
#endif
            scheduler.ScheduleJob(task, trigger);
        }

        private static JobKey GetJobKey(Type type)
        {
            var key = string.Format("{0}_Key", type.FullName);
            return new JobKey(key);
        }

        private static TriggerKey GetTriggerKey(Type type)
        {
            var key = string.Format("{0}_Trigger", type.FullName);
            return new TriggerKey(key);
        }

    }
}