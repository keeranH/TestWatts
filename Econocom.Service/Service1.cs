using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using log4net;

namespace Econocom.Service
{
    public partial class Service1 : ServiceBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Service1));
        ITaskScheduler scheduler;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.RequestAdditionalTime(120000);
           logger.Info("test");
            scheduler = new TaskScheduler();
            scheduler.Run();
        }

        protected override void OnStop()
        {
            if (scheduler != null)
            {
                scheduler.Stop();
            }
        }
    }
}
