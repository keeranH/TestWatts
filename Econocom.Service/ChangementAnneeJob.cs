using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using Econocom.Business.Service;
using Econocom.Data;
using NLog;
using Quartz;
using log4net;
using LogManager = log4net.LogManager;

namespace Econocom.Service
{
    [DisallowConcurrentExecution]
    public class ChangementAnneeJob : IJob
    {
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public virtual void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info("Debut changement annee calendaire recalculation.." + DateTime.Now);
                var connectionString = ConfigurationManager.ConnectionStrings["EconocomContext"];
                var econocomContext = new EconocomContext(connectionString.ToString());
                var econocomData = new EconocomDataManager(econocomContext);
                var businessService = new BusinessService(econocomData);
                var resultat = businessService.MajConfigSiPourNouvelleAnnee(10);
                Logger.Info("resultat=" + resultat);
                Logger.Info("Fin changement annee calendaire recalculation.." + DateTime.Now);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                Logger.Info("Fin avec erreur: changement annee calendaire recalculation.." + DateTime.Now);
            }
        }

    }
}
