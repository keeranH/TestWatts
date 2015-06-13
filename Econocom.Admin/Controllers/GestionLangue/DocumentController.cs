using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Model;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.Service;
using Econocom.Model.ViewModel;
using NLog;

namespace Econocom.Admin.Controllers.GestionLangue
{
    [Authorize]
    public class DocumentController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static int PageSize = 10;

        //
        // GET: /Document/

        public ActionResult Index()
        {
           
            return View("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(DocumentViewModel dvm)
        {

            UploadController uploadApi= new UploadController();

            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file];
                if (hpf != null)
                {
                    if (hpf.ContentLength > 0)
                    {
                        try
                        {
                            var fileName = Path.GetFileName(hpf.FileName);
                            var fileLen = hpf.ContentLength;
                            var input = new byte[fileLen];

                            // Initialize the stream.
                            var inputStream = hpf.InputStream;

                            // Read the file into the byte array.
                            inputStream.Read(input, 0, fileLen);

                            var doc = new DocumentUpload { Data = input, DocumentName = fileName };
                            var path = uploadApi.SauvegardeFichierPhysique(doc);
                            var statut = (path != null);
                            Document document = new Document();
                            document.Nom = dvm.Nom;
                            document.Repertoire = "~/uploads/"+fileName;
                            document.DateCreation = DateTime.Now;
                            

                            uploadApi.SauvegarderDocument(document);
                            
                            if (statut)
                            {
                                ViewBag.Success = Resource.Traduction.ChargementFichier_Succes;
                            }
                            else
                            {
                                ViewBag.Error = Resource.Traduction.ChargementFichier_Echec;
                            }

                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = Resource.Traduction.Selection_Fichier + ", Erreur: " + e.Message;
                            Logger.Error("Message: " + e.Message + ", " + e.StackTrace);
                        }
                    }
                }
                else
                {
                    ViewBag.Error = Resource.Traduction.Selection_Fichier;
                }
            }

           

            //var docName = formCollection["Nom_De_Document"];
            //var file = formCollection["file"];
            return RedirectToAction("Index", "Document");
        }

        public ActionResult GetTableDeDocument(int? page, string sort, string sortdir)
        {
            ServiceApiController serviceApi= new ServiceApiController();

            var liste = new List<Document>();
            var model = new DocumentViewModel();
            try
            {
                var pageDimension = 10;
                int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageDimension);
                var pageDimensionValue = string.IsNullOrEmpty(ConfigurationManager.AppSettings["PageSize"])
                                             ? 10
                                             : pageDimension;
                int totalPages = 1;

                liste = serviceApi.GetListDocument(page, pageDimension, out totalPages);
                model.ListeDocuments = liste;
                ViewBag.Total = totalPages;

                return PartialView("AffichageTableDocument", model);
            }
            catch (Exception e)
            {
                return null;

            }

        }

        public ActionResult Supprimer(int docId)
        {
            try
            {
                var serviceApi = new ServiceApiController();
                Document document = serviceApi.GetDocument(docId);
                string fileName = document.Repertoire;
                bool supprimerDocument = serviceApi.SupprimerDocument(docId);
                if (supprimerDocument)
                {
                    var controller = new UploadController();
                    controller.SupprimerFichierPhysique(fileName);
                }                
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return RedirectToAction("Index", "Document");
        }

        public ActionResult Telecharger(int docId)
        {
            try
            {
                ServiceApiController serviceApi = new ServiceApiController();
                Document document = serviceApi.GetDocument(docId);
                if (document != null)
                {
                    var extension = document.Repertoire.Split('.').Last();
                    return new FilePathResult(document.Repertoire, extension);
                }               
            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }
    }
}
