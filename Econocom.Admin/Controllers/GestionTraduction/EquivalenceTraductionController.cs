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
    public class EquivalenceTraductionController : Controller
    {
         private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;

        public EquivalenceTraductionController()
        {
            _service = new ServiceApiController();
        }

        //
        // GET: /AgeDeviceTraduction/
        public ActionResult Index()
        {
            try
            {
                var listeEquivalence = _service.GetListeEquivalence();
                return View(listeEquivalence);
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
        public ActionResult ActualiserContenu(EquivalenceTraductionViewModel equivalenceTraductionViewModel)
        {
            try
            {
                var equivalenceTraduction = new EquivalenceTraduction();
                equivalenceTraduction.InjectFrom(equivalenceTraductionViewModel);
                equivalenceTraduction.Id = equivalenceTraductionViewModel.Id.Value;
                _service.SetEquivalenceTraduction(equivalenceTraduction);
                return Json("<span class=\"succes\">Votre modification a ete prise en compte</span>", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return Json("<span class=\"erreur\">Votre modification a echoue</span>", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModifierContenu(int idEquivalence, int idLangue)
        {
            var equivalenceTraductionViewModel = new EquivalenceTraductionViewModel();
            try
            {
                var langues = _service.GetListeLangue();

                var equivalenceTraduction = _service.GetEquivalenceTraduction(idEquivalence, idLangue);
                if (equivalenceTraduction == null)
                {
                    equivalenceTraduction = new EquivalenceTraduction();
                    equivalenceTraduction.EquivalenceId = idEquivalence;
                }
                equivalenceTraductionViewModel.InjectFrom(equivalenceTraduction);
                equivalenceTraductionViewModel.Id = equivalenceTraduction.Id;
                equivalenceTraductionViewModel.LangueId = idLangue;
                equivalenceTraductionViewModel.ListeLangues = langues;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return PartialView("ContenuPartial", equivalenceTraductionViewModel);
        }
    }
}
