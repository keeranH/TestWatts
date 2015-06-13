using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using NLog;

namespace Econocom.Admin.Controllers.GestionPolitique
{
    public class PolitiqueController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public PolitiqueController()
        {
            _service = new ServiceApiController();
        }
        
        //
        // GET: /Politique/

        public ActionResult Index(int? page, string sort, string sortdir)
        {
            var listePolitiques = new List<Politique>();
            try
            {
                var pageDimension = 10;
                int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageDimension);
                var pageDimensionValue = string.IsNullOrEmpty(ConfigurationManager.AppSettings["PageSize"])
                                             ? 10
                                             : pageDimension;
                int totalPages = 1;
                listePolitiques = _service.GetListePolitique(page, pageDimension, sort, sortdir, out totalPages);
                ViewBag.Total = totalPages;
            }
            catch (Exception e)
            {
                Logger.Error("page={0}, sort={1}, sortdir={2}", page, sort, sortdir);
                Logger.Error(e.StackTrace);
            }

            return View(listePolitiques);
        }

        public ActionResult AjoutePolitique()
        {
            try
            {
                var politique = new Politique();
                return View(politique);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        [HttpPost]
        public ActionResult AjoutePolitique(Politique politique)
        {
            try
            {
                var dernierPolitique = _service.GetDernierPolitique();
                if (dernierPolitique != null)
                {
                    politique.Id = dernierPolitique.Id + 1;
                }
                else
                {
                    politique.Id = 1;
                }
                politique.DateDebut = DateTime.Now;
                politique.Simulations = null;
                politique = _service.SauvegardePolitique(politique);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ActionResult ModificationPolitique(int id)
        {
            try
            {
                var politique = _service.GetPolitique(id);
                return View(politique);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        [HttpPost]
        public ActionResult ModificationPolitique(Politique politique)
        {
            try
            {
                var politique1 = _service.GetPolitique(politique.Id);
                politique1.LibellePolitique = politique.LibellePolitique;
                politique1.Device = politique.Device;
                politique1.Objectif = politique.Objectif;
                politique1.Benefice = politique.Benefice;
                politique1.DateModification = DateTime.Now;

                politique1 = _service.MAJPolitique(politique1);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }
        
        public ActionResult SupprimerPolitique(int id)
        {
            try
            {
                var politique = _service.GetPolitique(id);
                politique.DateFin = DateTime.Now;
                politique = _service.MAJPolitique(politique);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

    }
}
