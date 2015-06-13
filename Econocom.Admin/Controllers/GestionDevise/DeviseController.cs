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

namespace Econocom.Admin.Controllers.GestionDevise
{
    public class DeviseController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public DeviseController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /Devise/

        public ActionResult Index(int? page, string sort, string sortdir)
        {
            var listeDeviseViewModel = new ListeDeviseViewModel();
            try
            {
                var pageDimension = 10;
                int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageDimension);
                var pageDimensionValue = string.IsNullOrEmpty(ConfigurationManager.AppSettings["PageSize"])
                                             ? 10
                                             : pageDimension;
                int totalPages = 1;
                var devises = _service.GetListeDevise(page, pageDimension, sort, sortdir, out totalPages);                
                listeDeviseViewModel.Devises = devises;
                listeDeviseViewModel.Total = totalPages;
            }
            catch (Exception e)
            {
                Logger.Error("page={0}, sort={1}, sortdir={2}", page, sort, sortdir);
                Logger.Error(e.StackTrace);
            }
            return View(listeDeviseViewModel);
        }


        public ActionResult AjouteDevise()
        {
            try
            {
                var devise = new DeviseViewModel();
                return View(devise);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        [HttpPost]
        public ActionResult AjouteDevise(DeviseViewModel deviseViewModel)
        {
            try
            {
                var devise = new Devise();
                devise.Libelle = deviseViewModel.Libelle;
                devise.CodeDevise = deviseViewModel.CodeDevise;
                devise.UrlImage = deviseViewModel.UrlImage;

                var dernierDevise = _service.GetDernierDevise();

                if (dernierDevise != null)
                {
                    devise.Id = dernierDevise.Id + 1;
                }
                else
                {
                    devise.Id = 1;
                }

                devise.DateDebut = DateTime.Now;

                if (deviseViewModel.File != null)
                {
                    if (deviseViewModel.File.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(deviseViewModel.File.FileName);
                        var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
                        deviseViewModel.File.SaveAs(path);
                        devise.UrlImage = "~/uploads/" + deviseViewModel.File.FileName;
                    }
                }

                devise = _service.SauvegardeDevise(devise);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }


        public ActionResult ModificationDevise(int id)
        {
            try
            {
                var devise = _service.GetDevise(id);

                var deviseViewModel = new DeviseViewModel();
                deviseViewModel.Id = devise.Id;
                deviseViewModel.Libelle = devise.Libelle;
                deviseViewModel.CodeDevise = devise.CodeDevise;
                deviseViewModel.UrlImage = devise.UrlImage;

                return View(deviseViewModel);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        [HttpPost]
        public ActionResult ModificationDevise(DeviseViewModel deviseViewModel)
        {
            try
            {
                var devise = _service.GetDevise(deviseViewModel.Id.Value);

                devise.Id = deviseViewModel.Id.Value;
                devise.Libelle = deviseViewModel.Libelle;
                devise.CodeDevise = deviseViewModel.CodeDevise;
                devise.UrlImage = deviseViewModel.UrlImage;
                devise.DateModification = DateTime.Now;

                if (deviseViewModel.File != null)
                {
                    if (deviseViewModel.File.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(deviseViewModel.File.FileName);
                        var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
                        deviseViewModel.File.SaveAs(path);
                        devise.UrlImage = "~/uploads/" + deviseViewModel.File.FileName;
                    }
                }
                else
                {
                    devise.UrlImage = deviseViewModel.UrlImage;
                }

                devise = _service.MAJDevise(devise);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ActionResult SupprimerDevise(int id)
        {
            try
            {
                var devise = _service.GetDevise(id);
                devise.DateFin = DateTime.Now;
                devise = _service.MAJDevise(devise);
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
