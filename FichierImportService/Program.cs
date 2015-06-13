using System.ServiceProcess;
using NLog;

namespace FichierImportService
{
    static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Entree principale de l'appli
        /// </summary>
        static void Main()
        {
            Logger.Info("ImportService launch point..");
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new ImportService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
