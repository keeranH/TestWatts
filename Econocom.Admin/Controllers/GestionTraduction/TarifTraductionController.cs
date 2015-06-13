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
    public class TarifTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public TarifTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /AgeDeviceTraduction/
        public ActionResult Index()
        {
            try
            {
                var listeTarif = _service.GetListeTarif();
                return View(listeTarif);
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
        public ActionResult ActualiserContenu(TarifTraductionViewModel tarifTraductionViewModel)
        {
            try
            {
                var tarifTraduction = new TarifTraduction();
                tarifTraduction.InjectFrom(tarifTraductionViewModel);
                tarifTraduction.Id = tarifTraductionViewModel.Id.Value;
                _service.SetTarifTraduction(tarifTraduction);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idTarif, int idLangue)
        {
            var tarifTraductionViewModel = new TarifTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var tarifTraduction = _service.GetTarifTraduction(idTarif, idLangue);
                if (tarifTraduction == null)
                {
                    tarifTraduction = new TarifTraduction();
                    tarifTraduction.TarifId = idTarif;
                }
                tarifTraductionViewModel.InjectFrom(tarifTraduction);
                tarifTraductionViewModel.Id = tarifTraduction.Id;
                tarifTraductionViewModel.LangueId = idLangue;
                tarifTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", tarifTraductionViewModel);
        }
    }
}
