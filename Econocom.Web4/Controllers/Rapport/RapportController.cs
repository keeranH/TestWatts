using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using CsvHelper.Configuration;
using Econocom.Helper.Rapport;
using Econocom.Model.ViewModel.Report;
using Econocom.Resource;
using Econocom.Web4.Controllers.ApiControllers;
using NLog;

namespace Econocom.Web4.Controllers.Rapport
{
    [Authorize]
    public class RapportController : BaseController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ServiceApiController _serviceApi;
        public static int PageSize = 10;
        public RapportController()
        {
            _serviceApi = new ServiceApiController();
            if (Session != null)
                Session["search"] = null;
            PageSize = 10;
        }


       [OutputCache(Duration = 0)] 
        public ActionResult Simulations(int? id, string search, int? page, string sort, string sortdir)
        {
            var rapportViewModel = new RapportViewModel();
            try
            {
                if (id == null)
                {
                    if (Session["rapportclientid"] != null)
                        id = (int)Session["rapportclientid"];                    
                }
                else
                {
                    Session["rapportclientid"] = id;                        
                }

                if (search != null)
                    Session["search"] = search;
                else
                {
                    if (Session["search"] != null)
                        search = Session["search"].ToString();
                }

                var pageDimension = 10;
                int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageDimension);
                var pageDimensionValue = string.IsNullOrEmpty(ConfigurationManager.AppSettings["PageSize"])
                                             ? 10
                                             : pageDimension;
                int totalPages = 1;
                var rapports = _serviceApi.GetRapportSimulation(id, search, page, sort, sortdir, pageDimension,
                                                                out totalPages);                
                rapportViewModel.Rapports = rapports;
                rapportViewModel.Total = totalPages;
            }
            catch (Exception e)
            {
                Logger.Error("id={0}, recherche={1}, page={2}, sort={3}, sortdir={4}", id, search, page, sort, sortdir);
                Logger.Error(e.StackTrace);
            }

