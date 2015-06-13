using System;
using System.Configuration;
using System.IO;
using System.Linq;
using NLog;
using Quartz;

namespace FichierImportService
{
    public class ImportJob : IJob
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private void PrintConfigSettings()
        {
            try
            {
                Logger.Info("LienFichiers: " + ServiceUtility.ConfigFichiers);
                Logger.Info("LienInitial: " + ServiceUtility.ConfigInitial);
                Logger.Info("LienTraitement: " + ServiceUtility.ConfigEnTraitement);
                Logger.Info("LienImportWatt: " + ServiceUtility.ConfigImportWatt);
                Logger.Info("LienFichierTraiteOk: " + ServiceUtility.ConfigFichierTraiteOk);
                Logger.Info("LienFichierTraiteKo: " + ServiceUtility.ConfigFichierTraiteKo);
                Logger.Info("ConnectionString: " + ConfigurationManager.ConnectionStrings["EconocomContext"]);
                Logger.Info("ConnectionString: " + ServiceUtility.ConfigBdd);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString);
                throw;
            }
        }

        public void Execute(IJobExecutionContext context)
        {
            PrintConfigSettings();
            Logger.Info("ImportJob..executes starts");
            try
            {
                //Récuperation des fichiers
                MoveFiles();
            }
            catch (Exception e)
            {

                Logger.Error(e.StackTrace);
                Logger.Info("Fin avec erreur: changement annee calendaire recalculation.." + DateTime.Now);
            }
            Logger.Info("ImportJob..executes ends");
        }

        /// <summary>
        /// Récuperation des fichiers 
        /// </summary>
        public void MoveFiles()
        {
            Logger.Info("ImportJob..Démarrage récuperation des fichiers");
            string[] files = Directory.GetFiles(ServiceUtility.ConfigFichiers);

            //Si aucun traitement est en cours et il existe des fichiers dans le repertoire
            if (files.Any() && !ServiceUtility.FlagInProgress)
            {              
                string name = Path.GetFileName(files[0]);
                Logger.Info("ImportJob..file name = " + name);
                string dest = Path.Combine(ServiceUtility.ConfigInitial, name);
                File.Move(files[0], dest);
                Logger.Info("Deplacement fichier " + name);
            }

            Logger.Info("ImportJob..Fin récuperation des fichiers");
        }
    }
}
