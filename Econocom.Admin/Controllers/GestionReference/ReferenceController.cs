using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.Service;
using Econocom.Model.ViewModel;
using NLog;

namespace Econocom.Admin.Controllers.GestionReference
{
    [Authorize]
    public class ReferenceController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public ReferenceController()
        {
            _service = new ServiceApiController();
        }
        //
        // GET: /Reference/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            var uploadApi = new UploadController();

            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    try
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var fileLen = file.ContentLength;
                        var input = new byte[fileLen];

                        // Initialize the stream.
                        var inputStream = file.InputStream;

                        // Read the file into the byte array.
                        inputStream.Read(input, 0, fileLen);

                        var document = new DocumentUpload { Data = input, DocumentName = fileName };
                        var objetsEnErreur = uploadApi.PostFile(document);
                        if (objetsEnErreur == null)
                        {
                            var message = String.Format("{0}: {1} {2}", Resource.Traduction.ChargementFichier_Echec,
                                                           Resource.Traduction.FichierInvalide,
                                                           Resource.Traduction.Erreur_VerifierLog);
                            ViewBag.Error = message;
                        }
                        else
                        {
                            var statut = (objetsEnErreur.Count == 0 ? true : false);
                            if (statut)
                            {
                                ViewBag.Success = Resource.Traduction.ChargementFichier_Succes;
                            }
                            else
                            {
                                var message = String.Format("{0} ({1}): {2} {3}", Resource.Traduction.ChargementFichier_Echec,Resource.Traduction.Erreur_Donnees,
                                                            Resource.Traduction.Erreur_Doublons,
                                                            Resource.Traduction.Erreur_VerifierLog);
                                ViewBag.Error = message;
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        ViewBag.Error = Resource.Traduction.Selection_Fichier+ ", Erreur: "+e.Message;
                        Logger.Error("Message: "+e.Message +", "+e.StackTrace);
                    }
                }
            }
            else
            {
                ViewBag.Error = Resource.Traduction.Selection_Fichier;
            }

            return View();
        }


        public ActionResult GetTablesDeReference()
        {
            var liste = new List<TypeObjet>();
            var model = new ReferenceUnitaireViewModel();
            try
            {
                liste = _service.GetListeTypeObjet();
                model.TypeObjets = liste;
                return PartialView("AffichageTablesDeReference", model);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return null;
                
            }
            
        }
    }
}
