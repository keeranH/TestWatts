using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using NLog;
using Quartz;
using Quartz.Impl;

namespace ECSChange
{
    public class TaskScheduler : ITaskScheduler
    {
        private IScheduler _scheduler;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public string Name
        {
            get { return GetType().Name; }
        }

        public void Run()
        {
            Logger.Error("Task Scheduler.. starts");
            try
            {
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                _scheduler = schedulerFactory.GetScheduler();

                string taskGroup = "Test Group";
                string taskName = "Test Name";

                IJobDetail testJob = JobBuilder.Create<ECSChangeJob>()
                                               .WithIdentity(taskName, taskGroup)
                                               .Build();

                ITrigger testTrigger = TriggerBuilder.Create()
                                                     .WithIdentity(taskName, taskGroup)
                                                     .StartNow()
                                                     .WithCronSchedule(ConfigurationManager.AppSettings["Interval"])
                                                     .Build();

                var dictionary = new Dictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>>();

                dictionary.Add(testJob, new Quartz.Collection.HashSet<ITrigger>()
                    {
                        testTrigger
                    });


                _scheduler.ScheduleJobs(dictionary, false);
                _scheduler.Start();
            }
            catch (Exception e)
            {
                Logger.Error(e.InnerException.ToString());
            }
            Logger.Error("Task Scheduler.. ends");
        }

        public void Stop()
        {
            _scheduler.Shutdown();
        }
    }
}
