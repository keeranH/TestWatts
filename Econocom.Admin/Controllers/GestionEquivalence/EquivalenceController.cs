using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using NLog;

namespace Econocom.Admin.Controllers.GestionEquivalence
{
    public class EquivalenceController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;
        
        public EquivalenceController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /Equivalence/

        public ActionResult Index(int? page, string sort, string sortdir)
        {
            var listeEquivalenceViewModel = new ListeEquivalenceViewModel();
            try
            {
                var pageDimension = 10;
                int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageDimension);
                var pageDimensionValue = string.IsNullOrEmpty(ConfigurationManager.AppSettings["PageSize"])
                                             ? 10
                                             : pageDimension;
                int totalPages = 1;
                var equivalences = _service.GetListeEquivalence(page, pageDimension,sort, sortdir,out totalPages);
                listeEquivalenceViewModel.Equivalences = equivalences;
                listeEquivalenceViewModel.Total = totalPages;
            }
            catch (Exception e)
            {
                Logger.Error("page={0}, sort={1}, sortdir={2}", page, sort, sortdir);
                Logger.Error(e.StackTrace);
            }
            return View(listeEquivalenceViewModel);
        }


        public ActionResult AjouteEquivalence()
        {
            try
            {
                var equivalence = new EquivalenceViewModel();
                return View(equivalence);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        [HttpPost]
        public ActionResult AjouteEquivalence(EquivalenceViewModel equivalenceViewModel)
        {
            try
            {
                var equivalence = new Equivalence();
                equivalence.LibelleEquivalence = equivalenceViewModel.LibelleEquivalence;
                equivalence.Valeur = equivalenceViewModel.Valeur;
                equivalence.Mesure = equivalenceViewModel.Mesure;
                equivalence.Afficher = equivalenceViewModel.Afficher;

                var dernierEquivalence = _service.GetDernierEquivalence();
                
                if (dernierEquivalence != null)
                {
                    equivalence.Id = equivalence.Id + 1;
                }
                else
                {
                    equivalence.Id = 1;
                }
                
                equivalence.DateDebut = DateTime.Now;

                if (equivalenceViewModel.File != null)
                {
                    if (equivalenceViewModel.File.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(equivalenceViewModel.File.FileName);
                        var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
                        equivalenceViewModel.File.SaveAs(path);
                        equivalence.LienImage = "~/uploads/" + equivalenceViewModel.File.FileName;
                    }
                }
                
                equivalence = _service.SauvegardeEquivalence(equivalence);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }


        public ActionResult ModificationEquivalence(int id)
        {
            try
            {
                var equivalence = _service.GetEquivalence(id);

                var equivalenceViewModel = new EquivalenceViewModel();
                equivalenceViewModel.Id = equivalence.Id;
                equivalenceViewModel.LibelleEquivalence = equivalence.LibelleEquivalence;
                equivalenceViewModel.Valeur = equivalence.Valeur;
                equivalenceViewModel.Mesure = equivalence.Mesure;
                equivalenceViewModel.Afficher = equivalence.Afficher;
                equivalenceViewModel.LienImage = equivalence.LienImage;

                return View(equivalenceViewModel);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        [HttpPost]
        public ActionResult ModificationEquivalence(EquivalenceViewModel equivalenceViewModel)
        {
            try
            {
                var equivalence = _service.GetEquivalence(equivalenceViewModel.Id.Value);

                equivalence.LibelleEquivalence = equivalenceViewModel.LibelleEquivalence;
                equivalence.Valeur = equivalenceViewModel.Valeur;
                equivalence.Mesure = equivalenceViewModel.Mesure;
                equivalence.Afficher = equivalenceViewModel.Afficher;
                equivalence.DateModification = DateTime.Now;

                if (equivalenceViewModel.File != null)
                {
                    if (equivalenceViewModel.File.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(equivalenceViewModel.File.FileName);
                        var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
                        equivalenceViewModel.File.SaveAs(path);
                        equivalence.LienImage = "~/uploads/" + equivalenceViewModel.File.FileName;
                    }
                }
                else
                {
                    equivalence.LienImage = equivalenceViewModel.LienImage;
                }

                equivalence = _service.MAJEquivalence(equivalence);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ActionResult SupprimerEquivalence(int id)
        {
            try
            {
                var equivalence = _service.GetEquivalence(id);
                equivalence.DateFin = DateTime.Now;
                equivalence = _service.MAJEquivalence(equivalence);
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
