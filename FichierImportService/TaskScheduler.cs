using System;
using System.Collections.Generic;
using System.Configuration;
using NLog;
using Quartz;
using Quartz.Impl;

namespace FichierImportService
{
    public class TaskScheduler:ITaskScheduler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IScheduler _scheduler;
        public string Name
        {
            get { return GetType().Name; }
        }

        public void Run()
        {
            try
            {
                Logger.Info("TaskScheduler starts");
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                _scheduler = schedulerFactory.GetScheduler();

                string taskGroup = "Test Group";
                string taskName = "Test Name";

                IJobDetail testJob = JobBuilder.Create<ImportJob>()
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
                Logger.Info("TaskScheduler ends");
            }
            catch (Exception e)
            {
                Logger.Error((e.ToString()));
            }
        }

        public void Stop()
        {
            _scheduler.Shutdown();
        }
    }
}
