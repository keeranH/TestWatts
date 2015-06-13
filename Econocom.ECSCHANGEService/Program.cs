using System.ServiceProcess;

namespace Econocom.ECSCHANGEService
{
    static class Program
    {
        /// <summary>
        /// L'entrée principale de l'application
        /// </summary>
        static void Main()
        {

            ServiceBase[] ServicesToRun =
            { 
                new ECSCHANGEService() 
            };
            ServiceBase.Run(ServicesToRun);          
        }
    }
}
