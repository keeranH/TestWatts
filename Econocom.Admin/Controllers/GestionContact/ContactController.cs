using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Model.Enum;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using Econocom.Model.ViewModel.CMS;
using NLog;
using WebGrease.Css.Extensions;


namespace Econocom.Admin.Controllers.GestionContact
{
    [Authorize]
    public class ContactController : BaseController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;
        public static int PageSize = 10;

        public ContactController()
        {
            _service = new ServiceApiController();
            PageSize = 10;
        }

        //
        // GET: /Admin/Contact/
        public ActionResult Index()
        {
            try
            {
                var administrateur = _service.GetAdministrateur();
                var commercial = _service.GetCommercial();

                var listeUtilisateur = new ListeUtilisateurViewModel();
                listeUtilisateur.Administrateur = administrateur;
                listeUtilisateur.Commercial = commercial;

                return View(listeUtilisateur);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        [HttpPost]
        public ActionResult Index(ListeUtilisateurViewModel listeUtilisateurViewModel)
        {
           try
           {
               _service.SetListeUtilisateur(listeUtilisateurViewModel);
           }
           catch (Exception e)
           {
               throw e;
           }
           return View(listeUtilisateurViewModel);
        }
        
    
        public ActionResult Liste(int? id, string search, int? page, string sort, string sortdir)
        {
            var sameController = Econocom.Helper.Url.UrlHelper.ReferrerController(Request.UrlReferrer, HttpContext.Request.RequestContext.RouteData);

            if (!sameController)
                Session["search"] = null;

            var liste = new List<ContactViewModel>();
            try
            {
                if (search != null)
                    Session["search"] = search;
                else
                {
                    if (Session["search"] != null)
                        search = Session["search"].ToString();
                }

                var pageDimension = 10;
                int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageDimension);

                int totalPages = 1;
               
                liste = _service.ListeContact(id, search, page, sort, sortdir, pageDimension, out totalPages);
                ViewBag.Total = totalPages;
            }
            catch (Exception e)
            {
                Logger.Error("id={0}, recherche={1}, page={2}, sort={3}, sortdir={4}", id, search, page, sort, sortdir);
                Logger.Error(e.StackTrace);
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("Utilisateurs", liste);
            }
            else
                return View(liste);
        }

        public ActionResult Mail(int? page)
        {
            try
            {
                var pageDimension = 10;

                int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageDimension);
                var pageDimensionValue = string.IsNullOrEmpty(ConfigurationManager.AppSettings["PageSize"])
                                             ? 10
                                             : pageDimension;

                int totalPages = 1;
                var liste = _service.ListeContact(null, null, page, null, null, pageDimension, out totalPages);
                ViewBag.Total = totalPages;

                return View(liste);
            }
            catch (Exception e)
            {                    
                throw e;
            }
        }

        public FileResult ExportMail()
        {
            try
            {
                var liste = _service.GetListeContact();

                if (liste != null && liste.Any())
                {
                    var objet = liste.First();
                    var type = objet.GetType();

                    using (var stream = new MemoryStream())
                    {
                        using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
                        {
                            var csvWriter1 = new CsvWriter(streamWriter);
                            csvWriter1.Configuration.Delimiter = ";";
                            const string nomFichier = "Export_{0}_{1}.csv";
                            var typeObjet = type.ToString();
                            const string nomObjet = "ListeMail";
                            var date = DateTime.Now.ToString("yyyyMMddHHmmss");
                            var nomFichierFinale = String.Format(nomFichier, nomObjet, date);

                            csvWriter1.WriteHeader(typeof(ContactViewModel));
                            liste.Cast<ContactViewModel>().ForEach(csvWriter1.WriteRecord);

                            var listeExport = new List<ContactViewModel>();
                            //listeExport.AddRange(liste);
                            //listeExport.ForEach(csvWriter1.WriteRecord);

                            streamWriter.Flush();
                            return File(new MemoryStream(stream.ToArray()), "text/csv", nomFichierFinale);
                        }
                    }
                }
            }
            catch (Exception)
            {                
                throw;
            }
            return null;
        }        

    }
}
