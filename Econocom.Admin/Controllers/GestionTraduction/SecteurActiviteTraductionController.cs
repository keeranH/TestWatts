using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.Traduction;
using Econocom.Model.ViewModel.Traduction;
using NLog;
using Omu.ValueInjecter;

namespace Econocom.Admin.Controllers.GestionTraduction
{
    public class SecteurActiviteTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public SecteurActiviteTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /SecteurActiviteTraduction/
        public ActionResult Index()
        {
            try
            {
                //var listeSecteurActivite = _service.GetListeSecteurActivite();
                var listeSecteurActivite = _service.GetSecteurActivites();
                return View(listeSecteurActivite);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ActualiserContenu(SecteurActiviteTraductionViewModel secteurActiviteTraductionViewModel)
        {
            try
            {
                var secteurActiviteTraduction = new SecteurActiviteTraduction();
                secteurActiviteTraduction.InjectFrom(secteurActiviteTraductionViewModel);
                secteurActiviteTraduction.Id = secteurActiviteTraductionViewModel.Id.Value;
                _service.SetSecteurActiviteTraduction(secteurActiviteTraduction);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idSecteurActivite, int idLangue)
        {
            var secteurActiviteTraductionViewModel = new SecteurActiviteTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var ageDeviceTraduction = _service.GetSecteurActiviteTraduction(idSecteurActivite, idLangue);
                if (ageDeviceTraduction == null)
                {
                    ageDeviceTraduction = new SecteurActiviteTraduction();
                    ageDeviceTraduction.SecteurActiviteId = idSecteurActivite;
                }
                secteurActiviteTraductionViewModel.InjectFrom(ageDeviceTraduction);
                secteurActiviteTraductionViewModel.Id = ageDeviceTraduction.Id;
                secteurActiviteTraductionViewModel.LangueId = idLangue;
                secteurActiviteTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", secteurActiviteTraductionViewModel);
        }
    }
}