            if (Request.IsAjaxRequest())
            {                
                return PartialView("RapportSimulation", rapportViewModel);
            }
            else
                return View(rapportViewModel);
        }

       [OutputCache(Duration = 0)] 
        public ActionResult Personalisation(int? id, string search, int? page, string sort, string sortdir)
        {
            var rapportViewModel = new RapportViewModel();
            try
            {
                if (id == null)
                {
                    if (Session["rapportclientid"] != null)
                        id = (int)Session["rapportclientid"];
                }
                else
                {
                    Session["rapportclientid"] = id;
                }

                if (search != null)
                    Session["search"] = search;
                else
                {
                    if (Session["search"] != null)
                        search = Session["search"].ToString();
                }
                var pageDimension = 10;
                int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageDimension);
                var pageDimensionValue = string.IsNullOrEmpty(ConfigurationManager.AppSettings["PageSize"])
                                             ? 10
                                             : pageDimension;
                int totalPages = 1;
                var rapports = _serviceApi.GetRapportPersonnalisation(id, search, page, sort, sortdir, pageDimension,
                                                                out totalPages);
                rapportViewModel.Rapports = rapports;
                rapportViewModel.Total = totalPages;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("Rapports", rapportViewModel);
            }
            else
                return View(rapportViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OutputCache(Duration = 0)] 
        public FileResult GetRapportPersonnalisation(int id)
        {
            try
            {
                var rapport = _serviceApi.GetRapport(id);

                if (rapport != null)
                {
                    var liste = rapport.ConfigSi.ConfigSiDevices.ToList();
                    var rapportHelper = new RapportHelper();
                    var result = rapportHelper.GetRapportCsv(rapport);

                    var contact = _serviceApi.GetDetailsContact(rapport.ContactId.Value);
                    result.Personnalisation = rapportHelper.MAJPrix(result.Personnalisation, contact);

                    if (result != null)
                    {
                        using (var stream = new MemoryStream())
                        {
                            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
                            {
                                streamWriter.WriteLine(Econocom.Resource.Traduction.TitreRapportMaPersonnalisation);
                                streamWriter.WriteLine(Econocom.Resource.Traduction.Date+";"+result.DateRapport);
                                streamWriter.WriteLine(Econocom.Resource.Traduction.NomDuRapport + ";" + result.NomRapport);

                                var csvWriter1 = new CsvWriter(streamWriter);
                                var map = csvWriter1.Configuration.AutoMap<RapportConfigViewModel>();
                                var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                                map = rapportHelper.TraductionHeader(map);
                                
                                csvWriter1.Configuration.RegisterClassMap(map);
                                csvWriter1.Configuration.Delimiter = ";";
                                const string nomFichier = "{0}_{1}_{2}.csv";
                                var date = DateTime.Now.ToString("yyyyMMddHHmmss");
                                var nomFichierFinale = String.Format(nomFichier, result.TitreRapport.Replace(" ", "-"),
                                                                     result.NomRapport.Replace(" ", "-"), date);
                               
                                csvWriter1.WriteHeader(typeof (RapportConfigViewModel));
                                result.Personnalisation.ForEach(csvWriter1.WriteRecord);

                                streamWriter.Flush();
                                return File(new MemoryStream(stream.ToArray()), "text/csv", nomFichierFinale);
                            }
                        }
                    }
                }
                else
                    Logger.Info("rapport id={0} is null", id);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
           
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OutputCache(Duration = 0)] 
        public FileResult GetRapportSimulation(int id)
        {
            try
            {
                var simulation = _serviceApi.GetSimulationParIdRapport(id);

                if (simulation != null)
                {
                    var rapportHelper = new RapportHelper();

                    var result = rapportHelper.GetRapportSimulationCsv(simulation);
                    
                    var contact = _serviceApi.GetDetailsContact(simulation.ContactId.Value);
                    result.Personnalisation = rapportHelper.MAJPrix(result.Personnalisation, contact);
                    result.Simulation = rapportHelper.MAJPrix(result.Simulation, contact);
                    result.Comparaison = rapportHelper.MAJPrix(result.Comparaison, contact);
                    
                    if (result != null)
                    {
                        using (var stream = new MemoryStream())
                        {
                            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
                            {
                                streamWriter.WriteLine(Econocom.Resource.Traduction.TitreRapportMaSimulation);
                                streamWriter.WriteLine(Econocom.Resource.Traduction.Date + ";" + result.DateRapport);
                                streamWriter.WriteLine(Econocom.Resource.Traduction.NomDuRapport + ";" + result.NomRapport);

                                streamWriter.WriteLine(Econocom.Resource.Traduction.MonSIAvantSimulation);

                                var csvWriter1 = new CsvWriter(streamWriter);
                                var map = csvWriter1.Configuration.AutoMap<RapportConfigViewModel>();
                                var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                                map = rapportHelper.TraductionHeader(map);

                                csvWriter1.Configuration.RegisterClassMap(map);
                                csvWriter1.Configuration.Delimiter = ";";
                                const string nomFichier = "{0}_{1}_{2}.csv";
                                var date = DateTime.Now.ToString("yyyyMMddHHmmss");
                                var nomFichierFinale = String.Format(nomFichier, result.TitreRapport.Replace(" ", "-"),
                                                                     result.NomRapport.Replace(" ", "-"), date);

                                csvWriter1.WriteHeader(typeof(RapportConfigViewModel));
                                result.Personnalisation.ForEach(csvWriter1.WriteRecord);

                                streamWriter.WriteLine(Econocom.Resource.Traduction.MonSIApresSimulation);
                                var csvWriter = new CsvWriter(streamWriter);
                                csvWriter.Configuration.RegisterClassMap(map);
                                csvWriter.Configuration.Delimiter = ";";
                                csvWriter.WriteHeader(typeof(RapportConfigViewModel));
                                result.Simulation.ForEach(csvWriter.WriteRecord);

                                
                                streamWriter.WriteLine(Econocom.Resource.Traduction.AnalyseDesEcarts);
                                var csvWriterEcart = new CsvWriter(streamWriter);
                                csvWriterEcart.Configuration.RegisterClassMap(map);
                                csvWriterEcart.Configuration.Delimiter = ";";
                                csvWriterEcart.WriteHeader(typeof(RapportConfigViewModel));
                                result.Comparaison.ForEach(csvWriterEcart.WriteRecord);
                                
                                streamWriter.Flush();
                                return File(new MemoryStream(stream.ToArray()), "text/csv", nomFichierFinale);
                            }
                        }
                    }
                }
                else
                    Logger.Info("rapport id={0} is null", id);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }

            return null;
        }

        public ActionResult Print(int id)
        {
            try
            {

                var simulation = _serviceApi.GetSimulationParIdRapport(id);

                if (simulation != null)
                {
                    var rapportHelper = new RapportHelper();
                    var result = rapportHelper.GetRapportSimulationCsv(simulation);
                    if (result != null)
                    {
                        return View(result);
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return View();
        }

    }
}
