using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using NLog;
using Quartz;
using Quartz.Impl;
using log4net;
using log4net.Config;

[assembly: log4net.Config.XmlConfigurator]
namespace Econocom.Service
{
    public class TaskScheduler:ITaskScheduler
    {
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private IScheduler _scheduler;
        static TaskScheduler()
    {
    }

        public string Name
        {
            get { return GetType().Name; }
        }

        public void Run()
        {
           
            //NameValueCollection properties = new NameValueCollection();
            //properties["quartz.scheduler.instanceName"] = "MyTaskScheduler";

            //properties["quartz.plugin.xml.type"] = "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz";

            //string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //properties["quartz.plugin.xml.fileNames"] = path + @"\Tasks.xml";

            //ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler();
            

            //System.Diagnostics.Debugger.Launch(); // attach debugger to the process   
            Logger.Info("starting scheduler.." + DateTime.Now);
            _scheduler.Start();

            //IJobDetail jobDetail = JobBuilder.Create<ChangementAnneeJob>()
            //.WithIdentity("simpleJob", "simpleJobs")
            //.RequestRecovery()
            //.Build();

           // ITrigger trigger = TriggerBuilder.Create()
                //.WithIdentity("simpleTrigger", "simpleTriggers")
               // .StartNow()
                //.WithSimpleSchedule(x => x.WithRepeatCount(1).WithIntervalInSeconds(10))
               // .Build();

          //  _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public void Stop()
        {
            _scheduler.Shutdown();
        }
    }
}
