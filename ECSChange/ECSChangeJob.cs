using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using Quartz;

namespace ECSChange
{
    public class ECSChangeJob : IJob
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["LienFichiers"]);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);

                    // ADD Unique File Name Check to Below!!!!
                    string dest = Path.Combine(ConfigurationManager.AppSettings["LienInitial"], name);
                    File.Move(file, dest);
                    Logger.Info("Deplacement fichier " + name);
                }
            }
            catch (Exception e)
            {

                Logger.Error(e.StackTrace);
                Logger.Info("Fin avec erreur: changement annee calendaire recalculation.." + DateTime.Now);
            }
        }
    }
}
